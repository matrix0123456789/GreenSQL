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
        if (IsKeyword("CREATE"))
        {
            
            throw new NotImplementedException();
        }else if (IsKeyword("DROP"))
        {
            throw new NotImplementedException();
        }
        else if (IsKeyword("TRUNCATE"))
        {
            SkipKeyword("TRUNCATE");
            SkipWhiteSpace();
            if (IsKeyword("TABLE"))
            {
                SkipKeyword("TABLE");
                SkipWhiteSpace();
                return new TruncateTable
                {
                    TableName = ParseIdentifier()
                };
            }
            else
            {
                throw Exception("Expected keyword TABLE");
            }
        }
        else
        {
            throw Exception();
        }
    }

    private string ParseIdentifier()
    {
        SkipWhiteSpace();
        if (code[position] == '`')
        {
            position++;
            var start = position;
            while (position<code.Length)
            {
                if(code[position]=='`')
                {
                    var identifier = code.Substring(start, position-start);
                    position++;
                    return identifier;
                }
            }
            throw Exception("Expected closing backtick");
        }
        else
        {
            var start = position;
            while (position < code.Length)
            {
                if (char.IsWhiteSpace(code[position]))
                {
                    break;
                }
                else
                {
                    position++;
                }
                
            }
            return code.Substring(start, position-start);
        }
    }

    private Exception Exception(string message="Error parsing query")
    {
        return new Exception(message+" at position "+position);
    }
    private void SkipWhiteSpace()
    {
        while (position < code.Length && char.IsWhiteSpace(code[position]))
        {
            position++;
        }
    }

    private bool IsKeyword(string keyword)
    {
        return code.Substring(position, keyword.Length).Equals(keyword, StringComparison.OrdinalIgnoreCase);
    }
    private void SkipKeyword(string keyword)
    {
        if (IsKeyword(keyword))
        {
            position += keyword.Length;
        }
        else
        {
            throw Exception("Expected keyword "+keyword);
        }
    }
}