using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace PersonalLife.App;
internal static class Ui
{
 public static TextBlock Text(string text,double size=14)=>new(){Text=text,FontSize=size,TextWrapping=TextWrapping.Wrap,Margin=new Thickness(0,4,0,8)};
 public static Button Button(string text,Action action){var b=new Button{Content=text};b.Click+=(_,_)=>action();return b;}
 public static StackPanel Card(Panel parent,string title){var p=new StackPanel();p.Children.Add(Text(title,19));parent.Children.Add(new Border{Child=p,Background=Brushes.White,BorderBrush=new SolidColorBrush(Color.FromRgb(218,226,215)),BorderThickness=new Thickness(1),CornerRadius=new CornerRadius(12),Padding=new Thickness(18),Margin=new Thickness(0,0,0,16)});return p;}
 public static WrapPanel Row(Panel parent){var p=new WrapPanel{Margin=new Thickness(0,4,0,4)};parent.Children.Add(p);return p;}
 public static TextBox Field(Panel p,string label,string value="",bool multiline=false){p.Children.Add(Text(label,12));var t=new TextBox{Text=value,AcceptsReturn=multiline,TextWrapping=multiline?TextWrapping.Wrap:TextWrapping.NoWrap,MinHeight=multiline?80:0};System.Windows.Automation.AutomationProperties.SetName(t,label);p.Children.Add(t);return t;}
 public static ComboBox Choice(Panel p,string label,string[] values,int selected=0){p.Children.Add(Text(label,12));var c=new ComboBox{ItemsSource=values,SelectedIndex=selected};System.Windows.Automation.AutomationProperties.SetName(c,label);p.Children.Add(c);return c;}
 public static CheckBox Check(Panel p,string label,bool value,Action<bool> change){var c=new CheckBox{Content=label,IsChecked=value,Margin=new Thickness(0,8,0,8)};c.Click+=(_,_)=>change(c.IsChecked==true);System.Windows.Automation.AutomationProperties.SetName(c,label);p.Children.Add(c);return c;}
 public static DateOnly Date(string s)=>DateOnly.TryParseExact(s,"yyyy-MM-dd",out var d)?d:throw new ArgumentException("日期请填写 yyyy-MM-dd");
 public static TimeOnly? Time(string s)=>string.IsNullOrWhiteSpace(s)?null:TimeOnly.TryParseExact(s,"HH:mm",out var t)?t:throw new ArgumentException("时间请填写 HH:mm");
 public static int Number(string s,string label)=>int.TryParse(s,out var n)?n:throw new ArgumentException(label+"请填写整数");
 public static int? OptionalNumber(string s,string label)=>string.IsNullOrWhiteSpace(s)?null:Number(s,label);
 public static decimal? Weight(string s)=>string.IsNullOrWhiteSpace(s)?null:decimal.TryParse(s,out var n)?n:throw new ArgumentException("重量请填写数字");
}
