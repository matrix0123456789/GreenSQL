namespace GreenSQL.Core.SQL.Nodes;

public class CreateTableNode:NodeAbstract
{
    public PathNode TableName { get; set; }
    public List<ColumnDefinitionNode> ColumnDefinitions { get; set; } = new List<ColumnDefinitionNode>();
}