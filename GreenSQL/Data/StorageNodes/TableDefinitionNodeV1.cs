namespace GreenSQL.Data.StorageNodes;

public class TableDefinitionNodeV1 : AbstractStorageNode
{
    public override StorageNodeType NodeType => StorageNodeType.TableDefinitionV1;
    public override int Size => 8;

    public override void WriteToStream(Stream stream)
    {
        base.WriteToStream(stream);
        //todo
    }
}