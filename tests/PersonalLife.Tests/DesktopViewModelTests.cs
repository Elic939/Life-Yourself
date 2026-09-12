using PersonalLife.Core;
namespace PersonalLife.Tests;
[TestClass]
public class DesktopViewModelTests
{
    [TestMethod] public void NavigationPreservesHistoricalDate()
    {
        var n = new NavigationState(); var date = new DateOnly(2026, 9, 1);
        n.Navigate(PageId.Fitness, date); n.Navigate(PageId.Home);
        Assert.AreEqual(date, n.SelectedDate); Assert.AreEqual(PageId.Home, n.CurrentPage);
    }
    [TestMethod] public void AllSevenPagesCanBeSelected()
    {
        var n = new NavigationState();
        foreach(var page in Enum.GetValues<PageId>()) { n.Navigate(page); Assert.AreEqual(page,n.CurrentPage); }
        Assert.HasCount(7, Enum.GetValues<PageId>());
    }
}
