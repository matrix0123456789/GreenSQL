using System.Numerics;
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
        var result = runner.ExecuteSqlCommand("SELECT * FROM TestDB.TestTable");
        Assert.That(result.Data.Count, Is.EqualTo(1));
        Assert.That(result.Data.First().Count, Is.EqualTo(1));
        Assert.That(result.Data.First()[0], Is.EqualTo("Hello, world!"));
    }

    [Test]
    public void InsertAndSelectManyTypes()
    {
        var runner = new CommandRunner(new DbSet());
        runner.ExecuteSqlCommand("CREATE DATABASE TestDB");
        runner.ExecuteSqlCommand("CREATE TABLE TestDB.TestTable (a INT, b FLOAT, c FLOAT, d DATE, e TIME, f DATETIME)");
        runner.ExecuteSqlCommand(
            "INSERT INTO TestDB.TestTable (a, b, c, d, e, f) VALUES (123, 777, 1.2e+3, '2027-01-01','21:37', '2027-01-01 21:37:01')");
        var result = runner.ExecuteSqlCommand("SELECT * FROM TestDB.TestTable");
        Assert.That(result.Data.Count, Is.EqualTo(1));
        Assert.That(result.Data.First().Count, Is.EqualTo(6));
        Assert.That(result.Data.First()[0], Is.EqualTo(123));
        Assert.That(result.Data.First()[1], Is.EqualTo(777.0));
        Assert.That(result.Data.First()[2], Is.EqualTo(1200.0));
        Assert.That(result.Data.First()[3], Is.EqualTo(new DateOnly(2027, 1, 1)));
        Assert.That(((TimeOnly)result.Data.First()[4]).Ticks, Is.EqualTo(new TimeSpan(21, 37, 0).Ticks));
        Assert.That(result.Data.First()[5], Is.EqualTo(new DateTime(2027, 1, 1, 21, 37, 1)));
    }

    [Test]
    public void SelectWithoutTable()
    {
        var runner = new CommandRunner(new DbSet());
        var result = runner.ExecuteSqlCommand("SELECT 123, 'abc', 2+2");
        Assert.That(result.Data.Count, Is.EqualTo(1));
        Assert.That(result.Data.First().Count, Is.EqualTo(3));
        Assert.That(result.Data.First()[0], Is.EqualTo((BigInteger)123));
        Assert.That(result.Data.First()[1], Is.EqualTo("abc"));
        Assert.That(result.Data.First()[2], Is.EqualTo((BigInteger)4));
    }

    [Test]
    public void SelectSomeColumns()
    {
        var runner = new CommandRunner(new DbSet());
        runner.ExecuteSqlCommand("CREATE DATABASE TestDB");
        runner.ExecuteSqlCommand("CREATE TABLE TestDB.TestTable (a INT, b INT, c INT)");
        runner.ExecuteSqlCommand("INSERT INTO TestDB.TestTable (a, b, c) VALUES (1, 2, 3)");
        runner.ExecuteSqlCommand("INSERT INTO TestDB.TestTable (a, b, c) VALUES (100, 200, 300)");
        var result = runner.ExecuteSqlCommand("SELECT c, b FROM TestDB.TestTable");
        Assert.That(result.Data.Count, Is.EqualTo(2));
        Assert.That(result.Data.First().Count, Is.EqualTo(2));
        Assert.That(result.Data.First()[0], Is.EqualTo(3));
        Assert.That(result.Data.First()[1], Is.EqualTo(2));
        Assert.That(result.Data.Last()[0], Is.EqualTo(300));
        Assert.That(result.Data.Last()[1], Is.EqualTo(200));
    }
    [Test]
    public void SelectModifiedData()
    {
        var runner = new CommandRunner(new DbSet());
        runner.ExecuteSqlCommand("CREATE DATABASE TestDB");
        runner.ExecuteSqlCommand("CREATE TABLE TestDB.TestTable (a INT, b INT)");
        runner.ExecuteSqlCommand("INSERT INTO TestDB.TestTable (a, b) VALUES (2, 2)");
        runner.ExecuteSqlCommand("INSERT INTO TestDB.TestTable (a, b) VALUES (100, 23)");
        var result = runner.ExecuteSqlCommand("SELECT a, b, a+b FROM TestDB.TestTable");
        Assert.That(result.Data.Count, Is.EqualTo(2));
        Assert.That(result.Data.First().Count, Is.EqualTo(3));
        Assert.That(result.Data.First()[0], Is.EqualTo(2));
        Assert.That(result.Data.First()[1], Is.EqualTo(2));
        Assert.That(result.Data.First()[2], Is.EqualTo(4));
        Assert.That(result.Data.Last()[0], Is.EqualTo(100));
        Assert.That(result.Data.Last()[1], Is.EqualTo(23));
        Assert.That(result.Data.Last()[2], Is.EqualTo(123));
    }

    [Test]
    public void SelectMultipleTables()
    {
        var runner = new CommandRunner(new DbSet());
        runner.ExecuteSqlCommand("CREATE DATABASE TestDB");
        runner.ExecuteSqlCommand("CREATE TABLE TestDB.T1 (a INT)");
        runner.ExecuteSqlCommand("CREATE TABLE TestDB.T2 (a INT)");
        runner.ExecuteSqlCommand("CREATE TABLE TestDB.T3 (a INT)");
        runner.ExecuteSqlCommand("INSERT INTO TestDB.T1 (a) VALUES (1)");
        runner.ExecuteSqlCommand("INSERT INTO TestDB.T1 (a) VALUES (2)");
        runner.ExecuteSqlCommand("INSERT INTO TestDB.T1 (a) VALUES (3)");
        runner.ExecuteSqlCommand("INSERT INTO TestDB.T2 (a) VALUES (10)");
        runner.ExecuteSqlCommand("INSERT INTO TestDB.T2 (a) VALUES (20)");
        runner.ExecuteSqlCommand("INSERT INTO TestDB.T2 (a) VALUES (30)");
        runner.ExecuteSqlCommand("INSERT INTO TestDB.T3 (a) VALUES (100)");
        runner.ExecuteSqlCommand("INSERT INTO TestDB.T3 (a) VALUES (200)");
        runner.ExecuteSqlCommand("INSERT INTO TestDB.T3 (a) VALUES (300)");
        
        var result = runner.ExecuteSqlCommand("SELECT * FROM TestDB.T1, TestDB.T2, TestDB.T3");
        Assert.That(result.Data.Count, Is.EqualTo(27));
        Assert.That(result.Data.ToArray()[0], Is.EquivalentTo(new object[] { 1, 10, 100 }));
        Assert.That(result.Data.ToArray()[1], Is.EquivalentTo(new object[] { 1, 10, 200 }));
        Assert.That(result.Data.ToArray()[2], Is.EquivalentTo(new object[] { 1, 10, 300 }));
        Assert.That(result.Data.ToArray()[3], Is.EquivalentTo(new object[] { 1, 20, 100 }));
        Assert.That(result.Data.ToArray()[4], Is.EquivalentTo(new object[] { 1, 20, 200 }));
        Assert.That(result.Data.ToArray()[5], Is.EquivalentTo(new object[] { 1, 20, 300 }));
    }
}