namespace GreenSQL.Core.Test.Store.Events;

public enum EventType:ushort
{
    CreateDatabase,
    DropDatabase,
    CreateTable,
    DropTable,
    AddColumn,
    Insert,
}