namespace GreenSQL.Core.SQL.Nodes;

public class DropTableNode:NodeAbstract
{
    public PathNode TableName { get; set; }
}