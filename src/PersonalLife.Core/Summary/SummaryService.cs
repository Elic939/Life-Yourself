namespace PersonalLife.Core;
public sealed record ModuleLink(PageId Page,DateOnly Date,string Title,TimeOnly? Time);
public sealed record DaySummary(int TaskTotal,int TaskCompleted,long WaterMl,long GameMinutes,long WeekGameMinutes,int RecordedMeals,bool HasJournal,string? Mood,List<ModuleLink> Links,int ActualSets){public long ActualTrainingMinutes {get;init;}}
public static class SummaryService
{
 public static DaySummary Build(AppSnapshot s,DateOnly date,string weekStart)
 {
  var tasks=s.Tasks.Where(x=>x.Date==date).ToList();var week=DateRules.Week(date,weekStart);var workouts=s.Workouts.Where(x=>x.Date==date).ToList();var ids=s.Exercises.Where(x=>workouts.Any(w=>w.Id==x.WorkoutId)).Select(x=>x.Id).ToHashSet();var journal=s.Journals.SingleOrDefault(x=>x.Date==date);
  var links=workouts.Select(x=>new ModuleLink(PageId.Fitness,date,x.IsRestDay?"休息日":x.Title,x.StartTime)).Concat(s.Meals.Where(x=>x.Date==date&&!string.IsNullOrWhiteSpace(x.PlannedText)).Select(x=>new ModuleLink(PageId.Meals,date,MealTitle(x.Slot)+" · "+x.PlannedText,null))).Concat(s.GamePlans.Where(x=>x.Date==date).Select(x=>new ModuleLink(PageId.Gaming,date,x.GameName,x.StartTime))).OrderBy(x=>x.Time==null).ThenBy(x=>x.Time).ToList();
  return new(tasks.Count,tasks.Count(x=>x.IsCompleted),s.WaterEntries.Where(x=>x.Date==date).Sum(x=>(long)x.Milliliters),s.GameSessions.Where(x=>x.Date==date).Sum(x=>(long)x.DurationMinutes),s.GameSessions.Where(x=>x.Date>=week.Start&&x.Date<=week.End).Sum(x=>(long)x.DurationMinutes),s.Meals.Count(x=>x.Date==date&&x.IsRecorded),journal!=null,journal?.Mood,links,s.ExerciseSets.Count(x=>ids.Contains(x.ExerciseId))){ActualTrainingMinutes=s.Exercises.Where(x=>ids.Contains(x.Id)&&x.Kind=="duration").Sum(x=>(long)(x.ActualMinutes??0))};
 }
 public static string MealTitle(string slot)=>slot switch{"breakfast"=>"早餐","lunch"=>"午餐","dinner"=>"晚餐","snack"=>"加餐",_=>slot};
}
