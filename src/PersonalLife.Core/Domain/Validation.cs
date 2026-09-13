namespace PersonalLife.Core;
public static class Validation
{
 public static void Require(bool condition,string message){if(!condition)throw new ArgumentException(message);}
 public static void Task(DailyTask a){Require(a.Id!=Guid.Empty,"记录标识不能为空");Require(!string.IsNullOrWhiteSpace(a.Title),"事项名称不能为空");Require(a.Priority is "normal" or "important","请选择有效优先级");}
 public static void Workout(Workout a){Require(a.IsRestDay||!string.IsNullOrWhiteSpace(a.Title),"训练主题不能为空");Require(a.PlannedMinutes is null or >0,"预计时长必须大于0");}
 public static void Exercise(Exercise a){Require(!string.IsNullOrWhiteSpace(a.Name),"动作名称不能为空");Require(a.Kind is "strength" or "duration","请选择动作类型");Require(a.PlannedWeightKg is null or >=0,"重量不能为负数");Require(a.SortOrder>=0,"顺序不能为负数");if(a.Kind=="strength"){Require(a.PlannedSets>0&&a.PlannedReps>0,"计划组数和次数必须大于0");}else {Require(a.PlannedMinutes>0,"计划分钟必须大于0");Require(a.ActualMinutes is null or >=0,"实际分钟不能为负数");}}
 public static void Set(ExerciseSet a){Require(a.SetNumber>0,"组号必须大于0");Require(a.ActualReps>=0,"次数不能为负数");Require(a.ActualWeightKg is null or >=0,"重量不能为负数");}
}
