namespace GreenSQL.Core.SQL.Nodes;

public class DropDatabaseNode:NodeAbstract
{
    public PathNode DatabaseName { get; set; }
}