using System.Configuration;
using System.Data;
using System.Windows;

namespace PersonalLife.App;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
 protected override void OnStartup(StartupEventArgs e){base.OnStartup(e);try{var window=new MainWindow();if(!Dispatcher.HasShutdownStarted)window.Show();}catch(OperationCanceledException){Shutdown();}catch(Exception error){MessageBox.Show("无法启动，原有数据未被覆盖。\n"+error.Message,"我的生活");Shutdown();}}
}
