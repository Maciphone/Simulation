using Microsoft.AspNetCore.SignalR;
using StonePaperScissor.HubSignalWebsocket;
using StonePaperScissor.Service.Simulation.SimulationServices.Interfaces;
using StonePaperScissor.View;

namespace StonePaperScissor.Service.Simulation.SimulationServices;

public class SimulatorFactory : ISimulatorFactory
{
  
    private readonly IHubContext<SimulationHub> _hubContext;
    private readonly IServiceProvider _serviceProvider;

    public SimulatorFactory(IHubContext<SimulationHub> hubContext, IServiceProvider serviceProvider)
    {
        _hubContext = hubContext;
        _serviceProvider = serviceProvider;
    }

    public ISimulator CreateSimulator(int rows, int columns, List<Item> items)
    {
        var visualiser = _serviceProvider.GetRequiredService<IVisualiser>();
        var statistic = _serviceProvider.GetRequiredService<IGameStatistic>();

        return new Simulator(items, rows, columns, visualiser, statistic, _hubContext);
    }
}

    // public ISimulator CreateSimulator(int rows, int columns, List<Item> items, IVisualiser dotVisualiser, IGameStatistic statistic)
    // {
    //     return new Simulator
    //     {
    //         _items = items,
    //         X = rows,
    //         Y = columns,
    //         _dotVisualiser = dotVisualiser,
    //         _dotGameStatistic = statistic
    //     };
    // }
