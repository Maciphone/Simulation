using StonePaperScissor.Service.Simulation.SimulationServices.Interfaces;

namespace StonePaperScissor.Service.Simulation.SimulationServices;

public class SimulationStorage : ISimulationStorage
{
    private readonly Dictionary<string, ISimulator> _simulations = new();

    public void AddSimulation(string simulationId, ISimulator simulator)
    {
        _simulations[simulationId] = simulator;
    }

    public ISimulator GetSimulation(string simulationId)
    {
        return _simulations[simulationId];
        return _simulations.TryGetValue(simulationId, out var simulator) ? simulator : null;
    }

    public void RemoveSimulation(string simulationId)
    {
        _simulations.Remove(simulationId);
    }
}