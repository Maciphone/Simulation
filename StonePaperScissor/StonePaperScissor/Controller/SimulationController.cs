using System.Collections;
using Microsoft.AspNetCore.Mvc;

using StonePaperScissor.Service.Initialiser;
using StonePaperScissor.Service.Simulation;

namespace StonePaperScissor.Controller;

[ApiController]
[Route("api/[controller]")]
public class SimulationController : ControllerBase
{
    private readonly IInitialiser _simulationInicialiser;
    private  ISimulator _simulator;
   

    public SimulationController(IInitialiser simulationInicialiser)
    {
        _simulationInicialiser = simulationInicialiser;
        
        

    }
    
    [HttpPost("start")]
    public IActionResult StartSimulation(int rows, int columns, int itemCount)
    {
        _simulator = _simulationInicialiser.Initialise(rows, columns, itemCount);
        _simulator.PlayOneGame();
      
        if (_simulator == null)
        {
            return BadRequest("hol van a szimulator");
        }
       
        return Ok("Simulation started!");
    }
    
    [HttpPost("play")]
    public IActionResult PlayOneGame()
    {
        if (_simulator == null)
        {
            return BadRequest("Simulation has not been started.");
        }

        _simulator.PlayOneGame(); // Játék egy körének lejátszása
        return Ok("Played one game round.");
    }

}
    