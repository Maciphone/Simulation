using StonePaperScissor.Service.Simulation.SimulationServices.Interfaces;
using StonePaperScissor.View;

namespace StonePaperScissor.Service.Simulation.SimulationServices;

public class Simulator :ISimulator
{
    private static Random _random = new Random();
    public int X { get; set; }
    public int Y { get; set; }
    public List<Item> _items;
    private List<Item> _newItems;
    public  IVisualiser _dotVisualiser;
    public IGameStatistic _dotGameStatistic;
    private bool Stopped;
    
    

    public Simulator(List<Item> items, int x, int y, IVisualiser dotVisualiser, IGameStatistic dotGameStatistic)
    {
        _items = items;
        X = x;
        Y = y;
        _dotVisualiser = dotVisualiser;
        _dotGameStatistic = dotGameStatistic;
        _newItems = new List<Item>();
        Stopped = false;
    }

    public Simulator()
    {
        _newItems = new List<Item>();
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
                // Console.WriteLine(item.Type);
                // Console.WriteLine(item.Position);
                var bef = item.Position;
                item.Move(X, Y, _items);
                var after = item.Position;
                if (Equals(bef, after))
                {
                    Console.WriteLine("szaroslecsó");
                    return;
                }

                // Console.WriteLine(item.Position);
            }

            var newPos = item.Position;
        }
       
        TransformItems();
        ReactivateHitedItems();
        //RemoveNotAliveItems();
        //AddNewItems();
        _dotVisualiser.SimulationVisualisation(_items, X, Y);
        _dotGameStatistic.ShowStatistic(_items);
    }

    private void ReactivateHitedItems()
    {
        _items.ForEach(a=>a.Alive=true);
    }

    private void AddNewItems()
    {
        _items.AddRange(_newItems);
        _newItems.Clear();
    }

    public void PlayOneGame()
    {
       
        while (!OnlyOneType() && !Stopped)
        {
           
            PlayOneRound();
            Thread.Sleep(70);
        }
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

    private bool OnlyOneType()
    {
       
        bool isSingleType = _items.Any() && _items.Select(item => item.Type).Distinct().Count() == 1;
        return isSingleType;

    }

    private void RemoveNotAliveItems()
    {
        _items.RemoveAll(f => f.Alive == false);
    }

    private void TransformItems()
    {
        var hitedItems = _items.FindAll(e => e.Alive == false);
        foreach (var hitedItem in hitedItems)
        {
            MakeOneTransform(hitedItem);
            
        }
        
    }

    private void MakeOneTransform(Item hitedItem)
    {
        switch (hitedItem.Type)
        {
            case ItemType.Paper:
                //_newItems.Add(new Scissor("S", hitedItem.Position));
                hitedItem.Sign = "S";
                hitedItem.Type = ItemType.Scissor;
            break;
            case ItemType.Scissor:
                //_newItems.Add(new Stone("O", hitedItem.Position));
                hitedItem.Sign = "O";
                hitedItem.Type = ItemType.Stone;
                break;
            case ItemType.Stone:
                //_newItems.Add(new Paper("P", hitedItem.Position));
                hitedItem.Sign = "P";
                hitedItem.Type = ItemType.Paper;
                break;
        }
    }

    private void Shuffle_items()
    {
        
        for (int i = _items.Count - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1); 
            (_items[i], _items[j]) = (_items[j], _items[i]); 
        }
    }
    
}