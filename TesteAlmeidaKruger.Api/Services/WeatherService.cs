using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text.Json;
using TesteAlmeidaKruger.Api.Domain.Entities;
using TesteAlmeidaKruger.Api.Infrastructure.Context;

namespace TesteAlmeidaKruger.Api.Services;

public class WeatherService
{
    private readonly HttpClient _http;
    private readonly AppDbContext _db;
    private readonly string _apiKey = "f44edd8c2eb71e82f303b4d65036f4cd";

    public WeatherService(HttpClient http, AppDbContext db)
    {
        _http = http;
        _db = db;
    }

    public async Task<WeatherForecast?> GetWeatherAsync(string city)
    {
        try
        {
            var url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&units=metric&appid={_apiKey}";
            var response = await _http.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Erro ao chamar API: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<OpenWeatherResponse>(content);

            if (apiResponse == null) return null;

            var forecast = new WeatherForecast
            {
                City = apiResponse.Name,
                Date = DateTime.Now,
                TemperatureC = (int)apiResponse.Main.Temp,
                Summary = apiResponse.Weather.FirstOrDefault()?.Description
            };

            // Salvar no banco
            _db.WeatherForecasts.Add(forecast);
            await _db.SaveChangesAsync();

            return forecast;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exceção ao consumir API: {ex.Message}");
            return null;
        }
    }
}

// Classes para deserializar o JSON
public class OpenWeatherResponse
{
    public string Name { get; set; } = "";
    public MainInfo Main { get; set; } = new();
    public WeatherInfo[] Weather { get; set; } = Array.Empty<WeatherInfo>();
}

public class MainInfo
{
    public double Temp { get; set; }
}

public class WeatherInfo
{
    public string Description { get; set; } = "";
}
