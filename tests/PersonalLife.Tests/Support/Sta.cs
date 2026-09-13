using System.Windows.Threading;
namespace PersonalLife.Tests;
internal static class Sta
{
 public static void Run(Func<Task> action){Exception? error=null;var thread=new Thread(()=>{SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext());Dispatcher.CurrentDispatcher.InvokeAsync(async()=>{try{await action();}catch(Exception e){error=e;}finally{Dispatcher.ExitAllFrames();}});Dispatcher.Run();});thread.SetApartmentState(ApartmentState.STA);thread.Start();if(!thread.Join(TimeSpan.FromSeconds(30)))throw new TimeoutException("桌面测试超时");if(error!=null)System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(error).Throw();}
}
