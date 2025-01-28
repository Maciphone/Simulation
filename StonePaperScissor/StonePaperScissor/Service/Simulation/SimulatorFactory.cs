using StonePaperScissor.View;

namespace StonePaperScissor.Service.Simulation;

public class SimulatorFactory : ISimulatorFactory
{
  

    public ISimulator CreateSimulator(int rows, int columns, List<Item> items, IVisualiser dotVisualiser, IGameStatistic statistic)
    {
        return new Simulator
        {
            _items = items,
            X = rows,
            Y = columns,
            _dotVisualiser = dotVisualiser,
            _dotGameStatistic = statistic
        };
    }
}