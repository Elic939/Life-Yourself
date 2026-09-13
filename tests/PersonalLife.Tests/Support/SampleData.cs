using PersonalLife.Core;
namespace PersonalLife.Tests;
internal static class SampleData
{
 public static AppSnapshot Create(DateOnly date){var w=new Workout{Date=date,Title="上肢力量训练",StartTime=new(18,0),PlannedMinutes=45};var e=new Exercise{WorkoutId=w.Id,Name="哑铃卧推",PlannedSets=3,PlannedReps=10,PlannedWeightKg=10};return new(){Tasks=[new(){Date=date,Title="读书 20 分钟",Time=new(9,0),IsCompleted=true},new(){Date=date,Title="整理房间",Time=new(16,0),Priority="important"}],Workouts=[w],Exercises=[e],ExerciseSets=[new(){ExerciseId=e.Id,SetNumber=1,ActualReps=10,ActualWeightKg=10}],Meals=[new(){Date=date,Slot="breakfast",PlannedText="燕麦、鸡蛋",ActualText="燕麦、鸡蛋",IsRecorded=true},new(){Date=date,Slot="dinner",PlannedText="番茄鸡蛋面"}],WaterEntries=[new(){Date=date,Milliliters=500}],GamePlans=[new(){Date=date,GameName="星露谷物语",StartTime=new(20,0),PlannedMinutes=60}],GameSessions=[new(){Date=date,GameName="星露谷物语",DurationMinutes=30}],Journals=[new(){Date=date,Mood="平静",Summary="今天完成了阅读，晚上继续训练。",Gains="安排时间后更从容了。",UpdatedAt=DateTimeOffset.Now}]};}
}
