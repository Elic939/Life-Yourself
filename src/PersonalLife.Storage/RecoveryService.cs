using PersonalLife.Core;
namespace PersonalLife.Storage;
public static class RecoveryService
{
 public static string? ConfirmAndRestore(string databasePath,string backupPath,Func<BackupPreview,bool> confirm){var preview=BackupService.ReadPreview(backupPath);return confirm(preview)?RestoreDamagedDatabase(databasePath,preview):null;}
 public static string RestoreDamagedDatabase(string databasePath,string backupPath)
 =>RestoreDamagedDatabase(databasePath,BackupService.ReadPreview(backupPath));
 public static string RestoreDamagedDatabase(string databasePath,BackupPreview preview)
 {
  var full=Path.GetFullPath(databasePath);var temp=full+".recovery-"+Guid.NewGuid().ToString("N");var preserved=full+".preserved-"+Guid.NewGuid().ToString("N");
  try{using(var recovered=new SqliteDataStore(temp)){new BackupService(recovered).Restore(preview);}if(File.Exists(full))File.Replace(temp,full,preserved);else File.Move(temp,full);return preserved;}
  finally{if(File.Exists(temp))File.Delete(temp);}
 }
}
