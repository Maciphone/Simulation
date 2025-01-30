using System.Collections;
using Microsoft.AspNetCore.Mvc;
using StonePaperScissor.Service.Simulation;
using StonePaperScissor.Service.Simulation.SimulationServices.Interfaces;

namespace StonePaperScissor.Controller;

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
    public IActionResult StartSimulation(int rows, int columns, int itemCount)
    {
        var simulationId = _simulatorService.InitialiseSimulation(rows, columns, itemCount);
        
        return Ok(new { SimulationId = simulationId });
        

        
    }
    
    [HttpPost("play")]
    public IActionResult PlayOneGame(string simulationId)
    {
       
       _simulatorService.StartSimulation(simulationId);
       return Ok();
    }
    
    [HttpPost("pause")]
    public IActionResult StopGame(string simulationId)
    {
        _simulatorService.PauseSimulation(simulationId);
        return Ok();
    }
    
    [HttpPost("resume")]
    public IActionResult ResumeGame(string simulationId)
    {
        _simulatorService.ResumeSimulation( simulationId);
        return Ok();
    }
    

}
    