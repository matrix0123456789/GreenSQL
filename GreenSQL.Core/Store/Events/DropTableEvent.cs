namespace GreenSQL.Core.Test.Store.Events;

public class DropTableEvent:Event
{
    public string[] Path { get; set; }
}