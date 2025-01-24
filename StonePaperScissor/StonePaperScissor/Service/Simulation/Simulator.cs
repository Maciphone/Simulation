using System.Security.Cryptography.Xml;
using StonePaperScissor.Service.Simulation.Items;
using StonePaperScissor.View;

namespace StonePaperScissor.Service.Simulation;

public class Simulator
{
    public int Rows { get; set; }
    public int Columns { get; set; }
    public List<Item> _items;
    private List<Item> _newItems;
    private readonly IVisualiser _dotVisualiser;
    private readonly IGameStatistic _dotGameStatistic;
    private readonly IUtil _util;
    

    public Simulator(List<Item> items, int rows, int columns, IVisualiser dotVisualiser, IGameStatistic dotGameStatistic, IUtil util)
    {
        _items = items;
        Rows = rows;
        Columns = columns;
        _dotVisualiser = dotVisualiser;
        _dotGameStatistic = dotGameStatistic;
        _util = util ?? throw new ArgumentNullException(nameof(util));
        _newItems = new List<Item>();
    }

    public Item GetItemByPosition(Position position)
    {
        return _items.Find(f => Equals(f.Position, position));
    }
      // _items.Where(item => item.Alive).ToList()
        //     .ForEach(item => item.Move());  // nullreference hiba
    private void PlayOneRound()
    {
  
        foreach (var item in _items)
        {
            if(item.Alive){ item.Move();}
        }
       
        TransformItems();
        RemoveNotAliveItems();
        _dotVisualiser.SimulationVisualisation(_items, Rows, Columns);
        _dotGameStatistic.ShowStatistic(_items);
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
        var hitedItems = _items.Where(e => e.Alive == false);
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
                _newItems.Add(new Scissor("S", hitedItem.Position, _util));
            break;
            case ItemType.Scissor:
                _newItems.Add(new Stone("O", hitedItem.Position, _util));
                break;
            case ItemType.Stone:
                _newItems.Add(new Paper("P", hitedItem.Position, _util));
                break;
        }
    }
}