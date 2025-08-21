using Microsoft.EntityFrameworkCore;
using TesteAlmeidaKruger.Api.Domain.Entities;

namespace TesteAlmeidaKruger.Api.Infrastructure.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<WeatherForecast> WeatherForecasts { get; set; }
    }
}
