using System.Numerics;
using GreenSQL.Core.SQL.Nodes;
using GreenSQL.Core.SQL.Parser;

namespace GreenSQL.Core.Test.SQL.Parser;

[TestFixture]
public class SqlParserTest
{
    [Test]
    public void CreateDatabase()
    {
        var node = SqlParser.Parse(@"CREATE DATABASE TestDB").First();

        Assert.That(node, Is.TypeOf<CreateDatabaseNode>());
        Assert.That(((CreateDatabaseNode)node).DatabaseName.Values[0], Is.EqualTo("TestDB"));
    }

    [Test]
    public void CreateDatabase2()
    {
        var node = SqlParser.Parse(@"CREATE DATABASE ParentDB.`ChildDB`").First();

        Assert.That(node, Is.TypeOf<CreateDatabaseNode>());
        Assert.That(((CreateDatabaseNode)node).DatabaseName.Values[0], Is.EqualTo("ParentDB"));
        Assert.That(((CreateDatabaseNode)node).DatabaseName.Values[1], Is.EqualTo("ChildDB"));
    }

    [Test]
    public void DropDatabase()
    {
        var node = SqlParser.Parse(@"DROP DATABASE TestDB").First();

        Assert.That(node, Is.TypeOf<DropDatabaseNode>());
        Assert.That(((DropDatabaseNode)node).DatabaseName.Values[0], Is.EqualTo("TestDB"));
    }

    [Test]
    public void DropDatabase2()
    {
        var node = SqlParser.Parse(@"DROP DATABASE ParentDB.`ChildDB`").First();

        Assert.That(node, Is.TypeOf<DropDatabaseNode>());
        Assert.That(((DropDatabaseNode)node).DatabaseName.Values[0], Is.EqualTo("ParentDB"));
        Assert.That(((DropDatabaseNode)node).DatabaseName.Values[1], Is.EqualTo("ChildDB"));
    }

    [Test]
    public void CreateTable()
    {
        var node = SqlParser
            .Parse(@"CREATE TABLE `DbName`.TableName (column1 int, `column2` text, column3 datetime not null)").First();

        Assert.That(node, Is.TypeOf<CreateTableNode>());
        Assert.That(((CreateTableNode)node).TableName.Values[0], Is.EqualTo("DbName"));
        Assert.That(((CreateTableNode)node).TableName.Values[1], Is.EqualTo("TableName"));
        Assert.That(((CreateTableNode)node).ColumnDefinitions.Count, Is.EqualTo(3));
        Assert.That(((CreateTableNode)node).ColumnDefinitions[0].Name, Is.EqualTo("column1"));
        Assert.That(((CreateTableNode)node).ColumnDefinitions[0].DataType, Is.EqualTo("int"));
        Assert.That(((CreateTableNode)node).ColumnDefinitions[0].IsNullable, Is.EqualTo(true));
        Assert.That(((CreateTableNode)node).ColumnDefinitions[1].Name, Is.EqualTo("column2"));
        Assert.That(((CreateTableNode)node).ColumnDefinitions[1].DataType, Is.EqualTo("text"));
        Assert.That(((CreateTableNode)node).ColumnDefinitions[1].IsNullable, Is.EqualTo(true));
        Assert.That(((CreateTableNode)node).ColumnDefinitions[2].Name, Is.EqualTo("column3"));
        Assert.That(((CreateTableNode)node).ColumnDefinitions[2].DataType, Is.EqualTo("datetime"));
        Assert.That(((CreateTableNode)node).ColumnDefinitions[2].IsNullable, Is.EqualTo(false));
    }

    [Test]
    public void CreateInsertSimple()
    {
        var node = SqlParser.Parse("INSERT INTO tableName (column1) VALUES ('Test1')").First();

        Assert.That(node, Is.TypeOf<InsertByTupleNode>());
        Assert.That(((InsertByTupleNode)node).TableName.Values[0], Is.EqualTo("tableName"));
        Assert.That(((InsertByTupleNode)node).Collumns.Count, Is.EqualTo(1));
        Assert.That(((InsertByTupleNode)node).Collumns[0], Is.EqualTo("column1"));
        Assert.That(((InsertByTupleNode)node).Values[0][0], Is.TypeOf<StringLiteralNode>());
        Assert.That((((InsertByTupleNode)node).Values[0][0] as StringLiteralNode).Value, Is.EqualTo("Test1"));
    }

    [Test]
    public void CreateInsert2Columns()
    {
        var node = SqlParser.Parse("INSERT INTO tableName (column1, column2) VALUES ('Test1', 'Test2')").First();

        Assert.That(node, Is.TypeOf<InsertByTupleNode>());
        Assert.That(((InsertByTupleNode)node).TableName.Values[0], Is.EqualTo("tableName"));
        Assert.That(((InsertByTupleNode)node).Collumns.Count, Is.EqualTo(2));
        Assert.That(((InsertByTupleNode)node).Collumns[0], Is.EqualTo("column1"));
        Assert.That(((InsertByTupleNode)node).Collumns[1], Is.EqualTo("column2"));
        Assert.That(((InsertByTupleNode)node).Values[0][0], Is.TypeOf<StringLiteralNode>());
        Assert.That((((InsertByTupleNode)node).Values[0][0] as StringLiteralNode).Value, Is.EqualTo("Test1"));
        Assert.That(((InsertByTupleNode)node).Values[0][1], Is.TypeOf<StringLiteralNode>());
        Assert.That((((InsertByTupleNode)node).Values[0][1] as StringLiteralNode).Value, Is.EqualTo("Test2"));
    }

    [Test]
    public void CreateInsertInteger()
    {
        var node = SqlParser
            .Parse("INSERT INTO tableName (column1, column2, column3, column4) VALUES (123, -987, 1.23, 2.3e-6)")
            .First();

        Assert.That(node, Is.TypeOf<InsertByTupleNode>());
        Assert.That(((InsertByTupleNode)node).TableName.Values[0], Is.EqualTo("tableName"));
        Assert.That(((InsertByTupleNode)node).Collumns.Count, Is.EqualTo(4));
        Assert.That(((InsertByTupleNode)node).Collumns[0], Is.EqualTo("column1"));
        Assert.That(((InsertByTupleNode)node).Collumns[1], Is.EqualTo("column2"));
        Assert.That(((InsertByTupleNode)node).Collumns[2], Is.EqualTo("column3"));
        Assert.That(((InsertByTupleNode)node).Collumns[3], Is.EqualTo("column4"));
        Assert.That(((InsertByTupleNode)node).Values[0][0], Is.TypeOf<IntegerLiteralNode>());
        Assert.That((((InsertByTupleNode)node).Values[0][0] as IntegerLiteralNode).Value, Is.EqualTo((BigInteger)123));
        Assert.That(((InsertByTupleNode)node).Values[0][1], Is.TypeOf<IntegerLiteralNode>());
        Assert.That((((InsertByTupleNode)node).Values[0][1] as IntegerLiteralNode).Value,
            Is.EqualTo((BigInteger)(-987)));
        Assert.That(((InsertByTupleNode)node).Values[0][2], Is.TypeOf<FloatLiteralNode>());
        Assert.That((((InsertByTupleNode)node).Values[0][2] as FloatLiteralNode).Value,
            Is.EqualTo((double)(1.23)));
        Assert.That(((InsertByTupleNode)node).Values[0][3], Is.TypeOf<FloatLiteralNode>());
        Assert.That((((InsertByTupleNode)node).Values[0][3] as FloatLiteralNode).Value,
            Is.EqualTo((double)(2.3e-6)));
    }

    [Test]
    public void SelectStar()
    {
        var node = SqlParser.Parse(@"SELECT * from abc").First();

        Assert.That(node, Is.TypeOf<SelectNode>());
        Assert.That(((SelectNode)node).Collumns.Count, Is.EqualTo(1));
        Assert.That(((SelectNode)node).Collumns[0].IsSpread, Is.EqualTo(true));
        Assert.That(((SelectNode)node).Collumns[0].Expression, Is.TypeOf<WildcardNode>());
    }
    
    [Test]
    public void SelectSomeColumns()
    {
        var node = SqlParser.Parse(@"SELECT first, second, third from abc").First();

        Assert.That(node, Is.TypeOf<SelectNode>());
        Assert.That(((SelectNode)node).Collumns.Count, Is.EqualTo(3));
        Assert.That(((SelectNode)node).Collumns[0].IsSpread, Is.EqualTo(false));
        Assert.That(((SelectNode)node).Collumns[0].Expression, Is.TypeOf<PathExpressionNode>());
        Assert.That((((SelectNode)node).Collumns[0].Expression as PathExpressionNode).Values, Is.EquivalentTo(new[] { "first" }));
        Assert.That(((SelectNode)node).Collumns[1].IsSpread, Is.EqualTo(false));
        Assert.That(((SelectNode)node).Collumns[1].Expression, Is.TypeOf<PathExpressionNode>());
        Assert.That((((SelectNode)node).Collumns[1].Expression as PathExpressionNode).Values, Is.EquivalentTo(new[] { "second" }));
        Assert.That(((SelectNode)node).Collumns[2].IsSpread, Is.EqualTo(false));
        Assert.That(((SelectNode)node).Collumns[2].Expression, Is.TypeOf<PathExpressionNode>());
        Assert.That((((SelectNode)node).Collumns[2].Expression as PathExpressionNode).Values, Is.EquivalentTo(new[] { "third" }));
    }

    [Test]
    public void SelectNoTable()
    {
        var node = SqlParser.Parse(@"SELECT 'abc', 123, 123 as x").First();

        Assert.That(node, Is.TypeOf<SelectNode>());
        Assert.That(((SelectNode)node).From.Count, Is.EqualTo(0));
        Assert.That(((SelectNode)node).Collumns.Count, Is.EqualTo(3));
        Assert.That(((SelectNode)node).Collumns[0].Alias, Is.EqualTo("'abc'"));
        Assert.That(((SelectNode)node).Collumns[0].Expression, Is.TypeOf<StringLiteralNode>());
        Assert.That((((SelectNode)node).Collumns[0].Expression as StringLiteralNode).Value, Is.EqualTo("abc"));
        Assert.That(((SelectNode)node).Collumns[1].Alias, Is.EqualTo("123"));
        Assert.That(((SelectNode)node).Collumns[1].Expression, Is.TypeOf<IntegerLiteralNode>());
        Assert.That((((SelectNode)node).Collumns[1].Expression as IntegerLiteralNode).Value, Is.EqualTo((BigInteger)123));
        Assert.That(((SelectNode)node).Collumns[2].Alias, Is.EqualTo("x"));
        Assert.That(((SelectNode)node).Collumns[2].Expression, Is.TypeOf<IntegerLiteralNode>());
        Assert.That((((SelectNode)node).Collumns[2].Expression as IntegerLiteralNode).Value, Is.EqualTo((BigInteger)123));
    }
}