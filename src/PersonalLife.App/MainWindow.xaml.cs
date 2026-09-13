using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using PersonalLife.Core;
using PersonalLife.Storage;
namespace PersonalLife.App;
public partial class MainWindow : Window
{
 readonly NavigationState navigation=new();
 readonly string[] titles=["首页总览","今日计划","健身计划","饮食计划","游戏娱乐","总结日志","数据与设置"];
 readonly SaveCoordinator saves=new();
 readonly IDataStore store;
 readonly UndoService undo;
 readonly SingleInstance instance;
 readonly string databasePath;
 bool changingDate,closingAllowed,closingPending;
 DateOnly SelectedDate=>navigation.SelectedDate;
 public MainWindow() : this(null) {}
 public MainWindow(string? dataDirectory)
 {
   InitializeComponent();
   var args=Environment.GetCommandLineArgs();var index=Array.IndexOf(args,"--data-dir");
   databasePath=dataDirectory!=null?Path.Combine(Path.GetFullPath(dataDirectory),"life.db"):index>=0&&index+1<args.Length?Path.Combine(Path.GetFullPath(args[index+1]),"life.db"):DataPaths.DatabasePath;
   instance=new(databasePath);if(!instance.Owns){Application.Current.Shutdown();store=null!;undo=null!;return;}
   try{store=new SqliteDataStore(databasePath);}catch(Exception error){
    if(MessageBox.Show("无法打开数据文件，原文件已保留。\n"+databasePath+"\n"+error.Message+"\n是否选择备份恢复？","数据文件异常",MessageBoxButton.YesNo,MessageBoxImage.Warning)!=MessageBoxResult.Yes){instance.Dispose();throw new OperationCanceledException();}
    var dialog=new Microsoft.Win32.OpenFileDialog{Filter="生活备份 (*.json)|*.json"};if(dialog.ShowDialog()!=true){instance.Dispose();throw new OperationCanceledException();}
    try{var preserved=RecoveryService.ConfirmAndRestore(databasePath,dialog.FileName,p=>MessageBox.Show(p.Describe()+"\n确认用此备份恢复全部记录？原文件会另存保留。","备份预览与恢复确认",MessageBoxButton.YesNo,MessageBoxImage.Warning)==MessageBoxResult.Yes);if(preserved==null)throw new OperationCanceledException();store=new SqliteDataStore(databasePath);MessageBox.Show("恢复完成。原文件另存于：\n"+preserved);}catch{instance.Dispose();throw;}
   }
   undo=new(store);UndoButton.Click+=(_,_)=>Run(undo.Undo);
   var undoTimer=new System.Windows.Threading.DispatcherTimer{Interval=TimeSpan.FromSeconds(1)};undoTimer.Tick+=(_,_)=>UndoButton.Visibility=undo.CanUndo?Visibility.Visible:Visibility.Collapsed;undoTimer.Start();
   foreach(var page in Enum.GetValues<PageId>()){var b=new Button{Content=titles[(int)page],HorizontalContentAlignment=HorizontalAlignment.Left};b.Click+=async(_,_)=>await Navigate(page);Navigation.Children.Add(b);}
   saves.Changed+=()=>Dispatcher.InvokeAsync(UpdateSaveState);
   RetryButton.Click+=async(_,_)=>{if(await saves.FlushAsync()&&Drawer.Visibility!=Visibility.Visible)Render();UpdateSaveState();};
   Closing+=OnClosing;
   changingDate=true;DateInput.SelectedDate=DateTime.Today;changingDate=false;Render();
 }
 void UpdateSaveState(){SaveLabel.Text=saves.State switch{SaveState.Saving=>"保存中…",SaveState.Invalid=>"未保存："+saves.Error,SaveState.Failed=>"保存失败："+saves.Error,_=>"已保存"};RetryButton.Visibility=saves.State==SaveState.Failed?Visibility.Visible:Visibility.Collapsed;}
 async Task Navigate(PageId page,DateOnly? date=null){if(!await saves.FlushAsync()){UpdateSaveState();return;}CloseEditor();navigation.Navigate(page,date);changingDate=true;DateInput.SelectedDate=SelectedDate.ToDateTime(TimeOnly.MinValue);changingDate=false;Render();ContentScroll.ScrollToTop();}
 void Render(){for(var i=0;i<Navigation.Children.Count;i++){var button=(Button)Navigation.Children[i];button.FontWeight=i==(int)navigation.CurrentPage?FontWeights.SemiBold:FontWeights.Normal;button.Background=i==(int)navigation.CurrentPage?new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(213,232,220)):System.Windows.Media.Brushes.Transparent;}PageTitle.Text=titles[(int)navigation.CurrentPage];DatePanel.Visibility=navigation.CurrentPage==PageId.Settings?Visibility.Collapsed:Visibility.Visible;PageContent.Children.Clear();if(navigation.CurrentPage==PageId.Home)RenderHome();else if(navigation.CurrentPage==PageId.Tasks){RenderTasks();AddModuleLinks();}else if(navigation.CurrentPage==PageId.Fitness)RenderFitness();else if(navigation.CurrentPage==PageId.Meals)RenderMeals();else if(navigation.CurrentPage==PageId.Gaming)RenderGaming();else if(navigation.CurrentPage==PageId.Journal){AddJournalSummary();RenderJournal();}else if(navigation.CurrentPage==PageId.Settings)RenderSettings();UpdateSaveState();UndoButton.Visibility=undo.CanUndo?Visibility.Visible:Visibility.Collapsed;}
 async void DateChanged(object? s,SelectionChangedEventArgs e){if(changingDate||store==null)return;if(DateInput.SelectedDate is DateTime d)await Navigate(navigation.CurrentPage,DateOnly.FromDateTime(d));changingDate=true;DateInput.SelectedDate=SelectedDate.ToDateTime(TimeOnly.MinValue);changingDate=false;}
 void PreviousDay(object s,RoutedEventArgs e)=>DateInput.SelectedDate=(DateInput.SelectedDate??DateTime.Today).AddDays(-1);
 void NextDay(object s,RoutedEventArgs e)=>DateInput.SelectedDate=(DateInput.SelectedDate??DateTime.Today).AddDays(1);
 void Today(object s,RoutedEventArgs e)=>DateInput.SelectedDate=DateTime.Today;
 async void OnClosing(object? sender,CancelEventArgs e){if(closingAllowed)return;e.Cancel=true;if(closingPending)return;closingPending=true;await System.Windows.Threading.Dispatcher.Yield(System.Windows.Threading.DispatcherPriority.Background);if(await saves.FlushAsync()){closingAllowed=true;store.Dispose();instance.Dispose();Close();}else if(MessageBox.Show(this,"有尚未保存的修改。是否明确放弃并退出？选择否可继续编辑或重试。","尚未保存",MessageBoxButton.YesNo)==MessageBoxResult.Yes){saves.Discard();closingAllowed=true;store.Dispose();instance.Dispose();Close();}closingPending=false;}
 async void Run(Func<WriteReceipt> action){if(!await saves.FlushAsync()){UpdateSaveState();return;}saves.Queue("command",action);if(await saves.FlushAsync())Render();else UpdateSaveState();}
 async Task<bool> StartEditor(string title){if(!await saves.FlushAsync()){UpdateSaveState();return false;}DrawerContent.Children.Clear();DrawerContent.Children.Add(Ui.Text(title,22));Drawer.Visibility=Visibility.Visible;PageContent.IsEnabled=false;DatePanel.IsEnabled=false;return true;}
 void CloseEditor(){Drawer.Visibility=Visibility.Collapsed;PageContent.IsEnabled=true;DatePanel.IsEnabled=true;}
 void FinishEditor(){DrawerContent.Children.Add(Ui.Button("完成并关闭",async()=>{if(await saves.FlushAsync()){CloseEditor();Render();}}));DrawerContent.Children.Add(Ui.Button("放弃尚未保存的修改",async()=>{IsEnabled=false;try{await saves.DiscardAsync();CloseEditor();Render();}finally{IsEnabled=true;}}));}
 void Observe(string key,Func<Func<WriteReceipt>> capture,params Control[] inputs)
 {
   void Changed(){try{saves.Queue(key,capture());}catch(Exception e){saves.SetInvalid(key,e.Message);}UpdateSaveState();}
   foreach(var input in inputs){if(input is TextBox t)t.TextChanged+=(_,_)=>Changed();else if(input is ComboBox c)c.SelectionChanged+=(_,_)=>Changed();else if(input is CheckBox check)check.Click+=(_,_)=>Changed();}
 }
}
