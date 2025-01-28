namespace StonePaperScissor.Service.Simulation.SimulationServices.Interfaces;

public interface IInitialiser
{
    ISimulator Initialise(int row, int columns, int items);
}