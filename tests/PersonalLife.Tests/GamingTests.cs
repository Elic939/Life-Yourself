using PersonalLife.Core;
namespace PersonalLife.Tests;
[TestClass]public class GamingTests
{
 [TestMethod]public void CrossMidnightUsesStartDate(){var start=new DateTimeOffset(2026,9,12,23,30,0,TimeSpan.FromHours(8));Assert.AreEqual(60,DateRules.GameMinutes(start,start.AddHours(1)));Assert.AreEqual(new DateOnly(2026,9,12),DateRules.GameDate(start));}
 [TestMethod]public void ActualTotalsAndDeletion(){using var t=new TestStore();var s=new GamingService(t.Store);var a=new GameSession{Date=new(2026,9,13),GameName="星露谷",DurationMinutes=45};s.Save(a);var b=a with{Id=Guid.NewGuid(),DurationMinutes=30};s.Save(b);Assert.AreEqual(75,s.GetWeekMinutes(a.Date,"monday"));t.Store.Delete<GameSession>(b.Id);Assert.AreEqual(45,s.GetForDate(a.Date).Sum(x=>x.DurationMinutes));}
 [TestMethod]public void RangeRecomputesMinutesAndDate(){using var t=new TestStore();var s=new GamingService(t.Store);var start=new DateTimeOffset(2026,9,12,23,30,0,TimeSpan.FromHours(8));s.Save(new GameSession{GameName="游戏",Mode="range",StartedAt=start,EndedAt=start.AddHours(1),DurationMinutes=999});var x=t.Store.ReadAll().GameSessions.Single();Assert.AreEqual(60,x.DurationMinutes);Assert.AreEqual(new DateOnly(2026,9,12),x.Date);}
 [TestMethod]public void WeekStartsAndYearBoundary(){Assert.AreEqual(new DateOnly(2025,12,29),DateRules.Week(new(2026,1,1),"monday").Start);Assert.AreEqual(new DateOnly(2026,9,13),DateRules.Week(new(2026,9,13),"sunday").Start);Assert.AreEqual(new DateOnly(2026,9,7),DateRules.Week(new(2026,9,13),"monday").Start);}
 [TestMethod]public void InvalidDurationRejected(){Assert.Throws<ArgumentException>(()=>DateRules.GameMinutes(DateTimeOffset.Now,DateTimeOffset.Now.AddHours(-1)));using var t=new TestStore();Assert.Throws<ArgumentException>(()=>new GamingService(t.Store).Save(new GameSession{GameName="游戏",DurationMinutes=-5}));}
}
