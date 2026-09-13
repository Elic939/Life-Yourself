using PersonalLife.Core;
namespace PersonalLife.Tests;
[TestClass]public class MealTests
{
 [TestMethod]public void ActualDoesNotOverwritePlan(){using var t=new TestStore();var s=new MealService(t.Store);var m=new Meal{Date=new(2026,9,13),Slot="dinner",PlannedText="番茄面"};s.Save(m);s.CopyPlanToActual(m.Id);Assert.AreEqual("番茄面",t.Store.ReadAll().Meals.Single().ActualText);s.Save(t.Store.ReadAll().Meals.Single() with{ActualText="米饭"});Assert.AreEqual("番茄面",t.Store.ReadAll().Meals.Single().PlannedText);Assert.IsTrue(t.Store.ReadAll().Meals.Single().IsRecorded);}
 [TestMethod]public void WaterTotalsAndDeletion(){using var t=new TestStore();var s=new MealService(t.Store);var w=new WaterEntry{Date=new(2026,9,13),Milliliters=250};s.Save(w);s.Save(w with{Id=Guid.NewGuid(),Milliliters=300});Assert.AreEqual(550,t.Store.ReadAll().WaterEntries.Sum(x=>x.Milliliters));t.Store.Delete<WaterEntry>(w.Id);Assert.AreEqual(300,t.Store.ReadAll().WaterEntries.Sum(x=>x.Milliliters));}
 [TestMethod]public void ClearActualResetsRecorded(){using var t=new TestStore();var s=new MealService(t.Store);var m=new Meal{Date=new(2026,9,13),ActualText="鸡蛋"};s.Save(m);s.Save(m with{ActualText="",IsRecorded=true});Assert.IsFalse(t.Store.ReadAll().Meals.Single().IsRecorded);}
 [TestMethod]public void InvalidWaterRejected(){using var t=new TestStore();var s=new MealService(t.Store);Assert.Throws<ArgumentException>(()=>s.Save(new WaterEntry{Milliliters=-1}));Assert.Throws<ArgumentException>(()=>s.Save(new WaterEntry{Milliliters=0}));}
}
