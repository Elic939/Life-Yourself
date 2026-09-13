namespace PersonalLife.Core;
public sealed class MealService(IDataStore store)
{
 public WriteReceipt Save(Meal x){Validation.Require(x.Slot is "breakfast" or "lunch" or "dinner" or "snack","请选择餐次");return store.Upsert(x with{IsRecorded=!string.IsNullOrWhiteSpace(x.ActualText)});}
 public WriteReceipt Save(WaterEntry x){Validation.Require(x.Milliliters>0,"饮水量必须大于0 ml");return store.Upsert(x);}
 public WriteReceipt CopyPlanToActual(Guid id){var x=store.ReadAll().Meals.Single(a=>a.Id==id);Validation.Require(!string.IsNullOrWhiteSpace(x.PlannedText),"请先填写餐食计划");return Save(x with{ActualText=x.PlannedText});}
 public List<Meal> GetForDate(DateOnly d)=>store.ReadAll().Meals.Where(x=>x.Date==d).ToList();
}
