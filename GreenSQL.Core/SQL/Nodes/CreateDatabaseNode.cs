namespace GreenSQL.Core.SQL.Nodes;

public class CreateDatabaseNode:NodeAbstract
{
    public PathNode DatabaseName { get; set; }
}