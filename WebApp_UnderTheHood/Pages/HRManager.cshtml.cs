using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using WebApp_UnderTheHood.Authorization;
using WebApp_UnderTheHood.Authorization.DTO;

namespace WebApp_UnderTheHood.Pages;

[Authorize(Policy = "HRManagerOnly")]
public class HRManagerModel : PageModel
{
    private readonly IHttpClientFactory httpClientFactory;
    [BindProperty]
    public List<WeatherForecastDTO> weatherForecastItems { get; set; } = new List<WeatherForecastDTO>();

    public HRManagerModel(IHttpClientFactory httpClientFactory)
    {
        this.httpClientFactory = httpClientFactory;
    }

    public async Task OnGet()
    {
        var httpClient = httpClientFactory.CreateClient("OurWebAPI");
        // post request to /auth to get token
        var res = await httpClient.PostAsJsonAsync("auth", new Authorization.Credential { UserName = "admin", Password = "password" });
        res.EnsureSuccessStatusCode();
        string jwt = await res.Content.ReadAsStringAsync();
        var token = JsonConvert.DeserializeObject<JwtToken>(jwt);  // convert jwt token

        // simple HTTP call - pass in DTO type 
        // weatherForecastItems = await httpClient.GetFromJsonAsync<List<WeatherForecastDTO>>("WeatherForecast") ?? new List<WeatherForecastDTO>();
        // the http call may return null 
        // use ?? to correct for the nullalble referenc error
        // if result is null => we initialize an empty list


        // HTTP call w jwt token to http header
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token?.AccessToken ?? string.Empty);
        weatherForecastItems = await httpClient.GetFromJsonAsync<List<WeatherForecastDTO>>("WeatherForecast") ?? new List<WeatherForecastDTO>();
    }
}

