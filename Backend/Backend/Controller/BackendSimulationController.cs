using Backend.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controller;


    
[ApiController]
[Route("api/[controller]")]
public class BackendSimulationController : ControllerBase
{ 
    private readonly HttpClient _httpClient;
    private string _simulationApiUrl;

    public BackendSimulationController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClient = httpClientFactory.CreateClient("SimulationApiClient");
        _simulationApiUrl = configuration["SimulationApi:BaseUrl"];
        if (_simulationApiUrl == "MISSING_URL")
        {
            throw new Exception("❌ Simulation API BaseUrl is missing in configuration!");
        }

        Console.WriteLine($"✅ Simulation API Base URL betöltve: {_simulationApiUrl}");
    }


    
    [HttpPost("initialize")]
    public async Task<IActionResult> InitializeSimulation([FromBody] InitializeSimulationRequest request)
    {
        Console.WriteLine(_simulationApiUrl);
        var response = await _httpClient.PostAsJsonAsync($"http://localhost:5050/api/Simulation/initialise", request);
        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        return Ok(await response.Content.ReadAsStringAsync());
    }

   
    [HttpPost("loadSaved")]
    public async Task<IActionResult> LoadSavedSimulation([FromBody] LoadSavedSimulationRequest request)
    {
        // Backend API nem alakítja át az ItemString-et, csak továbbítja!
        var response = await _httpClient.PostAsJsonAsync($"{_simulationApiUrl}/reloadSavedSimulation", request);
        //should return simulationId
        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        
        return Ok(await response.Content.ReadAsStringAsync());
    }

    
    [HttpPost("play")]
    public async Task<IActionResult> PlaySimulation([FromBody] SimulationIdRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"{_simulationApiUrl}/play", request);
        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        return Ok();
    }

   
    [HttpPost("pause")]
    public async Task<IActionResult> PauseSimulation([FromBody] SimulationIdRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"{_simulationApiUrl}/pause", request);
        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        return Ok();
    }

   
    [HttpPost("resume")]
    public async Task<IActionResult> ResumeSimulation([FromBody] SimulationIdRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"{_simulationApiUrl}/resume", request);
        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        return Ok();
    }

}