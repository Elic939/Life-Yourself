using System.Collections;
namespace PersonalLife.Core;
public sealed class UndoService(IDataStore store,Func<DateTimeOffset>? clock=null)
{
 AppSnapshot? deleted;DateTimeOffset until;
 DateTimeOffset Now=>(clock??(()=>DateTimeOffset.Now))();
 public bool CanUndo=>deleted!=null&&Now<until;
 public WriteReceipt Delete<T>(Guid id) where T:class
 {
  var before=store.ReadAll();var result=store.Delete<T>(id);var remaining=store.ReadAll().Entities().Select(x=>x.Id).ToHashSet();
  deleted=Select(before,x=>!remaining.Contains(x.Id));until=Now.AddSeconds(10);return result;
 }
 static AppSnapshot Select(AppSnapshot s,Func<IEntity,bool> filter)=>new(){Tasks=s.Tasks.Where(x=>filter(x)).ToList(),Workouts=s.Workouts.Where(x=>filter(x)).ToList(),Exercises=s.Exercises.Where(x=>filter(x)).ToList(),ExerciseSets=s.ExerciseSets.Where(x=>filter(x)).ToList(),Meals=s.Meals.Where(x=>filter(x)).ToList(),WaterEntries=s.WaterEntries.Where(x=>filter(x)).ToList(),GamePlans=s.GamePlans.Where(x=>filter(x)).ToList(),GameSessions=s.GameSessions.Where(x=>filter(x)).ToList(),Journals=s.Journals.Where(x=>filter(x)).ToList(),Settings=s.Settings};
 public WriteReceipt Undo()
 {
   if(!CanUndo)throw new InvalidOperationException("撤销已过期");var current=store.ReadAll();var ids=current.Entities().Select(x=>x.Id).ToHashSet();if(deleted!.Entities().Any(x=>ids.Contains(x.Id)))throw new InvalidOperationException("记录已存在，无法覆盖撤销");
   foreach(var property in typeof(AppSnapshot).GetProperties())if(property.GetValue(current) is IList target&&property.GetValue(deleted) is IList source)foreach(var item in source)target.Add(item);
   var receipt=store.ReplaceAll(current);Clear();return receipt;
 }
 public void Clear()=>deleted=null;
}

