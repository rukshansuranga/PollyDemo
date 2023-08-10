using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using WeatherService.Models.Incoming;
using WeatherService.Models.Outgoing;

namespace WeatherService.Controllers;

[Route("[controller]")]
public class WeatherController : ControllerBase
{
    private readonly HttpClient _httpClient;

    public WeatherController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    [HttpGet("{locationId}")]
    public async Task<ActionResult> Get(int locationId)
    {
        HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync($"temperature/{locationId}");

        if (httpResponseMessage.IsSuccessStatusCode)
        {
            int temperature = await httpResponseMessage.Content.ReadFromJsonAsync<int>();
            return Ok(temperature);
        }

        return StatusCode((int)httpResponseMessage.StatusCode, "The temperature service returned an error.");
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] WeatherInfo weatherModel)
    {
        var temperatureInfo = new TemperatureInfo
        {
            LocationId = weatherModel.LocationId,
            Temperature = weatherModel.Temperature,
            DateMeasured = weatherModel.DateTemperatureMeasured
        };

        string temperatureJson = JsonSerializer.Serialize(temperatureInfo);
        HttpContent httpContent = new StringContent(temperatureJson, Encoding.UTF8, "application/json");

        var httpResponseMessage = await _httpClient.PostAsync("temperature", httpContent);

        return StatusCode((int)httpResponseMessage.StatusCode);
    }

    [HttpDelete("{locationId}")]
    public async Task<ActionResult> Delete(int locationId)
    {
        HttpResponseMessage httpResponseMessage = await _httpClient.DeleteAsync($"temperature/{locationId}");

        return StatusCode((int)httpResponseMessage.StatusCode);
    }
}