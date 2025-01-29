using StonePaperScissor.Service.Simulation.SimulationServices.Interfaces;

namespace StonePaperScissor.Service.Simulation.SimulationServices;

public class SimulationService : ISimulatorService
{
    private readonly IInitialiser _initialiser;
    private ISimulator _simulator;
    private ISimulationStorage _simulationStorage;
    private int count;
    

    public SimulationService(IInitialiser initialiser, ISimulationStorage simulationStorage)
    {
        _initialiser = initialiser;
        _simulationStorage = simulationStorage;
    }

    public string InitialiseSimulation(int row, int columns, int itemCount)
    {

        var simulationId = Guid.NewGuid().ToString();
        var simulator = _initialiser.Initialise(row, columns, itemCount);
        if (simulator == null)
        {
            throw new InvalidOperationException("Failed to initialize simulation.");
        }

        _simulationStorage.AddSimulation(simulationId, simulator);
        return simulationId;
    }


    public void StartSimulation(string simulationId)
    {
        var simulator = _simulationStorage.GetSimulation(simulationId);
        if (simulator == null)
        {
            throw new InvalidOperationException($"Simulation with ID {simulationId} not found.");
        }

        simulator.PlayOneGame();

    }

    public void PauseSimulation(string simulationId)
    {
        var simulator = _simulationStorage.GetSimulation(simulationId);
        simulator.StopGame();
    }

    public void ResumeSimulation(string simulationId)
    {
        var simulator = _simulationStorage.GetSimulation(simulationId);
        simulator.Resume();
    }
    
    public void EndSimulation(string simulationId)
    {
        var simulator = _simulationStorage.GetSimulation(simulationId);
        simulator.End();
    }
    
    
}