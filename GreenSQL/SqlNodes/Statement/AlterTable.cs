namespace GreenSQL.SqlNodes.Statement;

public class AlterTable : Statement
{
    public string TableName { get; set; }
    public List<ColumnDefinition> AddColumns { get; set; }
    public List<string> DropColumns { get; set; }
}