namespace StonePaperScissor.Service.Simulation.SimulationServices.Interfaces;


public interface ISimulatorService
{
    string InitialiseSimulation(int row, int columns, int itemCount);
    void StartSimulation(string simulationId);
    public void PauseSimulation(string simulationId);
    void ResumeSimulation(string simulationId);
    void EndSimulation(string simulationId);
    void SetSimulationId(string simulationId);


    string InitialSavedSimulation(int rows, int columns, List<Item> savedItems);
}