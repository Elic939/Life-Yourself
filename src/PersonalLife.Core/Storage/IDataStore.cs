namespace PersonalLife.Core;
public interface IDataStore : IDisposable
{
 AppSnapshot ReadAll();
 WriteReceipt Upsert<T>(T entity) where T:class;
 WriteReceipt Delete<T>(Guid id) where T:class;
 WriteReceipt ReplaceAll(AppSnapshot snapshot);
 event Action? Changed;
}
