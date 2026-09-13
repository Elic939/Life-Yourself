namespace PersonalLife.Core;
public sealed class JournalService(IDataStore store)
{
 public WriteReceipt Save(JournalEntry x){var existing=GetForDate(x.Date).SingleOrDefault();if(existing==null&&string.IsNullOrWhiteSpace(x.Summary+x.Gains+x.Improvements+x.Mood))return new(0);return store.Upsert(x with{Id=existing?.Id??x.Id,UpdatedAt=DateTimeOffset.Now});}
 public List<JournalEntry> GetForDate(DateOnly date)=>store.ReadAll().Journals.Where(x=>x.Date==date).ToList();
}
