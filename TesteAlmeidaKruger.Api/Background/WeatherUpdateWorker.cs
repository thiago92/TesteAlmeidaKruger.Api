using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TesteAlmeidaKruger.Api.Infrastructure.Context;
using TesteAlmeidaKruger.Api.Services;

namespace TesteAlmeidaKruger.Api.Background;

public class WeatherUpdateWorker : BackgroundService
{
    private readonly IServiceProvider _provider;
    private readonly string[] _cities = new string[]
    {
        "São Paulo", "Rio de Janeiro", "Curitiba", "Belo Horizonte", "Porto Alegre"
    };

    public WeatherUpdateWorker(IServiceProvider provider)
    {
        _provider = provider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _provider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var httpClient = scope.ServiceProvider.GetRequiredService<HttpClient>();
            var service = new WeatherService(httpClient, db);

            foreach (var city in _cities)
            {
                await service.GetWeatherAsync(city);
            }

            await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
        }
    }
}
