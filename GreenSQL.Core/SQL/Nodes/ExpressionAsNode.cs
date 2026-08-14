namespace GreenSQL.Core.SQL.Nodes;

public class ExpressionAsNode:NodeAbstract
{
    public ExpressionNode Expression { get; set; }
    public string Alias { get; set; }
}