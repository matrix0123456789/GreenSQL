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
       else
       {
           throw new NotImplementedException();
       }
    }
}