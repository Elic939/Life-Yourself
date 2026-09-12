using Microsoft.Data.Sqlite;
namespace PersonalLife.Storage;
internal static class MigrationRunner
{
 public static void Run(SqliteConnection connection)
 {
   using var check=connection.CreateCommand();check.CommandText="PRAGMA quick_check";if((string?)check.ExecuteScalar()!="ok")throw new InvalidDataException("数据文件损坏，请从备份恢复，原文件已保留。");
   using var exists=connection.CreateCommand();exists.CommandText="SELECT count(*) FROM sqlite_master WHERE type='table' AND name='schema_meta'";
   if(Convert.ToInt32(exists.ExecuteScalar())>0){using var version=connection.CreateCommand();version.CommandText="SELECT version FROM schema_meta";if(Convert.ToInt32(version.ExecuteScalar())!=1)throw new InvalidDataException("数据版本不受支持，原文件已保留。");}
   using var stream=typeof(MigrationRunner).Assembly.GetManifestResourceStream("PersonalLife.Storage.Migrations.001_initial.sql")!;
   using var reader=new StreamReader(stream);using var tx=connection.BeginTransaction();using var cmd=connection.CreateCommand();cmd.Transaction=tx;cmd.CommandText=reader.ReadToEnd();cmd.ExecuteNonQuery();tx.Commit();
 }
}
