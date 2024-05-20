namespace GreenSQL.Data;

public class DBTable
{
    public DBTable(string tableName)
    {
        this.Name = tableName;
    }
    public string Name { get; private set; }
}