using PersonalLife.Core;
namespace PersonalLife.Tests;
[TestClass]public class JournalTests
{
 [TestMethod]public void OneEntryPerDate(){using var t=new TestStore();var s=new JournalService(t.Store);var d=new DateOnly(2026,9,13);s.Save(new JournalEntry{Date=d,Summary="开始"});var id=t.Store.ReadAll().Journals.Single().Id;s.Save(new JournalEntry{Date=d,Summary="后来"});Assert.HasCount(1,t.Store.ReadAll().Journals);Assert.AreEqual(id,t.Store.ReadAll().Journals.Single().Id);Assert.AreEqual("后来",t.Store.ReadAll().Journals.Single().Summary);}
 [TestMethod]public void PartialEntryPersists(){using var t=new TestStore();var s=new JournalService(t.Store);s.Save(new JournalEntry{Date=new(2026,9,13),Gains="认真吃饭",Mood="平静"});Assert.AreEqual("认真吃饭",t.Store.ReadAll().Journals.Single().Gains);Assert.IsTrue(t.Store.ReadAll().Journals.Single().UpdatedAt>DateTimeOffset.MinValue);}
 [TestMethod]public void EmptyNewEntryIsNotCreated(){using var t=new TestStore();new JournalService(t.Store).Save(new JournalEntry{Date=new(2026,9,13)});Assert.IsEmpty(t.Store.ReadAll().Journals);}
}
