namespace GreenSQL.Data.Storage;

public class MultiFileStorage : IStorage, IDisposable
{
    public DirectoryInfo Directory { get; set; }
    public MultiFileStorage(string path)
    {
        this.Directory = new DirectoryInfo(path);
    }


    public void Load()
    {
        if(!Directory.Exists)
        {
            Directory.Create();
        }
var databases = new List<Database>();
        foreach (var subDirectory in Directory.GetDirectories())
        {
            if(subDirectory.Name.StartsWith("db_"))
            {
                var database = new Database(subDirectory.Name.Substring(3));
                databases.Add(database);
            }
        }
        
        this.Server = new DBServer(this, databases);
    }
    public DBServer Server { get; set; }
    public void CreateDatabase(Database database)
    {
        var path = Path.Combine(Directory.FullName, "db_"+database.Name);
        new DirectoryInfo(path).Create();
    }

    public void Dispose()
    {
        // TODO release managed resources here
    }
}