using PersonalLife.Core;
using PersonalLife.Storage;
namespace PersonalLife.Tests;
[TestClass]
public class StorageTests
{
 [TestMethod] public void PersistAndReopen(){using var t=new TestStore();var item=new DailyTask{Date=new(2026,9,13),Title="读书"};t.Store.Upsert(item);using var reopened=new SqliteDataStore(t.PathName);Assert.AreEqual(item,reopened.ReadAll().Tasks.Single());}
 [TestMethod] public void RollbackFailedReplacement(){using var t=new TestStore();var item=new DailyTask{Date=new(2026,9,13),Title="保留我"};t.Store.Upsert(item);Assert.Throws<Exception>(()=>t.Store.ReplaceAll(new AppSnapshot{Tasks=[item with{Title="不应提交"}],Exercises=[new Exercise{WorkoutId=Guid.NewGuid(),Name="孤立动作"}]}));Assert.AreEqual(item,t.Store.ReadAll().Tasks.Single());}
 [TestMethod] public void MigrationDoesNotEraseData(){using var t=new TestStore();t.Store.Upsert(new WaterEntry{Date=new(2026,9,13),Milliliters=250});using var s=new SqliteDataStore(t.PathName);Assert.AreEqual(250,s.ReadAll().WaterEntries.Single().Milliliters);}
 [TestMethod] public void DatesAreUniqueForJournal(){using var t=new TestStore();var d=new DateOnly(2026,9,13);t.Store.Upsert(new JournalEntry{Date=d,Summary="第一条"});Assert.Throws<Exception>(()=>t.Store.Upsert(new JournalEntry{Date=d,Summary="第二条"}));Assert.HasCount(1,t.Store.ReadAll().Journals);}
 [TestMethod] public void CorruptFileIsNotOverwritten(){using var t=new TestStore();var p=Path.Combine(t.Folder,"broken.db");File.WriteAllText(p,"broken original");Assert.Throws<Exception>(()=>{using var s=new SqliteDataStore(p);s.ReadAll();});Assert.AreEqual("broken original",File.ReadAllText(p));}
}
