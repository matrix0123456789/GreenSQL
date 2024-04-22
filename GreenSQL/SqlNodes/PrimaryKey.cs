namespace GreenSQL.SqlNodes;

public class PrimaryKey:AbstractIndexDefinition
{
    public List<string> Columns { get; set; }
}