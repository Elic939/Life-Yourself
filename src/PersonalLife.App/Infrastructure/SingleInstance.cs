using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
namespace PersonalLife.App;
internal sealed class SingleInstance : IDisposable
{
 readonly Mutex mutex;
 public bool Owns {get;}
 public SingleInstance(string path){mutex=new Mutex(true,"Local\\PersonalLife-"+Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(path))),out var owns);Owns=owns;if(!owns){var current=Process.GetCurrentProcess();foreach(var p in Process.GetProcessesByName(current.ProcessName))if(p.Id!=current.Id&&p.MainWindowHandle!=IntPtr.Zero){ShowWindow(p.MainWindowHandle,9);SetForegroundWindow(p.MainWindowHandle);}}}
 [DllImport("user32.dll")]static extern bool SetForegroundWindow(IntPtr h);
 [DllImport("user32.dll")]static extern bool ShowWindow(IntPtr h,int n);
 public void Dispose(){if(Owns)mutex.ReleaseMutex();mutex.Dispose();}
}
