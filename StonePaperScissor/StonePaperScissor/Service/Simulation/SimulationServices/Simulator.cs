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
    private int _delay;
    private List<Item> _deathItems;
    
    

    public Simulator(List<Item> items, int x, int y, IVisualiser dotVisualiser, IGameStatistic dotGameStatistic, IHubContext<SimulationHub> hubContext)
    {
        _items = items;
        X = x;
        Y = y;
        _dotVisualiser = dotVisualiser;
        _dotGameStatistic = dotGameStatistic;
        _hubContext = hubContext;
        _newItems = new List<Item>();
        _deathItems = new List<Item>();
        Stopped = false;
        _delay = 30;
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
                if (hitItem != null)
                {
                    _newItems.Add(hitItem);
                }
                // {
                //     //_items.Remove(item);
                //     //_newItems.Add(item);
                //    // TransformItem(hitItem);
                //   //  ReactivateHitedItem(hitItem);
                // }

            } 
            else {
                 _deathItems.Add(item); }
             
        }
       
        TransformNewItems();
       // RemoveDeathItems();

        //_dotVisualiser.SimulationVisualisation(_items, X, Y);

        //_dotGameStatistic.ShowStatistic(_items);
    }

    private void RemoveDeathItems()
    {
        _items.RemoveAll(item => _deathItems.Contains(item));
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
            //send item list
            string gameState = SerializeGameState();
            await _hubContext.Clients.Group(_simulationId).SendAsync("ReceiveGameState", gameState);
            //send statistic
            var gameStatistic = _dotGameStatistic.SendStatistic(_items);
            await _hubContext.Clients.Group(_simulationId).SendAsync("ReceiveStatistic", gameStatistic);
           
            Thread.Sleep(_delay);
            
            Console.WriteLine(count);
            
        }
        //send winner
        var result = _items.Select(item => item.Type).ToList()[0];
        await _hubContext.Clients.Group(_simulationId).SendAsync("ReceiveWinner", result);

    }

    private string SerializeGameState()
    {
        return JsonSerializer.Serialize(_items);
        
    }


    private bool OnlyOneType()
    {
       
        // bool isSingleType = _items.Any() && _items.Select(item => item.Type).Distinct().Count() == 1;
        // return isSingleType;

        if (_items.Select(item => item.Type).ToHashSet().Count == 2 )
        {
            _delay = 15;

        }
        if (_items.Select(item => item.Type).ToHashSet().Count == 1)
        {
            var result = _items.Select(item => item.Type).ToList()[0];
            
        }
        return _items.Select(item => item.Type).ToHashSet().Count == 1;

    }
    

    private void TransformNewItems()
    {
        _newItems.ForEach(MakeOneTransform);
        _newItems.Clear();
        
    }

    private void MakeOneTransform(Item hitItem)
    {
        if (hitItem.Alive)
        {
            return;
        }

        // Item.Equals compares type and position, not individual identity.
        var index = _items.FindIndex(item => ReferenceEquals(item, hitItem));
        if (index < 0)
        {
            return;
        }

        Item replacement = hitItem.Type switch
        {
            ItemType.Paper => new Scissor("S", hitItem.Position),
            ItemType.Scissor => new Stone("O", hitItem.Position),
            ItemType.Stone => new Paper("P", hitItem.Position),
            _ => throw new ArgumentOutOfRangeException(
                nameof(hitItem.Type), hitItem.Type, "Unknown item type")
        };

        // Replace exactly one defeated object without changing the population.
        _items[index] = replacement;
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