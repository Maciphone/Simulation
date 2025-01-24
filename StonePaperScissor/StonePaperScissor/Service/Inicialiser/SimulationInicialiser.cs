using StonePaperScissor.Service.Simulation;
using StonePaperScissor.Service.Simulation.Items;
using StonePaperScissor.View;

namespace StonePaperScissor.Service.Inicialiser;

public class SimulationInicialiser :IInicialiser
{
    private static readonly Random _random = new Random();
    private readonly IVisualiser _dotVisualiser;
    private readonly IGameStatistic _dotGameStatistic;
    private readonly IUtil _util;

    public SimulationInicialiser(IVisualiser dotVisualiser, IGameStatistic dotGameStatistic, IUtil util)
    {
       
        _dotVisualiser = dotVisualiser;
        _dotGameStatistic = dotGameStatistic;
        _util = util ?? throw new ArgumentNullException(nameof(util));
    }

    
    public Simulator Inicialise(int row, int columns, int itemCount)
    {
        List<Item> startItems = CreateAllItems(itemCount, row, columns);

        return new Simulator(startItems, row, columns, _dotVisualiser, _dotGameStatistic, _util);
    }

    private List<Item> CreateAllItems(int itemCount, int row, int columns)
    {
        List<Position> startPositions = CreateStartPosition(row, columns);
  
        var stones = CreateItems<Stone>(startPositions, itemCount, "O");
        var papers = CreateItems<Paper>(startPositions, itemCount, "P");
        var scissors = CreateItems<Scissor>(startPositions, itemCount, "S");
        
        var allItems = new List<Item>();
        allItems.AddRange(stones);
        allItems.AddRange(papers);
        allItems.AddRange(scissors);

        return allItems;
       
    }

    private List<Item> CreateItems<T>(List<Position> startPositions, int itemCount, string sign)
        where T : Item
    {
        var items = new List<Item>();
        var index = _random.Next(startPositions.Count);
        var position = startPositions[index];
        var item = (T)Activator.CreateInstance(typeof(T), sign, position, _util);
        for (int i = 0; i < itemCount; i++)
        {
           Console.WriteLine(item);
            items.Add(item);
        }
        startPositions.RemoveAt(index);

        return items;
    }


    private List<Stone> CreateStone(List<Position> startPositions, int itemCount)
    {
        var index = _random.Next(startPositions.Count);
        var stone = new Stone("O", startPositions[index], _util);
        startPositions.RemoveAt(index);
        List<Stone> stones = new List<Stone>();
        for (int i = 0; i < itemCount; i++)
        {
            stones.Add(stone);
        }

        return stones;

    }

    private List<Position> CreateStartPosition(int row, int columns)
    {
        List<Position> positions = new List<Position>
        {
            new Position(0, 0),                
            new Position(row - 1, columns - 1), 
            new Position(0, columns - 1),       
            new Position(row - 1, 0)           
        };
        return positions;

    }
}