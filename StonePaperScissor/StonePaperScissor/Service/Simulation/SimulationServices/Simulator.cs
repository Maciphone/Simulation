using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using StonePaperScissor.HubSignalWebsocket;
using StonePaperScissor.Service.Simulation.Items;
using StonePaperScissor.Service.Simulation.SimulationServices.Interfaces;
using StonePaperScissor.View;

namespace StonePaperScissor.Service.Simulation.SimulationServices;

public class Simulator :ISimulator
{
    private static Random _random = new Random();
    private readonly IHubContext<SimulationHub> _hubContext;
    public int X { get; set; }
    public int Y { get; set; }
    public List<Item> _items;
    private List<Item> _newItems;
    public  IVisualiser _dotVisualiser;
    public IGameStatistic _dotGameStatistic;
    private bool Stopped;
    private int count;
    private string _simulationId;
    
    

    public Simulator(List<Item> items, int x, int y, IVisualiser dotVisualiser, IGameStatistic dotGameStatistic, IHubContext<SimulationHub> hubContext)
    {
        _items = items;
        X = x;
        Y = y;
        _dotVisualiser = dotVisualiser;
        _dotGameStatistic = dotGameStatistic;
        _hubContext = hubContext;
        _newItems = new List<Item>();
        Stopped = false;
    }

    public Simulator(IHubContext<SimulationHub> hubContext)
    {
        _hubContext = hubContext;
        _newItems = new List<Item>();
    }

    public void SetSimulationId(string simulationId)
    {
        _simulationId = simulationId;
    }
    
    public void StopGame()
    {
        Stopped = true;
    }

    public void Resume()
    {
        Stopped = false;
        PlayOneGame();
    }

    public void End()
    {
        _items.ForEach(e=>e.Type=ItemType.Paper);
    }

    public async void OpenSocketStream(string simulationId)
    {
        
        await _hubContext.Clients.Group(_simulationId).SendAsync("ReceiveGameState", "see ya players");
        
    }


    // _items.Where(item => item.Alive).To_items()
        //     .ForEach(item => item.Move());  // nullreference hiba
    private void PlayOneRound()
    {
       Shuffle_items();
        foreach (var item in _items)
        {
            if (item.Alive)
            {
              
              
                var hitItem = item.Move(X, Y, _items);
                if (hitItem != null){
                  _newItems.Add(hitItem);
                }
                // {
                //     //_items.Remove(item);
                //     //_newItems.Add(item);
                //    // TransformItem(hitItem);
                //   //  ReactivateHitedItem(hitItem);
                // }
                
            }
        }
       
        TransformNewItems();
   
        _dotVisualiser.SimulationVisualisation(_items, X, Y);
        //_dotGameStatistic.ShowStatistic(_items);
    }

    public void StartPlayOneGame()
    {
        PlayOneGame();
    }
    
    public async void PlayOneGame()
    {
       
        while (!OnlyOneType() && !Stopped)
        {
            count++;
            PlayOneRound();
            string gameState = SerializeGameState();
            await _hubContext.Clients.Group(_simulationId).SendAsync("ReceiveGameState", gameState);
           Thread.Sleep(70);
            Console.WriteLine(count);
        }
    }

    private string SerializeGameState()
    {
        return JsonSerializer.Serialize(_items);
        
    }


    private bool OnlyOneType()
    {
       
        // bool isSingleType = _items.Any() && _items.Select(item => item.Type).Distinct().Count() == 1;
        // return isSingleType;
        return _items.Select(item => item.Type).ToHashSet().Count == 1;

    }
    

    private void TransformNewItems()
    {
        _newItems.ForEach(MakeOneTransform);
        _newItems.Clear();
    }

    private void MakeOneTransform(Item hitedItem)
    {
        switch (hitedItem.Type)
        {
            case ItemType.Paper:
               _items.Add(new Scissor("S", hitedItem.Position));
               _items.Remove(hitedItem);
               
                // hitedItem.Sign = "S";
                // hitedItem.Type = ItemType.Scissor;
            break;
            case ItemType.Scissor:
               _items.Add(new Stone("O", hitedItem.Position));
               _items.Remove(hitedItem);

                // hitedItem.Sign = "O";
                // hitedItem.Type = ItemType.Stone;
                break;
            case ItemType.Stone:
                _items.Add(new Paper("P", hitedItem.Position));
                _items.Remove(hitedItem);

                // hitedItem.Sign = "P";
                // hitedItem.Type = ItemType.Paper;
                break;
        }
    }
    
    //Fisher-Yates Shuffle
    private void Shuffle_items()
    {
        int n = _items.Count;
        while (n > 1)
        {
            int k = _random.Next(n--);
            (_items[n], _items[k]) = (_items[k], _items[n]);
        }
    }

  
    
    

    
}