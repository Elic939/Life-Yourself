using PersonalLife.Core;
namespace PersonalLife.Tests;
[TestClass]public class TaskTests
{
 [TestMethod]public void CompletionAndPriorityPersist(){using var t=new TestStore();var s=new TaskService(t.Store);var a=new DailyTask{Date=new(2026,9,13),Title="采购",Priority="important"};s.Save(a);s.Save(a with{IsCompleted=true});Assert.IsTrue(s.GetForDate(a.Date).Single().IsCompleted);Assert.AreEqual("important",s.GetForDate(a.Date).Single().Priority);}
 [TestMethod]public void MoveDate(){using var t=new TestStore();var s=new TaskService(t.Store);var a=new DailyTask{Date=new(2026,9,13),Title="读书"};s.Save(a);s.Save(a with{Date=a.Date.AddDays(1)});Assert.IsEmpty(s.GetForDate(a.Date));Assert.HasCount(1,s.GetForDate(a.Date.AddDays(1)));}
 [TestMethod]public void EmptyTitleRejected(){using var t=new TestStore();var s=new TaskService(t.Store);Assert.Throws<ArgumentException>(()=>s.Save(new DailyTask{Title="  "}));Assert.IsEmpty(t.Store.ReadAll().Tasks);}
}
