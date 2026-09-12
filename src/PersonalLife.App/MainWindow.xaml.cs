using System.Windows;
using System.Windows.Controls;
using PersonalLife.Core;
namespace PersonalLife.App;
public partial class MainWindow : Window
{
    readonly NavigationState navigation = new();
    readonly string[] titles = ["首页总览","今日计划","健身计划","饮食计划","游戏娱乐","总结日志","数据与设置"];
    public MainWindow() { InitializeComponent(); foreach(var page in Enum.GetValues<PageId>()) { var b=new Button {Content=titles[(int)page],HorizontalContentAlignment=HorizontalAlignment.Left}; b.Click+=(_,_)=>Show(page); Navigation.Children.Add(b); } DateInput.SelectedDate=DateTime.Today; Show(PageId.Home); }
    void Show(PageId page) { navigation.Navigate(page); PageTitle.Text=titles[(int)page]; DatePanel.Visibility=page==PageId.Settings?Visibility.Collapsed:Visibility.Visible; PageContent.Children.Clear(); PageContent.Children.Add(new TextBlock {Text="尚无记录，请添加今天的内容。",Margin=new Thickness(0,20,0,0)}); }
    void DateChanged(object? s,SelectionChangedEventArgs e) { if(DateInput.SelectedDate is DateTime d) { navigation.Navigate(navigation.CurrentPage,DateOnly.FromDateTime(d)); if(PageContent!=null)Show(navigation.CurrentPage); } }
    void PreviousDay(object s,RoutedEventArgs e)=>DateInput.SelectedDate=(DateInput.SelectedDate??DateTime.Today).AddDays(-1);
    void NextDay(object s,RoutedEventArgs e)=>DateInput.SelectedDate=(DateInput.SelectedDate??DateTime.Today).AddDays(1);
    void Today(object s,RoutedEventArgs e)=>DateInput.SelectedDate=DateTime.Today;
}
