namespace StonePaperScissor.Service.Simulation.SimulationServices.Interfaces;


public interface ISimulatorService
{
    bool InitialiseSimulation(int row, int columns, int itemCount);
    void StartSimulation();
    public void StopSimulation();
    void ResumeGame();

}