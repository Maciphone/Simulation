using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using StonePaperScissor.Service.Simulation;

namespace StonePaperScissor.HubSignalWebsocket;

[Authorize] //only authorised user/guest can join
public class SimulationHub :Hub
{
    //szálbiztos dict. több szálon lehet hozzáférni
    private static ConcurrentDictionary<string, string> gamemasterDictionary = new();
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

    public async Task<bool> RoomExists(string gameMasterId)
    {
       
        if (gamemasterDictionary.ContainsKey(gameMasterId)) 
        {
            return true;
        }

        return false;

    }
    public async Task JoinGameMaster(string gameMasterId, string? simulationId)
    {
        if (simulationId != null)
        {
            gamemasterDictionary[gameMasterId] = simulationId;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, gameMasterId);
        await Clients.Group(gameMasterId).SendAsync("ReceiveSimulationId", simulationId);
        
    }
    //csoport tagjainak üzenet küldése
    
    
    public async Task<string> GetSimulationIdForGameMaster(string gameMasterId)
    {
        if (gamemasterDictionary.TryGetValue(gameMasterId, out var simulationId))
        {
            return simulationId;
        }
        return null; 
    }
  
    
    
    
    //csoport elhagyása
    public async Task LeaveGameMaster(string gameMasterId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameMasterId);
        if(!gamemasterDictionary.TryRemove(gameMasterId, out string value))
        {
            await Clients.Caller.SendAsync("Error", $"GameMaster {gameMasterId} nem lett törölve");
        }
        await Clients.Caller.SendAsync("LeftGameMaster", gameMasterId);
    }
}