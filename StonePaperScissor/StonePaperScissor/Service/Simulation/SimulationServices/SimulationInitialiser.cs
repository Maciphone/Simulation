using StonePaperScissor.Service.Simulation.Items;
using StonePaperScissor.Service.Simulation.SimulationServices.Interfaces;
using StonePaperScissor.View;

namespace StonePaperScissor.Service.Simulation.SimulationServices;

public class SimulationInitialiser : IInitialiser
{
    private static readonly Random _random = new Random();
 //   private readonly ISimulatorFactory _simulatorFactory;
    private readonly IServiceProvider _serviceProvider;
   

    public SimulationInitialiser(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    
    public ISimulator Initialise(int row, int columns, int itemCount)
    {
        List<Item> startItems = CreateAllItems(itemCount, row, columns);
        using var scope = _serviceProvider.CreateScope();
        var simulatorFactory = scope.ServiceProvider.GetRequiredService<ISimulatorFactory>();


        return simulatorFactory.CreateSimulator(row,columns,startItems);
    }

    public ISimulator ReloadSavedSimulator(int row, int columns, List<Item> items)
    {
        using var scope = _serviceProvider.CreateScope();
        var simulatorFactory = scope.ServiceProvider.GetRequiredService<ISimulatorFactory>();
        return simulatorFactory.CreateSimulator(row,columns,items);
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
        var index = _random.Next(startPositions.Count);
        var items = new List<Item>();
        var position = startPositions[index];
        for (int i = 0; i < itemCount; i++)
        {
             var item = (T)Activator.CreateInstance(typeof(T), sign, position);
           Console.WriteLine(item);
            items.Add(item);
        }
        startPositions.RemoveAt(index);

        return items;
    }


    private List<Stone> CreateStone(List<Position> startPositions, int itemCount)
    {
        var index = _random.Next(startPositions.Count);
        var stone = new Stone("O", startPositions[index]);
        startPositions.RemoveAt(index);
        List<Stone> stones = new List<Stone>();
        for (int i = 0; i < itemCount; i++)
        {
            stones.Add(stone);
        }

        return stones;

    }

    private List<Position> CreateStartPosition(int x, int y
    )
    {
        List<Position> positions = new List<Position>
        {
            new Position(0, 0),                
            new Position(x - 1, y - 1), 
            new Position(x/2, y/2),       
            new Position(x - 1, 0)           
        };
        return positions;

    }
}