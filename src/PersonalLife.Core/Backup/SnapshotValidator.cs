using System.Text.Json;
namespace PersonalLife.Core;
public static class SnapshotValidator
{
 public static void Validate(AppSnapshot s)
 {
  Validation.Require(s.Tasks!=null&&s.Workouts!=null&&s.Exercises!=null&&s.ExerciseSets!=null&&s.Meals!=null&&s.WaterEntries!=null&&s.GamePlans!=null&&s.GameSessions!=null&&s.Journals!=null&&s.Settings!=null,"备份缺少模块");
  var entities=s.Entities().ToList();Validation.Require(entities.All(x=>x!=null&&x.Id!=Guid.Empty),"记录标识无效");Validation.Require(entities.Select(x=>x.Id).Distinct().Count()==entities.Count,"记录标识重复");
  foreach(var x in s.Tasks!)Validation.Task(x);
  foreach(var x in s.Workouts!){Validation.Workout(x);Validation.Require(!x.IsRestDay||!s.Exercises!.Any(e=>e.WorkoutId==x.Id),"休息日不能包含训练动作");}
  foreach(var x in s.Exercises!){Validation.Exercise(x);Validation.Require(s.Workouts!.Any(w=>w.Id==x.WorkoutId),"动作缺少所属训练");}
  foreach(var x in s.ExerciseSets!){Validation.Set(x);Validation.Require(s.Exercises!.Any(e=>e.Id==x.ExerciseId&&e.Kind=="strength"),"组记录缺少力量动作");}
  foreach(var x in s.Meals!){Validation.Require(x.Slot is "breakfast" or "lunch" or "dinner" or "snack","餐次无效");Validation.Require(x.PlannedText!=null&&x.ActualText!=null,"餐食文本无效");Validation.Require(x.IsRecorded==!string.IsNullOrWhiteSpace(x.ActualText),"餐食记录状态不一致");}
  foreach(var x in s.WaterEntries!)Validation.Require(x.Milliliters>0,"饮水量无效");
  foreach(var x in s.GamePlans!)Validation.Require(!string.IsNullOrWhiteSpace(x.GameName)&&x.PlannedMinutes>0,"娱乐计划无效");
  foreach(var x in s.GameSessions!)Validation.Require(GamingService.Normalize(x)==x,"游戏日期或时长不一致");
  foreach(var x in s.Journals!)Validation.Require(x.Summary!=null&&x.Gains!=null&&x.Improvements!=null,"日志文本无效");
  Validation.Require(s.Settings!.WeekStartsOn is "monday" or "sunday","一周起始日无效");
  Validation.Require(s.Workouts!.Select(x=>x.Date).Distinct().Count()==s.Workouts!.Count,"训练日期重复");
  Validation.Require(s.Journals!.Select(x=>x.Date).Distinct().Count()==s.Journals!.Count,"日志日期重复");
  Validation.Require(s.Meals!.Select(x=>(x.Date,x.Slot)).Distinct().Count()==s.Meals!.Count,"餐次重复");
  Validation.Require(s.ExerciseSets!.Select(x=>(x.ExerciseId,x.SetNumber)).Distinct().Count()==s.ExerciseSets!.Count,"组号重复");
 }
 public static void RequireShape(JsonElement element,Type type)
 {
  Validation.Require(element.ValueKind==JsonValueKind.Object,"备份对象格式错误");
  var names=element.EnumerateObject().Select(p=>p.Name).ToList();Validation.Require(names.Distinct().Count()==names.Count,"备份包含重复字段");
  foreach(var p in type.GetProperties()){var name=JsonNamingPolicy.CamelCase.ConvertName(p.Name);Validation.Require(element.TryGetProperty(name,out _),"备份缺少字段："+name);}
 }
}
