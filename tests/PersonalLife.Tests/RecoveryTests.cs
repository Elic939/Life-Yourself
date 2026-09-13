using PersonalLife.Core;
using PersonalLife.Storage;
namespace PersonalLife.Tests;
[TestClass]public class RecoveryTests
{
 [TestMethod]public void RejectedPreviewLeavesOriginalDamagedFile(){using var t=new TestStore();t.Store.Upsert(new DailyTask{Title="预览内容"});var backup=Path.Combine(t.Folder,"valid.json");new BackupService(t.Store).Export(backup);var damaged=Path.Combine(t.Folder,"damaged.db");File.WriteAllText(damaged,"keep original");var seen=false;var result=RecoveryService.ConfirmAndRestore(damaged,backup,p=>{seen=true;Assert.HasCount(1,p.Data.Tasks);Assert.IsGreaterThan(DateTimeOffset.MinValue,p.ExportedAt);return false;});Assert.IsTrue(seen);Assert.IsNull(result);Assert.AreEqual("keep original",File.ReadAllText(damaged));}
 [TestMethod]public void RestorePreservesOriginalDamagedFile(){using var t=new TestStore();t.Store.Upsert(new DailyTask{Title="恢复成功",Date=new(2026,9,13)});var backup=Path.Combine(t.Folder,"valid.json");new BackupService(t.Store).Export(backup);var damaged=Path.Combine(t.Folder,"damaged.db");File.WriteAllText(damaged,"original damaged bytes");var preserved=RecoveryService.RestoreDamagedDatabase(damaged,backup);Assert.AreEqual("original damaged bytes",File.ReadAllText(preserved));using var restored=new SqliteDataStore(damaged);Assert.AreEqual("恢复成功",restored.ReadAll().Tasks.Single().Title);}
 [TestMethod]public void InvalidBackupCannotReplaceDamagedFile(){using var t=new TestStore();var damaged=Path.Combine(t.Folder,"damaged.db");var backup=Path.Combine(t.Folder,"bad.json");File.WriteAllText(damaged,"keep");File.WriteAllText(backup,"{}");Assert.Throws<Exception>(()=>RecoveryService.RestoreDamagedDatabase(damaged,backup));Assert.AreEqual("keep",File.ReadAllText(damaged));}
}
