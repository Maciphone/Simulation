using StonePaperScissor.Service.Simulation;

namespace StonePaperScissor.Service.Inicialiser;

public interface IInicialiser
{
    Simulator Inicialise(int row, int columns, int items);
}