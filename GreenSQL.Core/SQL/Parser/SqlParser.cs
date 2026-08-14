using System.Text.RegularExpressions;
using GreenSQL.Core.SQL.Nodes;

namespace GreenSQL.Core.SQL.Parser;

public class SqlParser
{
    private readonly string code;
    private int position;

    public SqlParser(string code, int position = 0)
    {
        this.code = code;
        this.position = position;
    }

    public static List<NodeAbstract> Parse(string code)
    {
        var parser = new SqlParser(code);
        return parser.Parse();
    }

    public List<NodeAbstract> Parse()
    {
        var ret = new List<NodeAbstract>();
        while (position < code.Length)
        {
            SkipWhitespace();
            if (Is("CREATE"))
            {
                Skip("CREATE");
                SkipWhitespace();
                if (Is("DATABASE"))
                {
                    Skip("DATABASE");
                    SkipWhitespace();
                    ret.Add(new CreateDatabaseNode()
                    {
                        DatabaseName = ParsePathNode()
                    });
                }
                else if (Is("TABLE"))
                {
                    Skip("TABLE");
                    SkipWhitespace();
                    var tableName = ParsePathNode();
                    var node = new CreateTableNode();
                    ret.Add(node);
                    node.TableName = tableName;
                    SkipWhitespace();
                    Skip("(");
                    while (position < code.Length && !Is(")"))
                    {
                        var columnDefinition = new ColumnDefinitionNode();
                        node.ColumnDefinitions.Add(columnDefinition);
                        columnDefinition.Name = ParsePathNode().Values.Single();
                        SkipWhitespace();
                        var type = "";
                        while (position < code.Length && new Regex("[a-zA-Z0-9]").IsMatch(code[position].ToString()))
                        {
                            type += code[position];
                            position++;
                        }

                        columnDefinition.DataType = type;
                        SkipWhitespace();
                        if (Is("NULL"))
                        {
                            Skip("NULL");
                            columnDefinition.IsNullable = true;
                        }
                        else if (Is("NOT"))
                        {
                            Skip("NOT");
                            SkipWhitespace();
                            Skip("NULL");

                            columnDefinition.IsNullable = false;
                        }

                        if (Is(")"))
                        {
                            Skip(")");
                            break;
                        }
                        else
                        {
                            Skip(",");
                        }
                    }
                }
                else
                {
                    Error("Expected 'DATABASE' or 'TABLE' after 'CREATE'");
                }
            }
            else if (Is("DROP"))
            {
                Skip("DROP");
                SkipWhitespace();
                if (Is("DATABASE"))
                {
                    Skip("DATABASE");
                    SkipWhitespace();
                    ret.Add(new DropDatabaseNode()
                    {
                        DatabaseName = ParsePathNode()
                    });
                }
                else if (Is("TABLE"))
                {
                    Skip("TABLE");
                    SkipWhitespace();
                    ret.Add(new DropTableNode()
                    {
                        TableName = ParsePathNode()
                    });
                }
                else
                {
                    Error("Expected 'DATABASE' or 'TABLE' after 'DROP'");
                }
            }
            
            else if (Is("INSERT"))
            {
                Skip("INSERT");
                SkipWhitespace();
                Skip("INTO");
                SkipWhitespace();
                var node = new InsertByTupleNode(); //Todo fix
                node.TableName = ParsePathNode();
                ret.Add(node);
                SkipWhitespace();
                if (Is("("))
                {
                    node.Collumns = new List<string>();
                    Skip("(");
                    while (position < code.Length && !Is(")"))
                    {
                        var columnName = ParsePathNode().Values.Single();
                        node.Collumns.Add(columnName);
                        if (Is(")"))
                        {
                            Skip(")");
                            break;
                        }
                        else
                        {
                            Skip(",");
                        }
                    }

                    SkipWhitespace();
                    Skip("VALUES");
                    SkipWhitespace();
                    Skip("(");
                    var row = new List<ExpressionNode>();
                    node.Values = new List<List<ExpressionNode>>() { row };
                    while (position < code.Length && !Is(")"))
                    {
                        var expression = ParseExpression();
                        row.Add(expression);
                        if (Is(")"))
                        {
                            Skip(")");
                            break;
                        }
                        else
                        {
                            Skip(",");
                        }
                    }
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        return ret;
    }

    private ExpressionNode ParseExpression()
    {
        ExpressionNode lastNode = null;
        while (position < code.Length)
        {
            if (Is(")"))
            {
                return lastNode;
            }
            else if (Is("'"))
            {
                if (lastNode != null)
                {
                    Error("Unexpected string literal");
                }

                Skip("'");
                var value = "";
                while (position < code.Length && !Is("'"))
                {
                    value += code[position];
                    position++;
                }

                Skip("'");
                lastNode=new StringLiteralNode(){Value = value};
            }
            else
            {
                Error("not expected character");
            }
        }

        return lastNode;
    }

    private PathNode ParsePathNode()
    {
        var ret = new List<string>();
        while (position < code.Length)
        {
            SkipWhitespace();
            if (Is("`"))
            {
                Skip("`");
                var part = "";
                while (position < code.Length && !Is("`"))
                {
                    part += code[position];
                    position++;
                }

                Skip("`");
                ret.Add(part);
            }
            else
            {
                var part = "";
                var notAllowedCharacters = new char[] { ';', ',', '.', '(', ')' };
                while (position < code.Length && !Is(".") && !char.IsWhiteSpace(code[position]) &&
                       !notAllowedCharacters.Contains(code[position]))
                {
                    part += code[position];
                    position++;
                }

                ret.Add(part);
            }

            SkipWhitespace();
            if (Is("."))
            {
                position++;
            }
            else
            {
                break;
            }
        }

        return new PathNode() { Values = ret.ToArray() };
    }

    private void SkipWhitespace()
    {
        while (position < code.Length && char.IsWhiteSpace(code[position]))
        {
            position++;
        }
    }

    public bool Is(string wanted)
    {
        return code.Length >= position + wanted.Length && code.Substring(position, wanted.Length)
            .Equals(wanted, StringComparison.OrdinalIgnoreCase);
    }

    public void Skip(string wanted)
    {
        if (!Is(wanted))
        {
            Error("Expected '" + wanted + "'");
        }

        position += wanted.Length;
    }

    private void Error(string message)
    {
        throw new Exception(message + " at position " + position);
    }
}