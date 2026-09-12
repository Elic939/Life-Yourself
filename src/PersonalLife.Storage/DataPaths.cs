namespace PersonalLife.Storage;
public static class DataPaths
{
 public static string DirectoryPath=>Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"PersonalLife");
 public static string DatabasePath=>Path.Combine(DirectoryPath,"life.db");
}
