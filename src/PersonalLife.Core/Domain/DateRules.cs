namespace PersonalLife.Core;
public static class DateRules
{
 public static int GameMinutes(DateTimeOffset start,DateTimeOffset end){var m=(end-start).TotalMinutes;Validation.Require(m>0&&m<=int.MaxValue&&m==Math.Truncate(m),"结束必须晚于开始，时长需为正整数分钟");return (int)m;}
 public static DateOnly GameDate(DateTimeOffset start)=>DateOnly.FromDateTime(start.DateTime);
 public static (DateOnly Start,DateOnly End) Week(DateOnly date,string starts){Validation.Require(starts is "monday" or "sunday","一周起始日无效");var offset=((int)date.DayOfWeek-(starts=="monday"?1:0)+7)%7;var first=date.AddDays(-offset);return(first,first.AddDays(6));}
}
