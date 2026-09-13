using PersonalLife.Core;
namespace PersonalLife.App;
public partial class MainWindow
{
 void RenderJournal()
 {
  var data=store.ReadAll();var item=data.Journals.SingleOrDefault(x=>x.Date==SelectedDate)??new JournalEntry{Date=SelectedDate};var c=Ui.Card(PageContent,"给今天留几句话");var moods=new[]{"未选择","愉快","平静","疲惫","低落","充实"};var mood=Ui.Choice(c,"今日心情",moods,System.Math.Max(0,System.Array.IndexOf(moods,item.Mood)));var summary=Ui.Field(c,"今日总结",item.Summary,true);var gains=Ui.Field(c,"收获",item.Gains,true);var improvements=Ui.Field(c,"待改进",item.Improvements,true);Observe("journal-"+SelectedDate,()=>{var d=item with{Mood=mood.SelectedIndex==0?null:moods[mood.SelectedIndex],Summary=summary.Text,Gains=gains.Text,Improvements=improvements.Text};return()=>new JournalService(store).Save(d);},mood,summary,gains,improvements);c.Children.Add(Ui.Button("删除当天日志",()=>Run(()=>{var saved=store.ReadAll().Journals.SingleOrDefault(x=>x.Date==SelectedDate);return saved==null?new WriteReceipt(0):undo.Delete<JournalEntry>(saved.Id);})));var history=Ui.Card(PageContent,"历史日志");foreach(var old in data.Journals.OrderByDescending(x=>x.Date))history.Children.Add(Ui.Button($"{old.Date:yyyy-MM-dd} · {old.Mood??"未选心情"}",async()=>await Navigate(PageId.Journal,old.Date)));
 }
}
