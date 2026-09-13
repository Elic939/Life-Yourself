namespace PersonalLife.Core;
public sealed class GamingService(IDataStore store)
{
 public WriteReceipt Save(GamePlan x){Validation.Require(!string.IsNullOrWhiteSpace(x.GameName),"游戏名称不能为空");Validation.Require(x.PlannedMinutes>0,"预计分钟必须大于0");return store.Upsert(x);}
 public WriteReceipt Save(GameSession x){var normalized=Normalize(x);return store.Upsert(normalized);}
 public static GameSession Normalize(GameSession x){Validation.Require(!string.IsNullOrWhiteSpace(x.GameName),"游戏名称不能为空");Validation.Require(x.Mode is "range" or "manual","记录方式无效");if(x.Mode=="range"){Validation.Require(x.StartedAt!=null&&x.EndedAt!=null,"请填写起止时间");return x with{Date=DateRules.GameDate(x.StartedAt!.Value),DurationMinutes=DateRules.GameMinutes(x.StartedAt.Value,x.EndedAt!.Value)};}Validation.Require(x.DurationMinutes>0,"实际分钟必须大于0");return x with{StartedAt=null,EndedAt=null};}
 public long GetWeekMinutes(DateOnly d,string start){var w=DateRules.Week(d,start);return store.ReadAll().GameSessions.Where(x=>x.Date>=w.Start&&x.Date<=w.End).Sum(x=>(long)x.DurationMinutes);}
 public List<GameSession> GetForDate(DateOnly d)=>store.ReadAll().GameSessions.Where(x=>x.Date==d).ToList();
}
