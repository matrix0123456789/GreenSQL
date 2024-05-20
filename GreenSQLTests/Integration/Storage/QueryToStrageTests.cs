using GreenSQL.Data.Storage;

namespace GreenSQLTests.Integration.Storage;

public class QueryToStrageTests
{
    [Test]
    public void InitServer()
    {
        var tmpDir = Path.GetTempPath()+Guid.NewGuid();
        var storage = new MultiFileStorage(tmpDir);
        storage.Load();
        Assert.True(new DirectoryInfo(tmpDir).Exists);
        Assert.That(storage.Server.Databases.Count, Is.EqualTo(0));
        
    }

    [Test]
    public void CreateDatabase()
    {
        var tmpDir = Path.GetTempPath()+Guid.NewGuid();
        using (var storage = new MultiFileStorage(tmpDir))
        {
            storage.Load();

            var context = storage.Server.CreateNewContext();

            context.Execute("CREATE DATABASE test");
            
            
            Assert.That(storage.Server.Databases.Count, Is.EqualTo(1));
            Assert.That(storage.Server.Databases[0].Name, Is.EqualTo("test"));
        }

        
        Assert.True(new DirectoryInfo(tmpDir+"/db_test").Exists);

        using (var storage = new MultiFileStorage(tmpDir))
        {
            storage.Load();
            Assert.That(storage.Server.Databases.Count, Is.EqualTo(1));
            Assert.That(storage.Server.Databases[0].Name, Is.EqualTo("test"));
        }
    }

    [Test]
    public void CreateEmptyTable()
    {
        var tmpDir = Path.GetTempPath()+Guid.NewGuid();
        using (var storage = new MultiFileStorage(tmpDir))
        {
            storage.Load();
            var context = storage.Server.CreateNewContext();

            context.Execute("CREATE DATABASE test");
            context.Execute("CREATE TABLE test.test_table (id INT PRIMARY KEY)");
            
            
            Assert.That(storage.Server.GetDatabase("test").Tables.Count, Is.EqualTo(1));
            Assert.That(storage.Server.GetDatabase("test").Tables[0].Name, Is.EqualTo("test_table"));
        }

        
        Assert.True(new FileInfo(tmpDir+"/db_test/test_table.dbtable").Exists);

        using (var storage = new MultiFileStorage(tmpDir))
        {
            storage.Load();
            Assert.That(storage.Server.GetDatabase("test").Tables.Count, Is.EqualTo(1));
            Assert.That(storage.Server.GetDatabase("test").Tables[0].Name, Is.EqualTo("test_table"));
        }
    }
}