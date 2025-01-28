using StonePaperScissor.Service.Initialiser;

namespace StonePaperScissor.Service.Simulation;

public class SimulationService : ISimulatorService
{
    private readonly IInitialiser _initialiser;
    private ISimulator _simulator;
    
    
}

public interface ISimulatorService
{
}