namespace GreenSQL.Data.Storage;

public interface IStorage
{
    void CreateDatabase(Database database);
    void CreateTable(Database database, DBTable table);
}