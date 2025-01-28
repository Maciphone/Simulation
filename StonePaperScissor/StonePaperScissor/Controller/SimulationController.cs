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
        if (_simulatorService.InitialiseSimulation(rows, columns, itemCount))
        {
            return Ok();
        }

        return BadRequest("game could not initialised");
    }
    
    [HttpPost("play")]
    public IActionResult PlayOneGame()
    {
       _simulatorService.StartSimulation();
       return Ok();
    }
    
    [HttpPost("pause")]
    public IActionResult StopGame()
    {
        _simulatorService.StopSimulation();
        return Ok();
    }
    
    [HttpPost("resume")]
    public IActionResult ResumeGame()
    {
        _simulatorService.ResumeGame();
        return Ok();
    }
    

}
    