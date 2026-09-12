using PersonalLife.Storage;
namespace PersonalLife.Tests;
public sealed class TestStore : IDisposable
{
 public string Folder {get;}=Path.Combine(Path.GetTempPath(),"PersonalLifeTests",Guid.NewGuid().ToString("N"));
 public string PathName=>Path.Combine(Folder,"life.db");
 public SqliteDataStore Store {get;}
 public TestStore(){Directory.CreateDirectory(Folder);Store=new(PathName);}
 public void Dispose(){Store.Dispose();Directory.Delete(Folder,true);}
}
