using GreenSQL.Core.Store;
using GreenSQL.Core.Test.Store;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseHttpsRedirection();
var dbSet = new DbSet();

var fileInfo = new FileInfo("./data.greensqld");
StreamWrapper streamWrapper = null;
if (fileInfo.Exists)
{
    var fileStream = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.ReadWrite);
    streamWrapper = new StreamWrapper(fileStream);
    var header = streamWrapper.ReadStringFixedLength(12);
    if (header != "GreenSQL0000")
    {
        fileStream.Close();
        fileInfo.MoveTo("./data_old_" + (DateTime.Now.Ticks) + ".greensqld");
        fileInfo = new FileInfo("./data.greensqld");
        fileStream = new FileStream(fileInfo.FullName, FileMode.CreateNew, FileAccess.ReadWrite);
        streamWrapper = new StreamWrapper(fileStream);
        streamWrapper.WriteStringFixedLength("GreenSQL0000", 12);
    }
}
else
{
    var fileStream = new FileStream(fileInfo.FullName, FileMode.CreateNew, FileAccess.ReadWrite);
    streamWrapper = new StreamWrapper(fileStream);
    streamWrapper.WriteStringFixedLength("GreenSQL0000", 12);
}

var store=new FileStore(dbSet, streamWrapper);
    store.LoadFromStream();
    store.StartListening();

app.MapPost("/_query", async (HttpContext context) =>
{
    var reader = new StreamReader(context.Request.Body);
    var sql = await reader.ReadToEndAsync();
    var runner = new CommandRunner(dbSet);
    var result = runner.ExecuteSqlCommand(sql);
    return Results.Ok(result);
});
app.Map("/",
    () =>
    {
        return Results.Ok(new
        {
            databases = dbSet.DatabaseNames.Select(db => new { name = db, links = new { details = "/" + db } })
                .ToArray()
        });
    });
app.Map("/{dbName}", (string dbName) =>
{
    var db = dbSet.GetDatabase(dbName);
    if (db == null)
    {
        return Results.NotFound();
    }
    else
    {
        return Results.Ok(new
        {
            tables = db.ListTables
        });
    }
});

app.Run();