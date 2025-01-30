using StonePaperScissor.View;

namespace StonePaperScissor.Service.Simulation.SimulationServices.Interfaces;


public interface ISimulatorFactory
{
    ISimulator CreateSimulator(int rows, int columns, List<Item> items);
}