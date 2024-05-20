namespace GreenSQL.Data;

public class Database
{
    private Dictionary<string, DBTable> tables = new();

    public Database(string databaseName)
    {
        this.Name = databaseName;
    }

    public string Name { get; private set; }
}