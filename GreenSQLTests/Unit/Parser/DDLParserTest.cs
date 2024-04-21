using GreenSQL.Parser;
using GreenSQL.SqlNodes.Statement;

namespace GreenSQLTests.Unit.Parser;

public class DDLParserTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void CreateTable1()
    {
        var sql = @"CREATE TABLE employees (
    id            INT32       PRIMARY KEY,
    first_name    TEXT        not null,
    last_name     TEXT        not null,
    mid_name      TEXT        not null,
    dateofbirth   DATE        not null
)";
        var obj = new QueryParser(sql).Parse();
        Assert.AreEqual(obj.GetType(), typeof(CreateTable));
        Assert.AreEqual(((CreateTable)obj).TableName, "employees");
        Assert.AreEqual(((CreateTable)obj).Columns.Count, 5);
        Assert.AreEqual(((CreateTable)obj).Columns[0].Name, "id");
        Assert.AreEqual(((CreateTable)obj).Columns[0].Type, "INT32");
        Assert.AreEqual(((CreateTable)obj).Columns[0].IsNotNull, false);
        Assert.AreEqual(((CreateTable)obj).Columns[1].Name, "first_name");
        Assert.AreEqual(((CreateTable)obj).Columns[1].Type, "TEXT");
        Assert.AreEqual(((CreateTable)obj).Columns[1].IsNotNull, true);
        Assert.AreEqual(((CreateTable)obj).Columns[2].Name, "last_name");
        Assert.AreEqual(((CreateTable)obj).Columns[2].Type, "TEXT");
        Assert.AreEqual(((CreateTable)obj).Columns[2].IsNotNull, true);
        Assert.AreEqual(((CreateTable)obj).Columns[3].Name, "mid_name");
        Assert.AreEqual(((CreateTable)obj).Columns[3].Type, "TEXT");
        Assert.AreEqual(((CreateTable)obj).Columns[3].IsNotNull, true);
        Assert.AreEqual(((CreateTable)obj).Columns[4].Name, "dateofbirth");
        Assert.AreEqual(((CreateTable)obj).Columns[4].Type, "DATE");
        Assert.AreEqual(((CreateTable)obj).Columns[4].IsNotNull, true);
        Assert.AreEqual(((CreateTable)obj).Indexes[0].GetType(), typeof(PrimaryKey));
        Assert.AreEqual(((PrimaryKey)((CreateTable)obj).Indexes[0]).Columns[0], "id");
        
    }

    [Test]
    public void CreateTable2()
    {
        var sql = @"CREATE TABLE `cities` (
    `id` INT32,
name TEXT,
population INT64 null,
    PRIMARY KEY (`id`),
INDEX `name` (`name`)";
        var obj = new QueryParser(sql).Parse();
        Assert.AreEqual(obj.GetType(), typeof(CreateTable));
        Assert.AreEqual(((CreateTable)obj).TableName, "cities");
        Assert.AreEqual(((CreateTable)obj).Columns.Count, 3);
        Assert.AreEqual(((CreateTable)obj).Columns[0].Name, "id");
        Assert.AreEqual(((CreateTable)obj).Columns[0].Type, "INT32");
        Assert.AreEqual(((CreateTable)obj).Columns[0].IsNotNull, false);
        Assert.AreEqual(((CreateTable)obj).Columns[1].Name, "name");
        Assert.AreEqual(((CreateTable)obj).Columns[1].Type, "TEXT");
        Assert.AreEqual(((CreateTable)obj).Columns[1].IsNotNull, false);
        Assert.AreEqual(((CreateTable)obj).Columns[2].Name, "population");
        Assert.AreEqual(((CreateTable)obj).Columns[2].Type, "INT64");
        Assert.AreEqual(((CreateTable)obj).Columns[2].IsNotNull, false);
        Assert.AreEqual(((CreateTable)obj).Indexes[0].GetType(), typeof(PrimaryKey));
        Assert.AreEqual(((PrimaryKey)((CreateTable)obj).Indexes[0]).Columns[0], "id");
        Assert.AreEqual(((CreateTable)obj).Indexes[1].GetType(), typeof(Index));
        Assert.AreEqual(((Index)((CreateTable)obj).Indexes[1]).Columns[0], "name");
        
    }
    [Test]
    public void CreateDatabase1()
    {
        var sql = @"CREATE DATABASE test";
        var obj = new QueryParser(sql).Parse();
        Assert.AreEqual(obj.GetType(), typeof(CreateDatabase));
        Assert.AreEqual(((CreateDatabase)obj).DatabaseName, "test");
    }

    [Test]
    public void CreateDatabase2()
    {
        var sql = @"CREATE SCHEMA `test`";
        var obj = new QueryParser(sql).Parse();
        Assert.AreEqual(obj.GetType(), typeof(CreateDatabase));
        Assert.AreEqual(((CreateDatabase)obj).DatabaseName, "test");
    }
    [Test]
    public void DropDatabase1()
    {
        var sql = @"DROP DATABASE test";
        var obj = new QueryParser(sql).Parse();
        Assert.AreEqual(obj.GetType(), typeof(DropDatabase));
        Assert.AreEqual(((DropDatabase)obj).DatabaseName, "test");
    }
    [Test]
    public void DropTable1()
    {
        var sql = @"DROP TABLE test";
        var obj = new QueryParser(sql).Parse();
        Assert.AreEqual(obj.GetType(), typeof(DropTable));
        Assert.AreEqual(((DropTable)obj).TableName, "test");
    }
    [Test]
    public void DropTable2()
    {
        var sql = @"DROP TABLE `test`";
        var obj = new QueryParser(sql).Parse();
        Assert.AreEqual(obj.GetType(), typeof(DropTable));
        Assert.AreEqual(((DropTable)obj).TableName, "test");
    }
    [Test]
    public void DropTable3()
    {
        var sql = @"DROP TABLE `test`.`test`";
        var obj = new QueryParser(sql).Parse();
        Assert.AreEqual(obj.GetType(), typeof(DropTable));
        Assert.AreEqual(((DropTable)obj).TableName, "test.test");
    }
    [Test]
    public void DropDatabase2()
    {
        var sql = @"DROP DATABASE `test`";
        var obj = new QueryParser(sql).Parse();
        Assert.AreEqual(obj.GetType(), typeof(DropDatabase));
        Assert.AreEqual(((DropDatabase)obj).DatabaseName, "test");
    }
    [Test]
    public void DropDatabase3()
    {
        var sql = @"DROP SCHEMA `test`";
        var obj = new QueryParser(sql).Parse();
        Assert.AreEqual(obj.GetType(), typeof(DropDatabase));
        Assert.AreEqual(((DropDatabase)obj).DatabaseName, "test");
    }
    [Test]
    public void DropDatabase4()
    {
        var sql = @"DROP SCHEMA test";
        var obj = new QueryParser(sql).Parse();
        Assert.AreEqual(obj.GetType(), typeof(DropDatabase));
        Assert.AreEqual(((DropDatabase)obj).DatabaseName, "test");
    }
    [Test]
    public void TruncateTable1()
    {
        var sql = @"TRUNCATE TABLE test";
        var obj = new QueryParser(sql).Parse();
        Assert.AreEqual(obj.GetType(), typeof(TruncateTable));
        Assert.AreEqual(((TruncateTable)obj).TableName, "test");
    }
    public void AlterTable1()
    {
        var sql = @"ALTER TABLE test ADD COLUMN name TEXT";
        var obj = new QueryParser(sql).Parse();
        Assert.AreEqual(obj.GetType(), typeof(AlterTable));
        Assert.AreEqual(((AlterTable)obj).TableName, "test");
        Assert.AreEqual(((AlterTable)obj).AddColumns[0].Name, "name");
        Assert.AreEqual(((AlterTable)obj).AddColumns[0].Type, "TEXT");
        
    }
    public void AlterTable2()
    {
        var sql = @"ALTER TABLE test DROP COLUMN name";
        var obj = new QueryParser(sql).Parse();
        Assert.AreEqual(obj.GetType(), typeof(AlterTable));
        Assert.AreEqual(((AlterTable)obj).TableName, "test");
        Assert.AreEqual(((AlterTable)obj).DropColumns[0], "name");
        
    }
    public void AlterTable3()
    {
        var sql = @"ALTER TABLE test ADD COLUMN name TEXT, DROP COLUMN age";
        var obj = new QueryParser(sql).Parse();
        Assert.AreEqual(obj.GetType(), typeof(AlterTable));
        Assert.AreEqual(((AlterTable)obj).TableName, "test");
        Assert.AreEqual(((AlterTable)obj).AddColumns[0].Name, "name");
        Assert.AreEqual(((AlterTable)obj).AddColumns[0].Type, "TEXT");
        Assert.AreEqual(((AlterTable)obj).DropColumns[0], "age");
        
    }
    
}