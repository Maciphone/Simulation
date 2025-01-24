using System.Collections;
using Microsoft.AspNetCore.Mvc;
using StonePaperScissor.Service.Inicialiser;
using StonePaperScissor.Service.Simulation;

namespace StonePaperScissor.Controller;

[ApiController]
[Route("api/[controller]")]
public class SimulationController : ControllerBase
{
    private readonly IInicialiser _simulationInicialiser;
    private readonly ISimulatorService _simulatorService;
   

    public SimulationController(IInicialiser simulationInicialiser, ISimulatorService simulatorService)
    {
        _simulationInicialiser = simulationInicialiser;
        _simulatorService = simulatorService;
        

    }
    
    [HttpPost("start")]
    public IActionResult StartSimulation(int rows, int columns, int itemCount)
    {
        _simulatorService.Simulator = _simulationInicialiser.Inicialise(rows, columns, itemCount);
        _simulatorService.Simulator.PlayOneGame();
      
        if (_simulatorService.Simulator == null)
        {
            return BadRequest("hol van a szimulator");
        }
       
        return Ok("Simulation started!");
    }
    
    [HttpPost("play")]
    public IActionResult PlayOneGame()
    {
        if (_simulatorService.Simulator == null)
        {
            return BadRequest("Simulation has not been started.");
        }

        _simulatorService.Simulator.PlayOneGame(); // Játék egy körének lejátszása
        return Ok("Played one game round.");
    }

}
    