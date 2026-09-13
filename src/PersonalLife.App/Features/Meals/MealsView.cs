using System.Windows.Controls;
using PersonalLife.Core;
namespace PersonalLife.App;
public partial class MainWindow
{
 void RenderMeals()
 {
  var data=store.ReadAll();foreach(var slot in new[]{("breakfast","早餐"),("lunch","午餐"),("dinner","晚餐"),("snack","加餐")})
  {
   var item=data.Meals.SingleOrDefault(x=>x.Date==SelectedDate&&x.Slot==slot.Item1)??new Meal{Date=SelectedDate,Slot=slot.Item1};var c=Ui.Card(PageContent,slot.Item2);var plan=Ui.Field(c,"计划吃什么",item.PlannedText);var actual=Ui.Field(c,"实际吃了什么",item.ActualText);Observe("meal-"+item.Id,()=>{var draft=item with{PlannedText=plan.Text,ActualText=actual.Text};return()=>new MealService(store).Save(draft);},plan,actual);var row=Ui.Row(c);row.Children.Add(Ui.Button("照计划记录",()=>{if(!string.IsNullOrWhiteSpace(plan.Text))actual.Text=plan.Text;else SaveLabel.Text="请先填写餐食计划";}));row.Children.Add(Ui.Button("清除餐次记录",()=>Run(()=>undo.Delete<Meal>(item.Id))));
  }
  var water=Ui.Card(PageContent,$"饮水 · {data.WaterEntries.Where(x=>x.Date==SelectedDate).Sum(x=>(long)x.Milliliters)} ml");var buttons=Ui.Row(water);buttons.Children.Add(Ui.Button("＋ 250 ml",()=>Run(()=>new MealService(store).Save(new WaterEntry{Date=SelectedDate,Milliliters=250}))));buttons.Children.Add(Ui.Button("自定义饮水",()=>EditWater(new WaterEntry{Date=SelectedDate})));foreach(var item in data.WaterEntries.Where(x=>x.Date==SelectedDate)){var row=Ui.Row(water);row.Children.Add(Ui.Text($"{item.Milliliters} ml"));row.Children.Add(Ui.Button("修改",()=>EditWater(item)));row.Children.Add(Ui.Button("删除",()=>Run(()=>undo.Delete<WaterEntry>(item.Id))));}
 }
 async void EditWater(WaterEntry item){if(!await StartEditor("饮水记录"))return;var ml=Ui.Field(DrawerContent,"毫升 ml",item.Milliliters==0?"":item.Milliliters.ToString());var date=Ui.Field(DrawerContent,"日期",item.Date.ToString("yyyy-MM-dd"));Observe("water-"+item.Id,()=>{var d=item with{Date=Ui.Date(date.Text),Milliliters=Ui.Number(ml.Text,"饮水量")};Validation.Require(d.Milliliters>0,"饮水量必须大于0");return()=>new MealService(store).Save(d);},ml,date);FinishEditor();}
}
