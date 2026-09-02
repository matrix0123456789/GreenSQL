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
        runner.ExecuteSqlCommand("CREATE TABLE TestDB.TestTable (a INT)");
        runner.ExecuteSqlCommand("INSERT INTO TestDB.TestTable (a) VALUES (123)");
        var result=runner.ExecuteSqlCommand("SELECT * FROM TestDB.TestTable");
        Assert.That(result.Data.Count, Is.EqualTo(1));
        Assert.That(result.Data.First().Count, Is.EqualTo(1));
        Assert.That(result.Data.First()[0], Is.EqualTo(123));
    }
}