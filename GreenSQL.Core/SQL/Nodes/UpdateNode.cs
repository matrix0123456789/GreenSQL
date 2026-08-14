namespace GreenSQL.Core.SQL.Nodes;

public class UpdateNode:NodeAbstract
{
    public PathNode TableName { get; set; }
    public List<ExpressionAsNode> Sets { get; set; }
    public ExpressionNode? Where { get; set; }
    public long? Skip { get; set; } = null;
    public long? Take { get; set; } = null;
}