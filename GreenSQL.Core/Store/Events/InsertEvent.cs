namespace GreenSQL.Core.Test.Store.Events;

public class InsertEvent : Event
{
    public string[] TablePath { get; set; }
    public object[] Values { get; set; }
}