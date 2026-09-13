using System.Windows;
using System.Windows.Controls;
using PersonalLife.Core;
namespace PersonalLife.App;
public partial class MainWindow
{
 void RenderTasks()
 {
  PageContent.Children.Add(Ui.Button("＋ 添加事项",()=>EditTask(new DailyTask{Date=SelectedDate})));
  var tasks=new TaskService(store).GetForDate(SelectedDate);
  foreach(var group in new[]{("按时间安排",tasks.Where(x=>!x.IsCompleted&&x.Time!=null).OrderBy(x=>x.Time).ToList()),("未指定时间",tasks.Where(x=>!x.IsCompleted&&x.Time==null).ToList())}){var card=Ui.Card(PageContent,group.Item1);if(group.Item2.Count==0)card.Children.Add(Ui.Text("暂无事项"));foreach(var item in group.Item2)TaskRow(card,item);}
  var done=new StackPanel();foreach(var item in tasks.Where(x=>x.IsCompleted))TaskRow(done,item);PageContent.Children.Add(new Expander{Header=$"已完成 · {tasks.Count(x=>x.IsCompleted)}",Content=done});
 }
 void TaskRow(Panel p,DailyTask item){var row=Ui.Row(p);Ui.Check(row,$"{item.Time?.ToString("HH:mm")??"随时"}  {item.Title}{(item.Priority=="important"?" · 重要":"")}",item.IsCompleted,value=>Run(()=>new TaskService(store).Save(item with{IsCompleted=value})));row.Children.Add(Ui.Button("编辑",()=>EditTask(item)));row.Children.Add(Ui.Button("删除",()=>Run(()=>undo.Delete<DailyTask>(item.Id))));}
 async void EditTask(DailyTask item)
 {
  if(!await StartEditor("日常事项"))return;var title=Ui.Field(DrawerContent,"事项名称",item.Title);var date=Ui.Field(DrawerContent,"日期 yyyy-MM-dd",item.Date.ToString("yyyy-MM-dd"));var time=Ui.Field(DrawerContent,"时间 HH:mm（可留空）",item.Time?.ToString("HH:mm")??"");var priority=Ui.Choice(DrawerContent,"优先级",["普通","重要"],item.Priority=="important"?1:0);
  Observe("task-"+item.Id,()=>{var draft=item with{Title=title.Text,Date=Ui.Date(date.Text),Time=Ui.Time(time.Text),Priority=priority.SelectedIndex==1?"important":"normal"};Validation.Task(draft);return()=>new TaskService(store).Save(draft);},title,date,time,priority);FinishEditor();
 }
}
