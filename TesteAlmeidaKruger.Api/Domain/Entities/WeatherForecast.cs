namespace TesteAlmeidaKruger.Api.Domain.Entities
{
    public class WeatherForecast
    {
        public int Id { get; set; }
        public string City { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public int TemperatureC { get; set; }
        public string? Summary { get; set; }
    }
}
