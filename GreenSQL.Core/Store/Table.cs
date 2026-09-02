using GreenSQL.Core.Test.Store.Events;

namespace GreenSQL.Core.Store;

public class Table
{
    public ReaderWriterLockSlim LockSlim = new ReaderWriterLockSlim();
    private Dictionary<string, Column> columns = new();
    private List<object[]> data = new List<object[]>();
    public event Action<Event> Changed;

    public IEnumerable<string> ColumnNames
    {
        get
        {
            LockSlim.EnterReadLock();
            try
            {
                return columns.Keys.ToList();
            }
            finally
            {
                LockSlim.ExitReadLock();
            }
        }
    }
    public DataType[] ColumnTypes
    {
        get
        {
            LockSlim.EnterReadLock();
            try
            {
                return columns.Values.Select(c => c.DataType).ToArray();
            }
            finally
            {
                LockSlim.ExitReadLock();
            }
        }
    }
    

    public Column AddColumn(string name, DataType dataType, bool isNullable)
    {
        LockSlim.EnterWriteLock();
        try
        {
            if (columns.ContainsKey(name))
            {
                throw new Exception($"Column {name} already exists");
            }
            else
            {
                var column = new Column()
                {
                    DataType = dataType,
                    IsNullable = isNullable
                };
                columns.Add(name, column);
                Changed?.Invoke(new AddColumnEvent() { ColumnName = name, ColumnType = dataType, IsNullable = isNullable });
                return column;
            }
        }
        finally
        {
            LockSlim.ExitWriteLock();
        }
    }

    public object[] GetDefaultRow()
    {
        LockSlim.EnterReadLock();
        try
        {
            var ret = new object[columns.Count];
            for (var i = 0; i < columns.Count; i++)
            {
                ret[i] = DataTypeHelper.GetDefault(columns.ToList()[i].Value.DataType);
            }

            return ret;
        }
        finally
        {
            LockSlim.ExitReadLock();
        }
    }

    public void AddRow(object[] newRow)
    {
        LockSlim.EnterWriteLock();
        try
        {
            data.Add(newRow);
            Changed?.Invoke(new InsertEvent() { Values = newRow });
        }
        finally
        {
            LockSlim.ExitWriteLock();
        }
    }

    public IEnumerable<object[]> GetAllData()
    {
        LockSlim.EnterReadLock();
        try
        {
            return data.ToList();
        }
        finally
        {
            LockSlim.ExitReadLock();
        }
    }
}