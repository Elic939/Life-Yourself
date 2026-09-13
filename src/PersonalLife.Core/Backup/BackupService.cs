using System.Text.Json;
using System.Text.Json.Serialization;
namespace PersonalLife.Core;
public sealed record BackupPreview(DateTimeOffset ExportedAt,AppSnapshot Data)
{
 public string Describe()=>$"导出时间：{ExportedAt:yyyy-MM-dd HH:mm:ss}\n事项 {Data.Tasks.Count}；训练 {Data.Workouts.Count}；动作 {Data.Exercises.Count}；组 {Data.ExerciseSets.Count}\n餐食 {Data.Meals.Count}；饮水 {Data.WaterEntries.Count}\n娱乐计划 {Data.GamePlans.Count}；游玩记录 {Data.GameSessions.Count}；日志 {Data.Journals.Count}\n包含基础偏好";
}
public sealed record BackupEnvelope(int FormatVersion,DateTimeOffset ExportedAt,AppSnapshot Data);
public sealed class BackupService(IDataStore store)
{
 static readonly JsonSerializerOptions Options=new(){PropertyNamingPolicy=JsonNamingPolicy.CamelCase,WriteIndented=true,UnmappedMemberHandling=JsonUnmappedMemberHandling.Disallow};
 public string Export(string path)
 {
   var full=Path.GetFullPath(path);var temp=full+"."+Guid.NewGuid().ToString("N")+".tmp";
   try{var envelope=new BackupEnvelope(1,DateTimeOffset.Now,store.ReadAll());using(var stream=new FileStream(temp,FileMode.CreateNew,FileAccess.Write,FileShare.None)){JsonSerializer.Serialize(stream,envelope,Options);stream.Flush(true);}if(File.Exists(full))File.Replace(temp,full,null);else File.Move(temp,full);return full;}
   finally{if(File.Exists(temp))File.Delete(temp);}
 }
 public BackupPreview Preview(string path)
 =>ReadPreview(path);
 public static BackupPreview ReadPreview(string path)
 {
   using var stream=File.OpenRead(path);using var doc=JsonDocument.Parse(stream);var root=doc.RootElement;SnapshotValidator.RequireShape(root,typeof(BackupEnvelope));Validation.Require(root.GetProperty("formatVersion").GetInt32()==1,"备份版本不受支持");
   var data=root.GetProperty("data");SnapshotValidator.RequireShape(data,typeof(AppSnapshot));
   foreach(var p in typeof(AppSnapshot).GetProperties()){var element=data.GetProperty(JsonNamingPolicy.CamelCase.ConvertName(p.Name));if(p.PropertyType.IsGenericType){Validation.Require(element.ValueKind==JsonValueKind.Array,"模块必须是数组");foreach(var item in element.EnumerateArray())SnapshotValidator.RequireShape(item,p.PropertyType.GenericTypeArguments[0]);}else SnapshotValidator.RequireShape(element,p.PropertyType);}
   var envelope=root.Deserialize<BackupEnvelope>(Options)??throw new InvalidDataException("备份为空");SnapshotValidator.Validate(envelope.Data);return new(envelope.ExportedAt,envelope.Data);
 }
 public WriteReceipt Restore(BackupPreview preview){SnapshotValidator.Validate(preview.Data);return store.ReplaceAll(preview.Data);}
 public WriteReceipt Clear()=>store.ReplaceAll(new());
}
