namespace GreenSQL.Data.StorageNodes;

public abstract class AbstractStorageNode
{
    abstract public StorageNodeType NodeType {  get; }
    abstract public int Size { get; }
    
    public virtual void WriteToStream(Stream stream)
    {
        var position = stream.Position;
        stream.Write(BitConverter.GetBytes((int)NodeType), 0, 4);
        stream.Write(BitConverter.GetBytes(Size), 0, 4);
        position += Size;
        stream.Position = position;
    }
}