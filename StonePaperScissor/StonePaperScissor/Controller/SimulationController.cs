using System.Collections;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StonePaperScissor.Dto;
using StonePaperScissor.Service.Simulation;
using StonePaperScissor.Service.Simulation.SimulationServices.Interfaces;

namespace StonePaperScissor.Controller;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SimulationController : ControllerBase
{
    private readonly ISimulatorService _simulatorService;

    public SimulationController(ISimulatorService simulatorService)
    {
        _simulatorService = simulatorService;
    }

    [HttpPost("initialise")]
    public IActionResult StartSimulation([FromBody] InitializeSimulationRequest request)
    {
        var simulationId = _simulatorService.InitialiseSimulation(request.Rows, request.Columns, request.ItemCount);
        
        return Ok(new { SimulationId = simulationId });
        
    }
    
    [HttpPost("reloadSavedSimulation")]
    public IActionResult StartSavedSimulation([FromBody] LoadSavedSimulationRequest request)
    {
        //
        var items =JsonSerializer.Deserialize<List<Item>>(request.SavedItems);
        
        if (items == null)
        {
            return BadRequest(new { message = "Invalid simulation state data" });
        }
        var simulationId = _simulatorService.InitialSavedSimulation(request.Rows, request.Columns, items);
        
        return Ok(new { SimulationId = simulationId });
        
    }
    
    [HttpPost("play")]
    public IActionResult PlayOneGame([FromBody] SimulationIdRequest simulationIdRequest)
    {
       
       _simulatorService.StartSimulation(simulationIdRequest.SimulationId);
       return Ok();
    }
    
    [HttpPost("pause")]
    public IActionResult StopGame([FromBody] SimulationIdRequest simulationIdRequest)
    {
        _simulatorService.PauseSimulation(simulationIdRequest.SimulationId);
        return Ok();
    }
    
    [HttpPost("resume")]
    public IActionResult ResumeGame([FromBody] SimulationIdRequest simulationIdRequest)
    {
        _simulatorService.ResumeSimulation( simulationIdRequest.SimulationId);
        return Ok();
    }
    

}
    