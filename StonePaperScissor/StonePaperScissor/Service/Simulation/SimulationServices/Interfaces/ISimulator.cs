namespace StonePaperScissor.Service.Simulation.SimulationServices.Interfaces;


public interface ISimulator
{
    void PlayOneGame();
    void StopGame();

    void Resume();

    void End();
    void SetSimulationId(string simulationId);
    void StartPlayOneGame();
}