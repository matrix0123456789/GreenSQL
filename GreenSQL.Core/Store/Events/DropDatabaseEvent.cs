namespace GreenSQL.Core.Test.Store.Events;

public class DropDatabaseEvent:Event
{
    public string[] Path { get; set; }
}