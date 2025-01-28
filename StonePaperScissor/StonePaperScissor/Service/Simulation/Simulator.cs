using System.Security.Cryptography.Xml;
using StonePaperScissor.Service.Simulation.Items;
using StonePaperScissor.View;

namespace StonePaperScissor.Service.Simulation;

public class Simulator :ISimulator
{
    private static Random _random = new Random();
    public int X { get; set; }
    public int Y { get; set; }
    public List<Item> _items;
    private List<Item> _newItems;
    public  IVisualiser _dotVisualiser;
    public IGameStatistic _dotGameStatistic;
    
    

    public Simulator(List<Item> items, int x, int y, IVisualiser dotVisualiser, IGameStatistic dotGameStatistic)
    {
        _items = items;
        X = x;
        Y = y;
        _dotVisualiser = dotVisualiser;
        _dotGameStatistic = dotGameStatistic;
        _newItems = new List<Item>();
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
        RemoveNotAliveItems();
        AddNewItems();
        _dotVisualiser.SimulationVisualisation(_items, X, Y);
        //_dotGameStatistic.ShowStatistic(_items);
    }

    private void AddNewItems()
    {
        _items.AddRange(_newItems);
        _newItems.Clear();
    }

    public void PlayOneGame()
    {
       
        while (!OnlyOneType())
        {
           
            PlayOneRound();
        }
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
                _newItems.Add(new Scissor("S", hitedItem.Position));
            break;
            case ItemType.Scissor:
                _newItems.Add(new Stone("O", hitedItem.Position));
                break;
            case ItemType.Stone:
                _newItems.Add(new Paper("P", hitedItem.Position));
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