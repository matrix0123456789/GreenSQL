namespace GreenSQL.Core.SQL.Nodes;

public class AlterTableNode:NodeAbstract
{
    public PathNode TableName { get; set; }
    public string? RenameTo { get; set; } = null;
    public List<ColumnDefinitionNode> NewColumnDefinitions { get; set; }
    public List<(string,ColumnDefinitionNode)> ModifiedColumnDefinitions { get; set; }
    public List<string> DroppedColumns { get; set; }
}