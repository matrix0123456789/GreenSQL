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
        }
        else if (type == EventType.Insert)
        {
            var path = stream.ReadStringArray();
            var valuesCount = stream.ReadUint64();
            var values = new object[valuesCount];
            for (ulong i = 0; i < valuesCount; i++)
            {
                var dataType = stream.ReadDataType();
                if (dataType == DataType.Text)
                {
                    values[i] = stream.ReadString();
                }
                else if (dataType == DataType.Integer)
                {
                    values[i] = stream.ReadInt64();
                }
                else if (dataType == DataType.Float)
                {
                    values[i] = stream.ReadFloat64();
                }
                else if (dataType == DataType.Date)
                {
                    var daysSinceEpoch = stream.ReadInt64();
                    values[i] = new DateOnly(1, 1, 1).AddDays((int)daysSinceEpoch);
                }
                else if (dataType == DataType.Time)
                {
                    var ticks = stream.ReadInt64();
                    values[i] = new TimeOnly(ticks);
                }
                else if (dataType == DataType.DateTime)
                {
                    var ticksSinceEpoch = stream.ReadInt64();
                    values[i] = new DateTime(1, 1, 1, 0, 0, 0).AddTicks(ticksSinceEpoch);
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
                else if (value is double d)
                {
                    stream.WriteDataType(DataType.Float);
                    stream.WriteFloat64(d);
                }
                else if (value is DateOnly dateOnly)
                {
                    stream.WriteDataType(DataType.Date);
                    var diff = (dateOnly.ToDateTime(new TimeOnly(0, 0, 0)) - (new DateTime(1, 1, 1, 0, 0, 0)));
                    stream.WriteInt64((long)diff.TotalDays);
                }
                else if (value is TimeOnly timeOnly)
                {
                    stream.WriteDataType(DataType.Time);
                    stream.WriteInt64(timeOnly.Ticks);
                }
                else if (value is DateTime dateTime)
                {
                    stream.WriteDataType(DataType.DateTime);
                    stream.WriteInt64(dateTime.Ticks - (new DateTime(1, 1, 1, 0, 0, 0)).Ticks);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }
    }
}