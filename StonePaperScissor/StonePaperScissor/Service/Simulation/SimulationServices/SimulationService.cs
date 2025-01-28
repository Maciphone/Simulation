using StonePaperScissor.Service.Simulation.SimulationServices.Interfaces;

namespace StonePaperScissor.Service.Simulation.SimulationServices;

public class SimulationService : ISimulatorService
{
    private readonly IInitialiser _initialiser;
    private ISimulator _simulator;
    

    public SimulationService(IInitialiser initialiser)
    {
        _initialiser = initialiser;
        
    }

    public bool InitialiseSimulation(int row, int columns, int itemCount)
    {
        _simulator = _initialiser.Initialise(row, columns, itemCount);
        if (_simulator != null)
        {
           
            return true;
        }

        return false;
    }

    public void StartSimulation()
    {
        if (_simulator == null)
        {
            throw new InvalidOperationException("Simulation has not been initialized. Call InitialiseSimulation() first.");
        }

        _simulator.PlayOneGame();
    }

    public void StopSimulation()
    {
        _simulator.StopGame();
    }

    public void ResumeGame()
    {
        _simulator.Resume();
    }
}