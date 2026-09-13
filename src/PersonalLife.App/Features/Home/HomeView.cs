using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using PersonalLife.Core;
namespace PersonalLife.App;
public partial class MainWindow
{
 void RenderHome()
 {
  var data=store.ReadAll();var s=SummaryService.Build(data,SelectedDate,data.Settings.WeekStartsOn);var top=Ui.Card(PageContent,"今天，按自己的节奏来");top.Children.Add(Ui.Text(s.TaskTotal==0?"今天尚无日常事项":$"日常事项 {s.TaskCompleted} / {s.TaskTotal} 已完成",24));if(s.TaskTotal>0)top.Children.Add(new ProgressBar{Maximum=s.TaskTotal,Value=s.TaskCompleted,Height=6,Margin=new Thickness(0,8,0,12)});
  var upcoming=data.Tasks.Where(x=>x.Date==SelectedDate&&!x.IsCompleted&&x.Time!=null).Select(x=>new ModuleLink(PageId.Tasks,SelectedDate,x.Title,x.Time)).Concat(s.Links.Where(x=>x.Time!=null)).Where(x=>SelectedDate!=DateOnly.FromDateTime(DateTime.Now)||x.Time>=TimeOnly.FromDateTime(DateTime.Now)).OrderBy(x=>x.Time).FirstOrDefault();top.Children.Add(Ui.Text(upcoming==null?"暂时没有接下来的定时安排":$"接下来　{upcoming.Time:HH:mm} · {upcoming.Title}"));
  var grid=new UniformGrid{Columns=ActualWidth<1100?1:2};PageContent.Children.Add(grid);grid.SizeChanged+=(_,_)=>grid.Columns=grid.ActualWidth<760?1:2;
  var tasks=Ui.Card(grid,"今日计划");foreach(var item in data.Tasks.Where(x=>x.Date==SelectedDate).OrderBy(x=>x.IsCompleted).ThenBy(x=>x.Time).Take(5))TaskRow(tasks,item);if(s.TaskTotal==0)tasks.Children.Add(Ui.Text("安排一件今天想完成的小事"));tasks.Children.Add(Ui.Button("查看今日计划 →",async()=>await Navigate(PageId.Tasks)));
  var fitness=Ui.Card(grid,"今日健身");var w=data.Workouts.SingleOrDefault(x=>x.Date==SelectedDate);fitness.Children.Add(Ui.Text(w==null?"尚未安排训练":w.IsRestDay?"今天休息":w.Title,20));if(w!=null){fitness.Children.Add(Ui.Text($"已记录 {s.ActualSets} 组 · {(w.IsCompleted?"训练完成":"进行中 / 未开始")}"));foreach(var e in data.Exercises.Where(x=>x.WorkoutId==w.Id).Take(3))fitness.Children.Add(Ui.Text(e.Kind=="strength"?$"{e.Name} · {e.PlannedSets}×{e.PlannedReps}":$"{e.Name} · 计划 {e.PlannedMinutes} 分钟 / 实际 {e.ActualMinutes?.ToString()??"—"} 分钟"));}fitness.Children.Add(Ui.Button("查看健身计划 →",async()=>await Navigate(PageId.Fitness)));
  var meals=Ui.Card(grid,"今日饮食");foreach(var slot in new[]{"breakfast","lunch","dinner","snack"}){var m=data.Meals.SingleOrDefault(x=>x.Date==SelectedDate&&x.Slot==slot);meals.Children.Add(Ui.Text($"{SummaryService.MealTitle(slot)} · {m?.PlannedText??"尚未安排"} · {(m?.IsRecorded==true?"已记录":"待记录")}"));}meals.Children.Add(Ui.Text($"饮水 {s.WaterMl} ml"));meals.Children.Add(Ui.Button("查看饮食计划 →",async()=>await Navigate(PageId.Meals)));
  var game=Ui.Card(grid,"娱乐与复盘");foreach(var p in data.GamePlans.Where(x=>x.Date==SelectedDate))game.Children.Add(Ui.Text($"{p.StartTime:HH:mm} · {p.GameName} · 计划 {p.PlannedMinutes} 分钟"));game.Children.Add(Ui.Text($"今日游戏 {s.GameMinutes} 分钟 · 本周 {s.WeekGameMinutes} 分钟"));game.Children.Add(Ui.Text(s.HasJournal?$"已写日志 · {s.Mood??"未选择心情"}":"还没有写今日总结"));game.Children.Add(Ui.Button("查看游戏娱乐 →",async()=>await Navigate(PageId.Gaming)));game.Children.Add(Ui.Button(s.HasJournal?"编辑今日日志 →":"写今日总结 →",async()=>await Navigate(PageId.Journal)));
 }
 void AddModuleLinks(){var c=Ui.Card(PageContent,"生活安排");var s=SummaryService.Build(store.ReadAll(),SelectedDate,store.ReadAll().Settings.WeekStartsOn);if(s.Links.Count==0)c.Children.Add(Ui.Text("各模块还没有当天安排"));foreach(var link in s.Links)c.Children.Add(Ui.Button($"{link.Time?.ToString("HH:mm")??"按餐次 / 未定时间"} · {link.Title} →",async()=>await Navigate(link.Page,link.Date)));}
 void AddJournalSummary(){var s=SummaryService.Build(store.ReadAll(),SelectedDate,store.ReadAll().Settings.WeekStartsOn);var c=Ui.Card(PageContent,"今天的足迹");c.Children.Add(Ui.Text($"日常事项 {s.TaskCompleted}/{s.TaskTotal} · 力量 {s.ActualSets} 组 / 有氧 {s.ActualTrainingMinutes} 分钟 · 已记录 {s.RecordedMeals} 餐 · 饮水 {s.WaterMl} ml · 游戏 {s.GameMinutes} 分钟"));}
}
