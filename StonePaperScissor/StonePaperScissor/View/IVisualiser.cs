using StonePaperScissor.Service.Simulation;

namespace StonePaperScissor.View;

public interface IVisualiser
{
    void SimulationVisualisation(List<Item> items, int rows, int columns);
}