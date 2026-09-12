namespace PersonalLife.Core;
public enum PageId { Home, Tasks, Fitness, Meals, Gaming, Journal, Settings }
public sealed class NavigationState
{
    public PageId CurrentPage { get; private set; }
    public DateOnly SelectedDate { get; private set; } = DateOnly.FromDateTime(DateTime.Now);
    public void Navigate(PageId page, DateOnly? date = null) { CurrentPage = page; if(date.HasValue) SelectedDate = date.Value; }
}
