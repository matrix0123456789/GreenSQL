using GreenSQL.Core.Test.Store.Events;

namespace GreenSQL.Core.Store;

public class Database
{
    public ReaderWriterLockSlim LockSlim = new ReaderWriterLockSlim();
    private Dictionary<string, Table> tables = new();
    public event Action<Event> Changed;

    public List<string> ListTables
    {
        get
        {
            LockSlim.EnterReadLock();
            try
            {
                return tables.Keys.ToList();
            }
            finally
            {
                LockSlim.ExitReadLock();
            }
        }
    }

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
                ret.Changed += (ev) =>
                {
                    if (ev is AddColumnEvent addColumnEvent)
                    {
                        addColumnEvent.TablePath = new[] { tableName };
                    }
                    else if (ev is InsertEvent insertEvent)
                    {
                        insertEvent.TablePath = new[] { tableName };
                    }

                    Changed?.Invoke(ev);
                };
                tables.Add(tableName, ret);
                Changed?.Invoke(new CreateTableEvent() { Path = new[] { tableName } });
                return ret;
            }
        }
        finally
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