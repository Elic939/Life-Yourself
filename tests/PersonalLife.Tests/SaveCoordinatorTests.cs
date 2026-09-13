using PersonalLife.Core;
namespace PersonalLife.Tests;
[TestClass]
public class SaveCoordinatorTests
{
 [TestMethod] public async Task DiscardWaitsForInFlightWrite(){var s=new SaveCoordinator();using var started=new ManualResetEventSlim();using var release=new ManualResetEventSlim();s.Queue("x",()=>{started.Set();release.Wait();return new(1);});var flush=s.FlushAsync();Assert.IsTrue(started.Wait(3000));var discard=s.DiscardAsync();var completedEarly=discard.IsCompleted;release.Set();await flush;await discard;Assert.IsFalse(completedEarly,"放弃必须等待正在提交的写入完成");Assert.AreEqual(SaveState.Saved,s.State);}
 [TestMethod] public async Task LatestEditWins(){using var t=new TestStore();var s=new SaveCoordinator();var a=new DailyTask{Date=new(2026,9,13),Title="一"};s.Queue("task",()=>t.Store.Upsert(a));s.Queue("task",()=>t.Store.Upsert(a with{Title="三"}));Assert.IsTrue(await s.FlushAsync());Assert.AreEqual("三",t.Store.ReadAll().Tasks.Single().Title);Assert.AreEqual(SaveState.Saved,s.State);}
 [TestMethod] public async Task FailureKeepsDraft(){using var t=new TestStore();var s=new SaveCoordinator();var fail=true;var a=new DailyTask{Date=new(2026,9,13),Title="草稿"};s.Queue("task",()=>fail?throw new IOException("磁盘失败"):t.Store.Upsert(a));Assert.IsFalse(await s.FlushAsync());Assert.AreEqual(SaveState.Failed,s.State);Assert.IsEmpty(t.Store.ReadAll().Tasks);fail=false;Assert.IsTrue(await s.FlushAsync());Assert.AreEqual(a,t.Store.ReadAll().Tasks.Single());}
 [TestMethod] public async Task NavigateWaitsForSave(){var s=new SaveCoordinator();var saved=false;s.Queue("x",()=>{Thread.Sleep(30);saved=true;return new(1);});Assert.IsTrue(await s.FlushAsync());Assert.IsTrue(saved);}
 [TestMethod] public async Task InvalidDraftBlocksClose(){var s=new SaveCoordinator();s.SetInvalid("x","标题必填");Assert.IsFalse(await s.FlushAsync());Assert.AreEqual(SaveState.Invalid,s.State);s.Discard();Assert.IsTrue(await s.FlushAsync());}
 [TestMethod] public async Task NewEditDuringWriteIsNotLost(){var s=new SaveCoordinator();using var started=new ManualResetEventSlim();using var release=new ManualResetEventSlim();var value=0;s.Queue("x",()=>{started.Set();release.Wait();value=1;return new(1);});var flush=s.FlushAsync();Assert.IsTrue(started.Wait(3000));s.Queue("x",()=>{value=2;return new(2);});release.Set();Assert.IsTrue(await flush);Assert.AreEqual(2,value);Assert.AreEqual(SaveState.Saved,s.State);}
}
