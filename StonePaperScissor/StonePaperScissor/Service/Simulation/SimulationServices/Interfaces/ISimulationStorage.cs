namespace StonePaperScissor.Service.Simulation.SimulationServices.Interfaces;

public interface ISimulationStorage
{
    void AddSimulation(string simulationId, ISimulator simulator);
    ISimulator GetSimulation(string simulationId);
    void RemoveSimulation(string simulationId);
}