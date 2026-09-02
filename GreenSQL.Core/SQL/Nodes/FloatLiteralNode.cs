using System.Numerics;

namespace GreenSQL.Core.SQL.Nodes;

public class FloatLiteralNode: ExpressionNode
{
    public double Value { get; set; }
}