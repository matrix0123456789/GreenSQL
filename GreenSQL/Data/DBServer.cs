using GreenSQL.Data.Storage;
using GreenSQL.Execution;

namespace GreenSQL.Data;

public class DBServer
{
    private readonly IStorage storage;
    private Dictionary<string, Database> databases = new();
    public List<Database> Databases => databases.Values.ToList();
    public DBServer(IStorage storage, List<Database> list)
    {
        this.storage=   storage;
        databases = list.ToDictionary(x => x.Name);
    }
    public DBExecutionContext CreateNewContext()
    {
        return new DBExecutionContext(this);
    }

    public void CreateDatabase(string databaseName)
    {
        if (databases.ContainsKey(databaseName))
        {
            throw new Exception("Database already exists");
        }
        var database = new Database(databaseName);
        databases.Add(databaseName, database);
        storage.CreateDatabase(database);
    }
}