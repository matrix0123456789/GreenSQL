namespace GreenSQL.Core.Test.Store.Events;

public class CreateDatabaseEvent : Event
{
    public string[] Path { get; set; }
}