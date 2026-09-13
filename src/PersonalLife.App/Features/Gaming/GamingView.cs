using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using PersonalLife.Core;
namespace PersonalLife.App;
public partial class MainWindow
{
 void RenderGaming()
 {
  var data=store.ReadAll();var plans=data.GamePlans.Where(x=>x.Date==SelectedDate).ToList();var actual=data.GameSessions.Where(x=>x.Date==SelectedDate).ToList();var total=Ui.Card(PageContent,"游戏用时");total.Children.Add(Ui.Text($"今日实际 {actual.Sum(x=>(long)x.DurationMinutes)} 分钟 / 计划 {plans.Sum(x=>(long)x.PlannedMinutes)} 分钟",22));total.Children.Add(Ui.Text($"所选日期所在周累计 {new GamingService(store).GetWeekMinutes(SelectedDate,data.Settings.WeekStartsOn)} 分钟"));
  var planned=Ui.Card(PageContent,"娱乐安排");planned.Children.Add(Ui.Button("＋ 安排娱乐时间",()=>EditGamePlan(new GamePlan{Date=SelectedDate,StartTime=new(20,0)})));if(plans.Count==0)planned.Children.Add(Ui.Text("今天尚未安排娱乐时间"));foreach(var p in plans){var row=Ui.Row(planned);row.Children.Add(Ui.Text($"{p.GameName} · {p.StartTime:HH:mm} · 计划 {p.PlannedMinutes} 分钟"));row.Children.Add(Ui.Button("编辑",()=>EditGamePlan(p)));row.Children.Add(Ui.Button("删除",()=>Run(()=>undo.Delete<GamePlan>(p.Id))));}
  var records=Ui.Card(PageContent,"实际游玩记录");records.Children.Add(Ui.Button("＋ 记录游戏时长",()=>EditGameSession(new GameSession{Date=SelectedDate})));if(actual.Count==0)records.Children.Add(Ui.Text("尚无实际记录"));foreach(var a in actual){var row=Ui.Row(records);row.Children.Add(Ui.Text($"{a.GameName} · {a.DurationMinutes} 分钟"+(a.Mode=="range"?$" · {a.StartedAt:MM-dd HH:mm}—{a.EndedAt:MM-dd HH:mm}":" · 手动记录")));row.Children.Add(Ui.Button("编辑",()=>EditGameSession(a)));row.Children.Add(Ui.Button("删除",()=>Run(()=>undo.Delete<GameSession>(a.Id))));}
 }
 async void EditGamePlan(GamePlan item){if(!await StartEditor("娱乐时间安排"))return;var name=Ui.Field(DrawerContent,"游戏名称",item.GameName);var date=Ui.Field(DrawerContent,"日期",item.Date.ToString("yyyy-MM-dd"));var start=Ui.Field(DrawerContent,"开始时间 HH:mm",item.StartTime.ToString("HH:mm"));var minutes=Ui.Field(DrawerContent,"预计分钟",item.PlannedMinutes==0?"":item.PlannedMinutes.ToString());Observe("gameplan-"+item.Id,()=>{var d=item with{GameName=name.Text,Date=Ui.Date(date.Text),StartTime=Ui.Time(start.Text)??throw new ArgumentException("开始时间必填"),PlannedMinutes=Ui.Number(minutes.Text,"预计分钟")};Validation.Require(!string.IsNullOrWhiteSpace(d.GameName)&&d.PlannedMinutes>0,"游戏名称必填，预计分钟必须大于0");return()=>new GamingService(store).Save(d);},name,date,start,minutes);FinishEditor();}
 async void EditGameSession(GameSession item)
 {
  if(!await StartEditor("实际游戏时长"))return;var name=Ui.Field(DrawerContent,"游戏名称",item.GameName);var mode=Ui.Choice(DrawerContent,"记录方式",["直接填写时长","填写开始与结束"],item.Mode=="range"?1:0);var direct=new StackPanel();DrawerContent.Children.Add(direct);var date=Ui.Field(direct,"归属日期",item.Date.ToString("yyyy-MM-dd"));var minutes=Ui.Field(direct,"实际分钟",item.DurationMinutes==0?"":item.DurationMinutes.ToString());var range=new StackPanel();DrawerContent.Children.Add(range);range.Children.Add(Ui.Text("跨午夜记录归属开始日期。时间格式：yyyy-MM-dd HH:mm",12));var start=Ui.Field(range,"开始",item.StartedAt?.ToString("yyyy-MM-dd HH:mm")??"");var end=Ui.Field(range,"结束",item.EndedAt?.ToString("yyyy-MM-dd HH:mm")??"");void Mode(){direct.Visibility=mode.SelectedIndex==0?Visibility.Visible:Visibility.Collapsed;range.Visibility=mode.SelectedIndex==1?Visibility.Visible:Visibility.Collapsed;}mode.SelectionChanged+=(_,_)=>Mode();Mode();
  Observe("gamesession-"+item.Id,()=>{DateTimeOffset Parse(string s)=>DateTime.TryParseExact(s,"yyyy-MM-dd HH:mm",CultureInfo.InvariantCulture,DateTimeStyles.None,out var dt)?new DateTimeOffset(dt,TimeZoneInfo.Local.GetUtcOffset(dt)):throw new ArgumentException("起止时间格式错误");var d=mode.SelectedIndex==0?item with{GameName=name.Text,Mode="manual",Date=Ui.Date(date.Text),DurationMinutes=Ui.Number(minutes.Text,"实际分钟"),StartedAt=null,EndedAt=null}:item with{GameName=name.Text,Mode="range",StartedAt=Parse(start.Text),EndedAt=Parse(end.Text)};d=GamingService.Normalize(d);return()=>new GamingService(store).Save(d);},name,mode,date,minutes,start,end);FinishEditor();
 }
}
