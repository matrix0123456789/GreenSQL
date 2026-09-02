using System.Numerics;

namespace GreenSQL.Core.SQL.Nodes;

public class IntegerLiteralNode: ExpressionNode
{
    public BigInteger Value { get; set; }
}