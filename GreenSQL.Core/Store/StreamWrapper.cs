using System.Text;
using GreenSQL.Core.Test.Store.Events;

namespace GreenSQL.Core.Store;

public class StreamWrapper(Stream stream)
{
    private BinaryReader _reader=new BinaryReader(stream);
    private BinaryWriter _writer=new BinaryWriter(stream);
    public bool HasMoreData =>stream.Position<stream.Length;

    public string ReadStringFixedLength(int bytesCount)
    {
        var bytes=_reader.ReadBytes(bytesCount);
        return Encoding.UTF8.GetString(bytes);
    }
    
    public void WriteStringFixedLength(string text, int bytesCount)
    {
        var bytes=Encoding.UTF8.GetBytes(text);
        if (bytes.Length < bytesCount)
        {
            _writer.Write(bytes);
            _writer.Write(new byte[bytesCount - bytes.Length]);
            _writer.Flush();
        }
        else
        {
            _writer.Write(bytes, 0, bytesCount);
        }
    }

    public void WriteEventType(EventType value)
    {
        var x=(ushort)value;
        _writer.Write(x);
        _writer.Flush();
    }

    public EventType ReadEventType()
    {
        var x=_reader.ReadUInt16();
        return (EventType)x;
    }
    
    public void WriteDataType(DataType value)
    {
        var x=(ushort)value;
        _writer.Write(x);
        _writer.Flush();
    }

    public DataType ReadDataType()
    {
        var x=_reader.ReadUInt16();
        return (DataType)x;
    }

    public void WriteStringArray(string[] arr)
    {
        _writer.Write((ulong)arr.Length);
        _writer.Flush();
        foreach (var e in arr)
        {
            WriteString(e);
        }
    }

    public void WriteString(string s)
    {
        var bytes = Encoding.UTF8.GetBytes(s);
       _writer.Write((ulong)bytes.LongLength);
       _writer.Write(bytes);
       _writer.Flush();
    }

    public string ReadString()
    {
        var bytesCount = _reader.ReadUInt64();
        var bytes=_reader.ReadBytes((int)bytesCount);//Todo rething later of above 2GB
        return Encoding.UTF8.GetString(bytes);
    }

    public string[] ReadStringArray()
    {
        var length = _reader.ReadUInt64();
        var ret=new string[length];
        for (ulong i = 0; i < length; i++)
        {
            ret[i] = ReadString();
        }
        return ret;
    }

    public void WriteUint64(ulong columnType)
    {
        _writer.Write(columnType);
        _writer.Flush();
    }
    public void WriteBoolean(bool value)
    {
        _writer.Write(value);
        _writer.Flush();
    }
    public UInt64 ReadUint64()
    {
        return _reader.ReadUInt64();
    }

    public bool ReadBoolean()
    {
        return _reader.ReadBoolean();
    }
}