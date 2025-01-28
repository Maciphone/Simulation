using StonePaperScissor.Service.Simulation;

namespace StonePaperScissor.Service.Initialiser;

public interface IInitialiser
{
    ISimulator Initialise(int row, int columns, int items);
}