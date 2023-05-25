using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NodesTestJob.Models;
using System.Data.Common;
using System.Reflection.Metadata;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddTransient<DataCreator>();
builder.Services.AddTransient<DataManager>();
var app = builder.Build();

app.Use(async (context, next) =>
{
    var dataCreator = context.RequestServices.GetRequiredService<DataCreator>();
    var data = dataCreator.Create();

    await next();
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.MapMethods("/api/notes/check", new[] { "GET" }, async (context) =>
{
    var dataManager = new DataManager(context.RequestServices.GetRequiredService<IConfiguration>());
    string output = dataManager.TriggerCheck(QuerySQL.GetCheckNotes);
    if (output.Contains("Ошибка")) { context.Response.StatusCode = 500; }
    else { context.Response.StatusCode = 200; }
    await context.Response.WriteAsync(output);
});

app.MapMethods("/api/notes/", new[] { "POST" }, async (context) =>
{
    var dataManager = new DataManager(context.RequestServices.GetRequiredService<IConfiguration>());

    var noteTitle = context.Request.Form["noteTitle"].ToString();
    var noteText = context.Request.Form["noteText"].ToString();

    if (dataManager.Insert(QuerySQL.InsertNote, new SqlParameter("@Title", noteTitle), new SqlParameter("@Text", noteText), new SqlParameter("@CreatedDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")), new SqlParameter("@ChangeDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")), new SqlParameter("@IsChecked", false)) is string errorMessage)
    {
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync(errorMessage);
    }
    else
    {
        context.Response.StatusCode = 200;
        await context.Response.WriteAsync("Заметка добавлена!");
    }
});

app.MapMethods("/api/notes", new[] { "PUT" }, async (context) =>
{
    var dataManager = new DataManager(context.RequestServices.GetRequiredService<IConfiguration>());

    var noteId = context.Request.Form["noteId"].ToString();
    var noteEl = context.Request.Form["noteEl"].ToString();
    var noteText = context.Request.Form["noteText"].ToString();

    if (dataManager.Update(noteEl == "Title" ? QuerySQL.UpdateNoteTitle : QuerySQL.UpdateNoteText, new SqlParameter("@noteId", noteId), new SqlParameter("@noteEl", noteEl), new SqlParameter("@noteText", noteText), new SqlParameter("@changeDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))) is string errorMessage)
    {
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync(errorMessage);
    }
    else
    {
        context.Response.StatusCode = 200;
        await context.Response.WriteAsync(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
    }
});

app.MapMethods("/api/notes/check", new[] { "PUT" }, async (context) =>
{
    var dataManager = new DataManager(context.RequestServices.GetRequiredService<IConfiguration>());

    var noteId = context.Request.Form["noteId"].ToString();

    if (dataManager.Update(QuerySQL.UpdateNoteCheck, new SqlParameter("@noteId", noteId), new SqlParameter("@changeDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))) is string errorMessage)
    {
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync(errorMessage);
    }
    else
    {
        context.Response.StatusCode = 200;
        await context.Response.WriteAsync(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
    }
});

app.MapMethods("/api/notes", new[] { "DELETE" }, async (context) =>
{
    var dataManager = new DataManager(context.RequestServices.GetRequiredService<IConfiguration>());

    var noteId = context.Request.Form["noteId"].ToString();

    if (dataManager.Delete(QuerySQL.DeleteNote, new SqlParameter("@noteId", noteId)) is string errorMessage)
    {
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync(errorMessage);
    }
    else
    {
        context.Response.StatusCode = 200;
        await context.Response.WriteAsync("Заметка удалена!");
    }
});

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
