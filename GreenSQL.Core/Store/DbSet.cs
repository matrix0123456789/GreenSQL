using GreenSQL.Core.SQL.Nodes;

namespace GreenSQL.Core.Store;

public class DbSet
{
    public ReaderWriterLockSlim LockSlim = new ReaderWriterLockSlim();
    private Dictionary<string, Database> databases = new();
    public List<string> DatabaseNames
    {
        get
        {
            LockSlim.EnterReadLock();
            try
            {
                return databases.Keys.ToList();
            }
            finally
            {
                LockSlim.ExitReadLock();
            }
        }
    }

    public void CreateDatabase(string databaseName)
    {
        LockSlim.EnterWriteLock();
        try
        {
            if (databases.ContainsKey(databaseName))
            {
                throw new Exception($"Database {databaseName} already exists");
            }
            else
            {
                databases.Add(databaseName, new Database());
            }
        }finally
        {
            LockSlim.ExitWriteLock();
        }
    }

    public Database? GetDatabase(string databaseName)
    {
        LockSlim.EnterReadLock();
        try
        {
            if (databases.ContainsKey(databaseName))
            {
                return databases[databaseName];
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