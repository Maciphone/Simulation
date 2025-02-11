using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using StonePaperScissor.Service.Simulation;

namespace StonePaperScissor.HubSignalWebsocket;

[Authorize] //only authorised user/guest can join
public class SimulationHub :Hub
{
    //Symulation
    public async Task JoinSimulation(string simulationId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, simulationId);
        await Clients.Caller.SendAsync("JoinedSimulation", simulationId);
    }

    public async Task SendSimulationState(string simulationId, string items)
    {
        await Clients.Group(simulationId).SendAsync("ReceiveGameState", items);
    }
    
    
    //Data flow on chat
    // kvázi chatszoba simulationId küldéséhez
    //létrehoz ha nincs, beléptet ha van
    public async Task JoinGameMaster(string gameMasterId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, gameMasterId);
        await Clients.Caller.SendAsync("JoinedGameMaster", gameMasterId);
    }
    //csoport tagjainak üzenet küldése
    public async Task SendSimulationIdToGameMaster(string gameMasterId, string simulationId)
    {
        await Clients.Group(gameMasterId).SendAsync("ReceiveSimulationId", simulationId);
    }
    
    //csoport elhagyása
    public async Task LeaveGameMaster(string gameMasterId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameMasterId);
        await Clients.Caller.SendAsync("LeftGameMaster", gameMasterId);
    }
}