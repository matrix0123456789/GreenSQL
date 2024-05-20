namespace GreenSQL.Data.Storage;

public class MultiFileStorage : IStorage, IDisposable
{
    public DirectoryInfo Directory { get; set; }
    private Dictionary<string, Stream> openFiles = new();

    public MultiFileStorage(string path)
    {
        this.Directory = new DirectoryInfo(path);
    }


    public void Load()
    {
        if (!Directory.Exists)
        {
            Directory.Create();
        }

        var databases = new List<Database>();
        foreach (var subDirectory in Directory.GetDirectories())
        {
            if (subDirectory.Name.StartsWith("db_"))
            {
                var tables = new List<DBTable>();
                foreach(var file in subDirectory.GetFiles())
                {
                    if (file.Extension == ".dbtable")
                    {
                        var table= new DBTable(file.Name.Substring(0, file.Name.Length - 8));
                        tables.Add(table);
                    }
                }
                var database = new Database(this, subDirectory.Name.Substring(3), tables);
                databases.Add(database);
            }
        }

        this.Server = new DBServer(this, databases);
    }

    public DBServer Server { get; set; }

    public void CreateDatabase(Database database)
    {
        var path = Path.Combine(Directory.FullName, "db_" + database.Name);
        new DirectoryInfo(path).Create();
    }

    public void CreateTable(Database database, DBTable table)
    {
        var path = Path.Combine(Directory.FullName, "db_" + database.Name + "/" + table.Name + ".dbtable");
        var stream = new FileInfo(path).Create();
        openFiles.Add(path, stream);
    }

    public void Dispose()
    {
        foreach (var stream in openFiles.Values)
        {
            stream.Close();
        }
    }
}