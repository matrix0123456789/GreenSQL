using GreenSQL.Core.Store;
using GreenSQL.Core.Test.Store.Events;

namespace GreenSQL.Core.Test.Store;

public class FileStore
{
    private readonly StreamWrapper _stream;
    private readonly DbSet _dbSet;
    private readonly BinarySerializer _binarySerializer;

    public FileStore(DbSet dbSet, StreamWrapper stream)
    {
        _stream = stream;
        _dbSet = dbSet;
        _binarySerializer = new BinarySerializer(stream);
    }

    public void StartListening()
    {
        _dbSet.Changed += (ev) => { _binarySerializer.Write(ev); };
    }

    public void LoadFromStream()
    {
        while (_stream.HasMoreData)
        {
            var e = _binarySerializer.ReadSingle();
            if (e is CreateDatabaseEvent createDatabaseEvent)
            {
                _dbSet.CreateDatabase(createDatabaseEvent.Path.First()); //todo first is tmp
            }
            else if (e is CreateTableEvent createTableEvent)
            {
                var db = _dbSet.GetDatabase(createTableEvent.Path.First()); //todo
                db.CreateTable(createTableEvent.Path.Last());
            }
            else if (e is AddColumnEvent addColumnEvent)
            {
                var db = _dbSet.GetDatabase(addColumnEvent.TablePath.First()); //todo
                var table = db.GetTable(addColumnEvent.TablePath.Last());
                table.AddColumn(addColumnEvent.ColumnName, addColumnEvent.ColumnType, addColumnEvent.IsNullable);
            }
            else if (e is InsertEvent insertEvent)
            {
                var db = _dbSet.GetDatabase(insertEvent.TablePath.First()); //todo
                var table = db.GetTable(insertEvent.TablePath.Last());
                table.AddRow(insertEvent.Values);
            }
        }
    }

}