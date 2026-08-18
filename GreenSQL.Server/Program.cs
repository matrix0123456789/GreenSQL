using GreenSQL.Core.Store;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseHttpsRedirection();
var dbSet = new DbSet();
app.MapPost("/_query", async (HttpContext context) =>
{
    var reader = new StreamReader(context.Request.Body);
    var sql = await reader.ReadToEndAsync();
    var runner=new CommandRunner(dbSet);
    var result=runner.ExecuteSqlCommand(sql);
    return Results.Ok(result);

});
app.Map("/", () =>
{
    return Results.Ok(new {databases=dbSet.DatabaseNames.Select(db => new { name = db, links = new { details = "/" + db } }).ToArray()});
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
