namespace PersonalLife.Core;
public enum SaveState { Saved, Saving, Invalid, Failed }
public sealed class SaveCoordinator
{
 readonly object gate=new();readonly SemaphoreSlim writer=new(1,1);
 readonly Dictionary<string,(long Version,Func<WriteReceipt> Write)> pending=new();
 readonly Dictionary<string,string> invalid=new();long version;long timer;
 public SaveState State {get;private set;}=SaveState.Saved;
 public string? Error {get;private set;}
 public event Action? Changed;
 public void Queue(string key,Func<WriteReceipt> write){long ticket;lock(gate){pending[key]=(++version,write);invalid.Remove(key);State=SaveState.Saving;Error=null;ticket=++timer;}Changed?.Invoke();_ = Debounce(ticket);}
 async Task Debounce(long ticket){await Task.Delay(500);lock(gate){if(ticket!=timer)return;}await FlushAsync();}
 public void SetInvalid(string key,string message){lock(gate){pending.Remove(key);invalid[key]=message;State=SaveState.Invalid;Error=message;++timer;}Changed?.Invoke();}
 public void Discard(){if(writer.CurrentCount==0)throw new InvalidOperationException("请等待正在保存的写入完成");lock(gate){pending.Clear();invalid.Clear();State=SaveState.Saved;Error=null;++timer;}Changed?.Invoke();}
 public async Task DiscardAsync(){lock(gate){pending.Clear();invalid.Clear();State=SaveState.Saving;++timer;}Changed?.Invoke();await writer.WaitAsync();try{lock(gate){State=invalid.Count>0?SaveState.Invalid:pending.Count>0?SaveState.Saving:SaveState.Saved;Error=invalid.Values.FirstOrDefault();}}finally{writer.Release();}Changed?.Invoke();}
 public async Task<bool> FlushAsync()
 {
   await writer.WaitAsync();
   try {
     while(true){
       KeyValuePair<string,(long Version,Func<WriteReceipt> Write)> item;
       lock(gate){
         if(pending.Count==0){State=invalid.Count>0?SaveState.Invalid:SaveState.Saved;Error=invalid.Values.FirstOrDefault();break;}
         item=pending.First();State=SaveState.Saving;
       }
       try{await Task.Run(item.Value.Write);}
       catch(Exception e){lock(gate){State=SaveState.Failed;Error=e.Message;}Changed?.Invoke();return false;}
       lock(gate){if(pending.TryGetValue(item.Key,out var current)&&current.Version==item.Value.Version)pending.Remove(item.Key);}
     }
     Changed?.Invoke();return State==SaveState.Saved;
   }finally{writer.Release();}
 }
}
