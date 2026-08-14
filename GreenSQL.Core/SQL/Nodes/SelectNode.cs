namespace GreenSQL.Core.SQL.Nodes;

public class SelectNode:NodeAbstract
{
    public List<ExpressionAsNode> Collumns { get; set; }
    public List<NodeAbstract> From { get; set; }
    public ExpressionNode? Where { get; set; }
    public long? Skip { get; set; } = null;
    public long? Take { get; set; } = null;
    //todo JOIN, Group By, Having, Order By
}