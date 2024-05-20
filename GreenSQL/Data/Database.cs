using GreenSQL.Data.Storage;
using GreenSQL.SqlNodes.Statement;

namespace GreenSQL.Data;

public class Database
{
    private Dictionary<string, DBTable> tables = new();
    private readonly IStorage storage;
    public List<DBTable> Tables => tables.Values.ToList();

    public Database(IStorage storage, string databaseName, List<DBTable> dbTables = null)
    {
        this.Name = databaseName;
        this.storage = storage;
        if (dbTables != null)
        {
            this.tables = dbTables.ToDictionary(x => x.Name);
        }
    }

    public string Name { get; private set; }

    public void CreateTable(string tableName)
    {
        if (tables.ContainsKey(tableName))
        {
            throw new Exception("Table already exists");
        }
        var table = new DBTable(tableName);
        tables.Add(tableName, table);
        storage.CreateTable(this, table);
    }
}