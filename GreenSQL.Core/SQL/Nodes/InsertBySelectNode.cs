namespace GreenSQL.Core.SQL.Nodes;

public class InsertBySelectNode:NodeAbstract
{
    public PathNode TableName { get; set; }
    public List<string>? Collumns { get; set; } = null;
    public SelectNode Select { get; set; }
}