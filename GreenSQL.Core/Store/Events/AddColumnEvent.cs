namespace GreenSQL.Core.Test.Store.Events;

public class AddColumnEvent:Event
{
    public string[] TablePath { get; set; }
    public string ColumnName { get; set; }
    public DataType ColumnType { get; set; }
    public bool IsNullable { get; set; }
}