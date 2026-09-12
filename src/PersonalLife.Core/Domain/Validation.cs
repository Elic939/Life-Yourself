namespace PersonalLife.Core;
public static class Validation
{
 public static void Require(bool condition,string message){if(!condition)throw new ArgumentException(message);}
 public static void Task(DailyTask a){Require(a.Id!=Guid.Empty,"记录标识不能为空");Require(!string.IsNullOrWhiteSpace(a.Title),"事项名称不能为空");Require(a.Priority is "normal" or "important","请选择有效优先级");}
}
