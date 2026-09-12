using PersonalLife.Core;
namespace PersonalLife.Tests;
[TestClass]public class UndoTests
{
 [TestMethod]public void RestoresSameIdWithoutErasingOtherEdits(){using var t=new TestStore();var u=new UndoService(t.Store);var a=new DailyTask{Date=new(2026,9,13),Title="恢复"};t.Store.Upsert(a);u.Delete<DailyTask>(a.Id);var other=a with{Id=Guid.NewGuid(),Title="新条目"};t.Store.Upsert(other);u.Undo();CollectionAssert.AreEquivalent(new[]{a,other},t.Store.ReadAll().Tasks);}
 [TestMethod]public void ExpiredUndoCannotRestore(){using var t=new TestStore();var now=DateTimeOffset.Now;var u=new UndoService(t.Store,()=>now);var a=new DailyTask{Title="过期"};t.Store.Upsert(a);u.Delete<DailyTask>(a.Id);now=now.AddSeconds(11);Assert.IsFalse(u.CanUndo);Assert.Throws<InvalidOperationException>(()=>u.Undo());Assert.IsEmpty(t.Store.ReadAll().Tasks);}
}
