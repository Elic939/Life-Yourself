namespace PersonalLife.Tests;

[TestClass]
public sealed class Test1
{
    [TestMethod]
    public void TestMethod1()
    {
        var empty = new PersonalLife.Core.AppSnapshot();
        Assert.IsEmpty(empty.Entities());
        Assert.AreEqual("monday", empty.Settings.WeekStartsOn);
    }
}
