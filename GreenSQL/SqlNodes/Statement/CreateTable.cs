namespace GreenSQL.SqlNodes.Statement;

public class CreateTable: Statement
{
    public string DatabaseName { get; set; }
    public string TableName { get; set; }
    public List<ColumnDefinition> Columns { get; set; }
    public List<AbstractIndexDefinition> Indexes { get; set; }
}