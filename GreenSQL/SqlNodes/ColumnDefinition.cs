namespace GreenSQL.SqlNodes;

public class ColumnDefinition
{
    public string Name { get; set; }
    public string Type { get; set; }
    public bool IsNotNull { get; set; }
}