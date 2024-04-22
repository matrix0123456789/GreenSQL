namespace GreenSQL.SqlNodes;

public class IndexDefinition:AbstractIndexDefinition
{
    public string IndexName { get; set; }
    public List<string> Columns { get; set; }
}