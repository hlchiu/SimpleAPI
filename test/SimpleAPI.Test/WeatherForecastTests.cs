using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Xunit;

namespace SimpleAPI.Test
{
    public class WeatherForecastTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public WeatherForecastTests()
        {
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(b => b.UseEnvironment("Testing")); 
        }   

        [Fact]
        public async Task GetWeatherForecast_ShouldReturnOkAndJson()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/weatherforecast");

            // Assert - 狀態碼 OK
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // 讀取 JSON
            var json = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrEmpty(json)); // JSON 不應該是 null 或空字串

            // 反序列化檢查
            var forecasts = JsonSerializer.Deserialize<List<WeatherForecastDto>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(forecasts);
            Assert.NotEmpty(forecasts);                // 至少要有一筆資料
            Assert.False(string.IsNullOrEmpty(forecasts![0].Summary));
        }

        private class WeatherForecastDto
        {
            public DateOnly Date { get; set; }
            public int TemperatureC { get; set; }
            public string? Summary { get; set; }
            public int TemperatureF { get; set; }
        }
    }
}
