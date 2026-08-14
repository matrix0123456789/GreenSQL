namespace GreenSQL.Core.SQL.Nodes;

public class InsertByTupleNode:NodeAbstract
{
    public PathNode TableName { get; set; }
    public List<string>? Collumns { get; set; } = null;
    public List<List<ExpressionNode>> Values { get; set; } = new List<List<ExpressionNode>>();
}