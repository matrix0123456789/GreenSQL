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
        }
        else if (type == EventType.CreateTable)
        {
            return new CreateTableEvent()
            {
                Path = stream.ReadStringArray()
            };
        }
        else if (type == EventType.AddColumn)
        {
            return new AddColumnEvent()
            {
                TablePath = stream.ReadStringArray(),
                ColumnName = stream.ReadString(),
                ColumnType = (DataType)stream.ReadUint64(),
                IsNullable = stream.ReadBoolean()
            };
        } else if (type == EventType.Insert)
        {
            var path=stream.ReadStringArray();
            var valuesCount=stream.ReadUint64();
            var values=new object[valuesCount];
            for (ulong i = 0; i < valuesCount; i++)
            {
                var dataType=stream.ReadDataType();
                if (dataType==DataType.Text)
                {
                    values[i]=stream.ReadString();
                }
                else if (dataType==DataType.Integer)
                {
                    values[i]=stream.ReadInt64();
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            return new InsertEvent()
            {
                TablePath = path,
                Values = values
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
        }
        else if (e is CreateTableEvent createTableEvent)

        {
            stream.WriteEventType(EventType.CreateTable);
            stream.WriteStringArray(createTableEvent.Path);
        }
        else if (e is AddColumnEvent addColumnEvent)
        {
            stream.WriteEventType(EventType.AddColumn);
            stream.WriteStringArray(addColumnEvent.TablePath);
            stream.WriteString(addColumnEvent.ColumnName);
            stream.WriteUint64((UInt64)addColumnEvent.ColumnType);
            stream.WriteBoolean(addColumnEvent.IsNullable);
        }
        else if (e is InsertEvent insertEvent)
        {
            stream.WriteEventType(EventType.Insert);
            stream.WriteStringArray(insertEvent.TablePath);
            stream.WriteUint64((UInt64)insertEvent.Values.LongLength);
            foreach (var value in insertEvent.Values)
            {
                if (value is string s)
                {
                    stream.WriteDataType(DataType.Text);
                    stream.WriteString(s);
                }
                else if (value is long l)
                {
                    stream.WriteDataType(DataType.Integer);
                    stream.WriteInt64(l);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }
    }
}