using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using TesteAlmeidaKruger.Api.Domain.Entities;
using TesteAlmeidaKruger.Api.Infrastructure.Context;
using TesteAlmeidaKruger.Api.Services;
using TesteAlmeidaKruger.Api.Background;

var builder = WebApplication.CreateSlimBuilder(args);

// Configurar SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=weatherdash.db"));

// Registrar HttpClient e Worker
builder.Services.AddHttpClient();
builder.Services.AddHostedService<WeatherUpdateWorker>();

var app = builder.Build();

// Serve arquivos estáticos da pasta wwwroot
app.UseDefaultFiles();
app.UseStaticFiles();

// Grupo para API de clima
var weatherApi = app.MapGroup("/weather");

// GET - lista todas previsões do banco
weatherApi.MapGet("/", async (AppDbContext db) =>
    await db.WeatherForecasts.ToListAsync());

// GET por id
weatherApi.MapGet("/{id}", async (int id, AppDbContext db) =>
    await db.WeatherForecasts.FindAsync(id) is WeatherForecast wf
        ? Results.Ok(wf)
        : Results.NotFound()); 

// POST - adiciona nova previsão
weatherApi.MapPost("/", async (WeatherForecast wf, AppDbContext db) =>
{
    db.WeatherForecasts.Add(wf);
    await db.SaveChangesAsync();
    return Results.Created($"/weather/{wf.Id}", wf);
});

// DELETE - remove por id
weatherApi.MapDelete("/{id}", async (int id, AppDbContext db) =>
{
    var wf = await db.WeatherForecasts.FindAsync(id);
    if (wf is null) return Results.NotFound();

    db.WeatherForecasts.Remove(wf);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();

[JsonSerializable(typeof(WeatherForecast[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}
