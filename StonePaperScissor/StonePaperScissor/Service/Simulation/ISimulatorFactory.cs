using StonePaperScissor.View;

namespace StonePaperScissor.Service.Simulation;

public interface ISimulatorFactory
{
    ISimulator CreateSimulator(int rows, int columns, List<Item> items, IVisualiser dotVisualiser, IGameStatistic statistic);
}