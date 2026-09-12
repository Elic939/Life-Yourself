namespace PersonalLife.Core;
public sealed class TaskService(IDataStore store)
{
 public List<DailyTask> GetForDate(DateOnly date)=>store.ReadAll().Tasks.Where(x=>x.Date==date).ToList();
 public WriteReceipt Save(DailyTask entity){Validation.Task(entity);return store.Upsert(entity with {Title=entity.Title.Trim()});}
 public WriteReceipt Delete(Guid id)=>store.Delete<DailyTask>(id);
}
