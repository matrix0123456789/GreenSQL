using GreenSQL.Core.Store;

namespace GreenSQL.Core.Test.Store;

[TestFixture]
public class CommandRunnerTest
{
    [Test]
    public void InsertAndSelect()
    {
        var runner = new CommandRunner(new DbSet());
        runner.ExecuteSqlCommand("CREATE DATABASE TestDB");
        runner.ExecuteSqlCommand("CREATE TABLE TestDB.TestTable (a TEXT)");
        runner.ExecuteSqlCommand("INSERT INTO TestDB.TestTable (a) VALUES ('Hello, world!')");
        var result=runner.ExecuteSqlCommand("SELECT * FROM TestDB.TestTable");
        Assert.That(result.Data.Count, Is.EqualTo(1));
        Assert.That(result.Data.First().Count, Is.EqualTo(1));
        Assert.That(result.Data.First()[0], Is.EqualTo("Hello, world!"));
    }
    
    [Test]
    public void InsertAndSelectManyTypes()
    {
        var runner = new CommandRunner(new DbSet());
        runner.ExecuteSqlCommand("CREATE DATABASE TestDB");
        runner.ExecuteSqlCommand("CREATE TABLE TestDB.TestTable (a INT, b FLOAT, c FLOAT)");
        runner.ExecuteSqlCommand("INSERT INTO TestDB.TestTable (a, b, c) VALUES (123, 777, 1.2e+3)");
        var result=runner.ExecuteSqlCommand("SELECT * FROM TestDB.TestTable");
        Assert.That(result.Data.Count, Is.EqualTo(1));
        Assert.That(result.Data.First().Count, Is.EqualTo(3));
        Assert.That(result.Data.First()[0], Is.EqualTo(123));
        Assert.That(result.Data.First()[1], Is.EqualTo(777.0));
        Assert.That(result.Data.First()[2], Is.EqualTo(1200.0));
    }
}