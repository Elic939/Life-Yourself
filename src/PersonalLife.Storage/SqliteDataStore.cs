using System.Text.Json;
using Microsoft.Data.Sqlite;
using PersonalLife.Core;
namespace PersonalLife.Storage;
public sealed class SqliteDataStore : IDataStore
{
 readonly string path; readonly object gate=new(); long revision;
 public event Action? Changed;
 static readonly Dictionary<Type,string> Tables=new() {
 [typeof(DailyTask)]="daily_tasks",[typeof(Workout)]="workouts",[typeof(Exercise)]="exercises",
 [typeof(ExerciseSet)]="exercise_sets",[typeof(Meal)]="meals",[typeof(WaterEntry)]="water_entries",
 [typeof(GamePlan)]="game_plans",[typeof(GameSession)]="game_sessions",[typeof(JournalEntry)]="journals",[typeof(UserSettings)]="settings"};
 public SqliteDataStore(string path){this.path=Path.GetFullPath(path);Directory.CreateDirectory(Path.GetDirectoryName(this.path)!);using var c=Open();MigrationRunner.Run(c);}
 SqliteConnection Open(){var c=new SqliteConnection(new SqliteConnectionStringBuilder {DataSource=path,Pooling=false,ForeignKeys=true,DefaultTimeout=5}.ToString());try {c.Open();using var cmd=c.CreateCommand();cmd.CommandText="PRAGMA synchronous=FULL";cmd.ExecuteNonQuery();return c;}catch{c.Dispose();throw;}}
 static string Table(Type type)=>Tables.TryGetValue(type,out var name)?name:throw new ArgumentException("不支持的数据类型");
 static List<T> Read<T>(SqliteConnection c,SqliteTransaction tx){using var cmd=c.CreateCommand();cmd.Transaction=tx;cmd.CommandText=$"SELECT body FROM {Table(typeof(T))} ORDER BY rowid";using var r=cmd.ExecuteReader();var result=new List<T>();while(r.Read())result.Add(JsonSerializer.Deserialize<T>(r.GetString(0))!);return result;}
 public AppSnapshot ReadAll(){lock(gate){using var c=Open();using var tx=c.BeginTransaction();var s=new AppSnapshot{Tasks=Read<DailyTask>(c,tx),Workouts=Read<Workout>(c,tx),Exercises=Read<Exercise>(c,tx),ExerciseSets=Read<ExerciseSet>(c,tx),Meals=Read<Meal>(c,tx),WaterEntries=Read<WaterEntry>(c,tx),GamePlans=Read<GamePlan>(c,tx),GameSessions=Read<GameSession>(c,tx),Journals=Read<JournalEntry>(c,tx),Settings=Read<UserSettings>(c,tx).Single()};tx.Commit();return s;}}
 static void Write(SqliteConnection c,SqliteTransaction tx,object entity)
 {
   using var cmd=c.CreateCommand();cmd.Transaction=tx;cmd.CommandText=$"INSERT INTO {Table(entity.GetType())}(id,body) VALUES($id,$body) ON CONFLICT(id) DO UPDATE SET body=excluded.body";
   cmd.Parameters.AddWithValue("$id",entity is IEntity e?e.Id.ToString():"settings");cmd.Parameters.AddWithValue("$body",JsonSerializer.Serialize(entity,entity.GetType()));cmd.ExecuteNonQuery();
 }
 WriteReceipt Transaction(Action<SqliteConnection,SqliteTransaction> work){WriteReceipt result;lock(gate){using var c=Open();using var tx=c.BeginTransaction();work(c,tx);tx.Commit();result=new(++revision);}Changed?.Invoke();return result;}
 public WriteReceipt Upsert<T>(T entity) where T:class=>Transaction((c,t)=>Write(c,t,entity));
 public WriteReceipt Delete<T>(Guid id) where T:class=>Transaction((c,t)=>{using var cmd=c.CreateCommand();cmd.Transaction=t;cmd.CommandText=$"DELETE FROM {Table(typeof(T))} WHERE id=$id";cmd.Parameters.AddWithValue("$id",id.ToString());cmd.ExecuteNonQuery();});
 public WriteReceipt ReplaceAll(AppSnapshot snapshot)=>Transaction((c,t)=>{
   foreach(var table in Tables.Values.Reverse()){using var cmd=c.CreateCommand();cmd.Transaction=t;cmd.CommandText=$"DELETE FROM {table}";cmd.ExecuteNonQuery();}
   foreach(var e in snapshot.Entities())Write(c,t,e);Write(c,t,snapshot.Settings);
 });
 public void Dispose(){}
}
