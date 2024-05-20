using GreenSQL.Data;
using GreenSQL.Parser;
using GreenSQL.SqlNodes.Statement;

namespace GreenSQL.Execution;

public class DBExecutionContext
{
    private readonly DBServer server;

    public DBExecutionContext(DBServer dbServer)
    {
        this.server = dbServer;
    }

    public void Execute(string sql)
    {
       var parsed= new QueryParser(sql).Parse();

       if (parsed is CreateDatabase)
       {
           server.CreateDatabase(((CreateDatabase)parsed).DatabaseName);
       }
       else if (parsed is CreateTable)
       {
           var database= server.GetDatabase(((CreateTable)parsed).DatabaseName);
              database.CreateTable(((CreateTable)parsed).TableName);
       }
       else
       {
           throw new NotImplementedException();
       }
    }
}