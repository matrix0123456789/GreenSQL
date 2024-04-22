namespace GreenSQL.SqlNodes.Statement;

public class DropTable: Statement
{
    public string TableName { get; set; }
    public string? DatabaseName { get; set; }
}