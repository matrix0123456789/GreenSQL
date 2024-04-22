using GreenSQL.SqlNodes;
using GreenSQL.SqlNodes.Statement;

namespace GreenSQL.Parser;

public class QueryParser
{
    private readonly string code;
    private int position = 0;

    public QueryParser(string code)
    {
        this.code = code;
    }

    public Statement Parse()
    {
        if (IsKeyword("CREATE"))
        {
            SkipKeyword("CREATE");
            SkipWhiteSpace();
            if (IsKeyword("DATABASE") || IsKeyword("SCHEMA"))
            {
                SkipKeyword("DATABASE", "SCHEMA");
                SkipWhiteSpace();
                return new CreateDatabase
                {
                    DatabaseName = ParseIdentifier()
                };
            }
            else if (IsKeyword("TABLE"))
            {
                SkipKeyword("TABLE");
                SkipWhiteSpace();
                var tableName = ParseIdentifier();
                var columnDefinitions = new List<ColumnDefinition>();
                var indexes = new List<AbstractIndexDefinition>();
                SkipWhiteSpace();
                if (position < code.Length && code[position] == '(')
                {
                    position++;
                    while (position < code.Length)
                    {
                        if (IsKeyword("PRIMARY KEY"))
                        {
                            SkipKeyword("PRIMARY KEY");
                            var columns = ReadColumnList();
                            indexes.Add(new PrimaryKey() { Columns = columns });
                        }
                        else if (IsKeyword("INDEX"))
                        {
                            SkipKeyword("INDEX");
                            var indexName = ParseIdentifier();
                            SkipWhiteSpace();
                            var columns = ReadColumnList();
                            indexes.Add(new IndexDefinition() { IndexName = indexName, Columns = columns });
                        }
                        else
                        {
                            var (columnDefinition, key) = ParseColumnDefinition();
                            columnDefinitions.Add(columnDefinition);
                            if (key != null)
                            {
                                indexes.Add(key);
                            }
                        }

                        SkipWhiteSpace();
                            if (position < code.Length && code[position] == ',')
                            {
                                position++;
                            }
                            else if (position < code.Length && code[position] == ')')
                            {
                                position++;
                                return new CreateTable
                                {
                                    TableName = tableName,
                                    Columns = columnDefinitions,
                                    Indexes = indexes
                                };
                            }
                            else
                            {
                                throw Exception("Expected ',' or ')'");
                            }
                        
                        
                        SkipWhiteSpace();
                    }

                    throw Exception();
                }
                else
                {
                    throw Exception("Expected '('");
                }
            }
            else
            {
                throw Exception("Expected keyword DATABASE, SCHEMA or TABLE");
            }
        }
        else if (IsKeyword("DROP"))
        {
            SkipKeyword("DROP");
            SkipWhiteSpace();
            if (IsKeyword("DATABASE") || IsKeyword("SCHEMA"))
            {
                SkipKeyword("DATABASE", "SCHEMA");
                SkipWhiteSpace();
                return new DropDatabase
                {
                    DatabaseName = ParseIdentifier()
                };
            }
            else if (IsKeyword("TABLE"))
            {
                SkipKeyword("TABLE");
                SkipWhiteSpace();
                var firstIdentifier = ParseIdentifier();
                SkipWhiteSpace();
                if (position < code.Length && code[position] == '.')
                {
                    position++;
                    return new DropTable
                    {
                        DatabaseName = firstIdentifier,
                        TableName = ParseIdentifier()
                    };
                }
                else
                {
                    return new DropTable
                    {
                        TableName = firstIdentifier
                    };
                }
            }
            else
            {
                throw Exception("Expected keyword DATABASE or TABLE");
            }
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

    private List<string> ReadColumnList()
    {
        SkipWhiteSpace();
        if (code[position] != '(')
        {
            throw Exception("Expected '('");
        }

        position++;
        var columns = new List<string>();
        while (position < code.Length)
        {
            var columnName = ParseIdentifier();
            columns.Add(columnName);
            SkipWhiteSpace();
            if (position < code.Length && code[position] == ',')
            {
                position++;
            }
            else if (position < code.Length && code[position] == ')')
            {
                position++;
                return columns;
            }
            else
            {
                throw Exception("Expected ',' or ')'");
            }
        }

        throw Exception();
    }

    private (ColumnDefinition, AbstractIndexDefinition?) ParseColumnDefinition()
    {
        var columnName = ParseIdentifier();
        SkipWhiteSpace();
        var columnType = ParseIdentifier();
        SkipWhiteSpace();
        var columnDefinition = new ColumnDefinition
        {
            Name = columnName,
            Type = columnType
        };
        AbstractIndexDefinition? key = null;
        while (position < code.Length && code[position] != ',' && code[position] != ')' && code[position] != ';')
        {
            if (IsKeyword("null"))
            {
                SkipKeyword("null");
            }
            else if (IsKeyword("not null"))
            {
                SkipKeyword("not null");
                columnDefinition.IsNotNull = true;
            }
            else if (IsKeyword("primary key"))
            {
                SkipKeyword("primary key");
                key = new PrimaryKey
                {
                    Columns = new List<string> { columnName }
                };
            }
            else
            {
                throw Exception("Unknown keyword");
            }

            SkipWhiteSpace();
        }

        return (columnDefinition, key);
    }

    private string ParseIdentifier()
    {
        SkipWhiteSpace();
        if (code[position] == '`')
        {
            position++;
            var start = position;
            while (position < code.Length)
            {
                if (code[position] == '`')
                {
                    var identifier = code.Substring(start, position - start);
                    position++;
                    return identifier;
                }
                else
                {
                    position++;
                }
            }

            throw Exception("Expected closing backtick");
        }
        else
        {
            var start = position;
            while (position < code.Length)
            {
                var c = code[position];
                if (char.IsWhiteSpace(c) || c==',' || c=='(' || c==')' || c==';')
                {
                    break;
                }
                else
                {
                    position++;
                }
            }

            return code.Substring(start, position - start);
        }
    }

    private Exception Exception(string message = "Error parsing query")
    {
        return new Exception(message + " at position " + position);
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
        if(position+ keyword.Length > code.Length)
        {
            return false;
        }
        return code.Substring(position, keyword.Length).Equals(keyword, StringComparison.OrdinalIgnoreCase);
    }

    private void SkipKeyword(params string[] keywords)
    {
        foreach (var keyword in keywords)
        {
            if (IsKeyword(keyword))
            {
                position += keyword.Length;
                return;
            }
        }

        throw Exception("Expected keyword " + keywords);
    }
}