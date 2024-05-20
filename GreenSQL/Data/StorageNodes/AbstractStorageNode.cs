namespace GreenSQL.Data.StorageNodes;

public abstract class AbstractStorageNode
{
    abstract public StorageNodeType NodeType {  get; }
    abstract public int Size { get; }
    
    public virtual void WriteToStream(BinaryWriter writer)
    {
        var position = writer.BaseStream.Position;
        writer.Write((int)NodeType);
        writer.Write(Size);
        position += Size;
        writer.Flush();
        writer.BaseStream.Position = position;
    }

    public static AbstractStorageNode ReadFromStream(BinaryReader reader)
    {
        AbstractStorageNode ret;
        var startPosition = reader.BaseStream.Position;
        var typeInt=reader.ReadInt32();
        //todo add pretty exception whet type is wrong
        var type=(StorageNodeType)typeInt;
        var size=reader.ReadInt32();

        if (type == StorageNodeType.TableDefinitionV1)
        {
            ret= TableDefinitionNodeV1.ReadNodeFromStream(reader, size);
        }
        else
        {
            throw new NotImplementedException();
        }
        
        reader.BaseStream.Position=startPosition+size;
        return ret;
    }
}