using System.Diagnostics;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using PersonalLife.Core;
namespace PersonalLife.App;
public partial class MainWindow
{
 void RenderSettings()
 {
  var data=Ui.Card(PageContent,"本机数据文件");data.Children.Add(Ui.Text(databasePath));data.Children.Add(Ui.Text("记录自动保存到当前电脑。更新或移动程序不会移动数据文件。",12));data.Children.Add(Ui.Button("打开数据文件夹",()=>{try{Process.Start(new ProcessStartInfo(Path.GetDirectoryName(databasePath)!){UseShellExecute=true});}catch(Exception e){MessageBox.Show(this,e.Message,"无法打开文件夹");}}));
  var backup=Ui.Card(PageContent,"备份与恢复");backup.Children.Add(Ui.Text("导出全部日期和全部模块。恢复将整份替换当前记录与设置。"));var row=Ui.Row(backup);row.Children.Add(Ui.Button("导出完整备份",async()=>await ExportBackup()));row.Children.Add(Ui.Button("选择备份并预览",PreviewBackup));
  var preference=Ui.Card(PageContent,"基础偏好");var week=Ui.Choice(preference,"一周从哪天开始",["星期一","星期日"],store.ReadAll().Settings.WeekStartsOn=="sunday"?1:0);Observe("settings",()=>{var setting=new UserSettings{WeekStartsOn=week.SelectedIndex==1?"sunday":"monday"};return()=>store.Upsert(setting);},week);
  var clear=Ui.Card(PageContent,"清空全部数据");clear.Children.Add(Ui.Text("清空所有模块记录与偏好，不删除你已导出的备份文件。"));clear.Children.Add(Ui.Button("清空全部数据",async()=>{if(!await saves.FlushAsync())return;if(MessageBox.Show(this,"将删除全部计划、记录、日志和偏好。已导出备份不受影响。确认清空？","清空全部数据",MessageBoxButton.YesNo,MessageBoxImage.Warning)!=MessageBoxResult.Yes)return;await Exclusive(()=>{var r=new BackupService(store).Clear();undo.Clear();return r;});}));
 }
 async Task<bool> ExportBackup()
 {
  if(!await saves.FlushAsync())return false;var dialog=new SaveFileDialog{Filter="生活备份 (*.json)|*.json",FileName=$"life-app-backup-{DateTime.Now:yyyy-MM-dd-HHmmss}.json"};if(dialog.ShowDialog(this)!=true)return false;
  IsEnabled=false;try{var path=await Task.Run(()=>new BackupService(store).Export(dialog.FileName));SaveLabel.Text="备份已导出："+path;return true;}catch(Exception e){MessageBox.Show(this,e.Message,"备份导出失败");return false;}finally{IsEnabled=true;}
 }
 async void PreviewBackup()
 {
  if(!await saves.FlushAsync())return;var dialog=new OpenFileDialog{Filter="生活备份 (*.json)|*.json"};if(dialog.ShowDialog(this)!=true)return;
  BackupPreview preview;try{preview=await Task.Run(()=>new BackupService(store).Preview(dialog.FileName));}catch(Exception e){MessageBox.Show(this,"文件未通过校验，当前数据未改变。\n"+e.Message,"无法恢复");return;}
  if(!await StartEditor("恢复备份预览"))return;var s=preview.Data;DrawerContent.Children.Add(Ui.Text($"导出时间：{preview.ExportedAt:yyyy-MM-dd HH:mm:ss}"));DrawerContent.Children.Add(Ui.Text($"事项 {s.Tasks.Count}\n训练 {s.Workouts.Count} / 动作 {s.Exercises.Count} / 组 {s.ExerciseSets.Count}\n餐食 {s.Meals.Count} / 饮水 {s.WaterEntries.Count}\n娱乐安排 {s.GamePlans.Count} / 实际记录 {s.GameSessions.Count}\n日志 {s.Journals.Count}\n包含基础偏好"));DrawerContent.Children.Add(Ui.Text("恢复会替换当前所有数据，不进行合并。"));DrawerContent.Children.Add(Ui.Button("先导出当前数据",async()=>await ExportBackup()));DrawerContent.Children.Add(Ui.Button("确认替换并恢复",async()=>{if(MessageBox.Show(this,"确认用此备份替换全部当前数据？","恢复确认",MessageBoxButton.YesNo,MessageBoxImage.Warning)!=MessageBoxResult.Yes)return;await Exclusive(()=>{var r=new BackupService(store).Restore(preview);undo.Clear();return r;});}));DrawerContent.Children.Add(Ui.Button("取消",()=>CloseEditor()));
 }
 async Task Exclusive(Func<WriteReceipt> operation){if(!await saves.FlushAsync())return;IsEnabled=false;try{await Task.Run(operation);saves.Discard();CloseEditor();Render();}catch(Exception e){MessageBox.Show(this,"操作失败，原有完整数据已保留。\n"+e.Message,"数据操作失败");}finally{IsEnabled=true;}}
}
