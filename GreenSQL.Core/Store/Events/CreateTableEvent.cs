namespace GreenSQL.Core.Test.Store.Events;

public class CreateTableEvent : Event
{
    public string[] Path { get; set; }
}