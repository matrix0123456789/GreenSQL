using GreenSQL.Core.Test.Store.Events;

namespace GreenSQL.Core.Store;

public class BinarySerializer(StreamWrapper stream)
{
    
    public Event ReadSingle()
    {
        var type = stream.ReadEventType();
        if (type == EventType.CreateDatabase)
        {
            return new CreateDatabaseEvent()
            {
                Path = stream.ReadStringArray()
            };
        }else if (type == EventType.CreateTable)
        {
            return new CreateTableEvent()
            {
                Path = stream.ReadStringArray()
            };
        }else if (type == EventType.AddColumn)
        {
            return new AddColumnEvent()
            {
                TablePath = stream.ReadStringArray(),
                ColumnName = stream.ReadString(),
                ColumnType = (DataType)stream.ReadUint64(),
                IsNullable = stream.ReadBoolean()
            };
        }

        throw new NotImplementedException();
    }
    
    public void Write(Event e)
    {
        if (e is CreateDatabaseEvent createDatabaseEvent)
        {
            stream.WriteEventType(EventType.CreateDatabase);
            stream.WriteStringArray(createDatabaseEvent.Path);
        }else if (e is CreateTableEvent createTableEvent)

        {
            stream.WriteEventType(EventType.CreateTable);
            stream.WriteStringArray(createTableEvent.Path);
        }else if (e is AddColumnEvent addColumnEvent)
        {
            stream.WriteEventType(EventType.AddColumn);
            stream.WriteStringArray(addColumnEvent.TablePath);
            stream.WriteString(addColumnEvent.ColumnName);
            stream.WriteUint64((UInt64)addColumnEvent.ColumnType);
            stream.WriteBoolean(addColumnEvent.IsNullable);
        }else if (e is InsertEvent insertEvent)
        {
            throw new NotImplementedException();
            //_stream.WriteEventType(EventType.Insert);

        }
    }
}