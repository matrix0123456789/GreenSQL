using GreenSQL.Core.SQL.Nodes;
using GreenSQL.Core.SQL.Parser;

namespace GreenSQL.Core.Store;

public class CommandRunner
{
    private readonly DbSet dbSet;

    public CommandRunner(DbSet dbSet)
    {
        this.dbSet = dbSet;
    }

    public ExecutionResult ExecuteSqlCommand(string sql)
    {
        var parsedNodes = SqlParser.Parse(sql);
        if (parsedNodes.Count != 1)
        {
            throw new Exception("Expected 1 parsed node");
        }

        var node = parsedNodes[0];
        if (node is CreateDatabaseNode nodeT)
        {
            if (nodeT.DatabaseName.Values.Length == 1)
            {
                dbSet.CreateDatabase(nodeT.DatabaseName.Values[0]);
                return new ExecutionResult();
            }
            else
            {
                throw new NotImplementedException("todo do it later");
            }
        }
        else if (node is CreateTableNode createTableNode)
        {
            if (createTableNode.TableName.Values.Length == 2)
            {
                var dbName = createTableNode.TableName.Values[0];
                var tableName = createTableNode.TableName.Values[1];
                var db = dbSet.GetDatabase(dbName);
                if (db == null)
                {
                    throw new Exception($"Database {dbName} does not exist");
                }

                var table = db.CreateTable(tableName);
                foreach (var x in createTableNode.ColumnDefinitions)
                {
                    table.AddColumn(x.Name, DataTypeHelper.ParseDataType(x.DataType), x.IsNullable);
                }

                return new ExecutionResult();
            }
            else
            {
                throw new NotImplementedException("todo");
            }
        }
        else if (node is InsertByTupleNode insertByTupleNode)
        {
            if (insertByTupleNode.TableName.Values.Length == 2)
            {
                var dbName = insertByTupleNode.TableName.Values[0];
                var tableName = insertByTupleNode.TableName.Values[1];
                var db = dbSet.GetDatabase(dbName);
                if (db == null)
                {
                    throw new Exception($"Database {dbName} does not exist");
                }

                var table = db.GetTable(tableName);
                if (table == null)
                {
                    throw new Exception($"Table {tableName} does not exist");
                }

                var collumnMap = new int[insertByTupleNode.Collumns.Count];
                for (var i = 0; i < insertByTupleNode.Collumns.Count; i++)
                {
                    var resultIndex = table.ColumnNames.ToList().FindIndex(x =>
                        x.Equals(insertByTupleNode.Collumns[i], StringComparison.CurrentCultureIgnoreCase));
                    collumnMap[i] = resultIndex;
                }

                var columnTypes = table.ColumnTypes;
                foreach (var row in insertByTupleNode.Values)
                {
                    var newRow = table.GetDefaultRow();
                    for (var i = 0; i < collumnMap.Length; i++)
                    {
                       
                        newRow[collumnMap[i]] = ExecuteExpression(row[i], columnTypes[collumnMap[i]]);
                    }
                    table.AddRow(newRow);
                }
                
                return new ExecutionResult();
            }
            else
            {
                throw new NotImplementedException("todo");
            }
        }
        else if (node is SelectNode sqlSelectNode)
        {
            //oversimplified
            var tablePath=(sqlSelectNode.From.First() as PathNode);
            var db=dbSet.GetDatabase(tablePath.Values[0]);
            if (db == null)
            {
                throw new Exception($"Database {tablePath.Values[0]} does not exist");
            }
            var table = db.GetTable(tablePath.Values[1]);
            if (table == null)
            {
                throw new Exception($"Table {tablePath.Values[1]} does not exist");
            }

            return new ExecutionResult()
            {
                Data = table.GetAllData()
            };
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    private object ExecuteExpression(ExpressionNode expressionNode, DataType? expectedType=null)
    {
        if(expressionNode is StringLiteralNode stringLiteralNode)
        {
            return stringLiteralNode.Value;
        }
        else if(expressionNode is IntegerLiteralNode integerLiteralNode)
        {
            if (expectedType == DataType.Integer)
            {
                return (long)integerLiteralNode.Value;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        else
        {
            throw new NotImplementedException();
        }
    }
}