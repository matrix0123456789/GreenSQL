namespace GreenSQL.Core.Store;

public class Database
{
    public ReaderWriterLockSlim LockSlim = new ReaderWriterLockSlim();
    private Dictionary<string, Table> tables = new();
    public Table CreateTable(string tableName)
    {
        LockSlim.EnterWriteLock();
        try
        {
            if (tables.ContainsKey(tableName))

            {
                throw new Exception($"Table {tableName} already exists");
            }
            else
            {
                var ret = new Table();
                tables.Add(tableName, ret);
                return ret;
            }
        }finally
        {
            LockSlim.ExitWriteLock();
        }
    }

    public Table GetTable(string tableName)
    {
        LockSlim.EnterReadLock();
        try
        {
            if (tables.TryGetValue(tableName, out var table))
            {
                return table;
            }
            else
            {
                return null;
            }
        }
        finally
        {
            LockSlim.ExitReadLock();
        }
    }
}