using Microsoft.AspNetCore.SignalR;
using StonePaperScissor.Service.Simulation;

namespace StonePaperScissor.HubSignalWebsocket;

public class SimulationHub :Hub
{
    public async Task JoinSimulation(string simulationId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, simulationId);
        await Clients.Caller.SendAsync("JoinedSimulation", simulationId);
    }

    public async Task SendSimulationState(string simulationId, string items)
    {
        await Clients.Group(simulationId).SendAsync("ReceiveGameState", items);
    }
}