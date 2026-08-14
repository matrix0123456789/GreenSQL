namespace GreenSQL.Core.SQL.Nodes;

public class ColumnDefinitionNode
{
    public string Name { get; set; }
    public string DataType { get; set; }
    public string DataTypeParameter { get; set; }
    public bool IsNullable { get; set; } = true;
}