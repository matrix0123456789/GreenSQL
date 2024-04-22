using GreenSQL.SqlNodes.Statement;

namespace GreenSQL.Parser;

public class QueryParser
{
    private readonly string code;
    private int position=0;

    public QueryParser(string code)
    {
        this.code = code;
    }

    public Statement Parse()
    {
        throw new NotImplementedException();
    }
}