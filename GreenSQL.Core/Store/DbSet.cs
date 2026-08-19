using GreenSQL.Core.SQL.Nodes;
using GreenSQL.Core.Test.Store.Events;

namespace GreenSQL.Core.Store;

public class DbSet
{
    public ReaderWriterLockSlim LockSlim = new ReaderWriterLockSlim();
    private Dictionary<string, Database> databases = new();
    public event Action<Event> Changed;

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
                var db = new Database();
                databases.Add(databaseName, db);
                Changed?.Invoke(new CreateDatabaseEvent { Path = new[] { databaseName } });
                db.Changed += (ev) =>
                {
                    if (ev is CreateTableEvent createTableEvent)
                    {
                        createTableEvent.Path = new[] { databaseName }.Concat(createTableEvent.Path).ToArray();
                    }
                    else if (ev is AddColumnEvent addColumnEvent)
                    {
                        addColumnEvent.TablePath = new[] { databaseName }.Concat(addColumnEvent.TablePath).ToArray();
                    }else if (ev is InsertEvent insertEvent)
                    {
                        insertEvent.TablePath = new[] { databaseName }.Concat(insertEvent.TablePath).ToArray();
                    }

                    Changed?.Invoke(ev);
                };
            }
        }
        finally
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