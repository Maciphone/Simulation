using StonePaperScissor.Service.Simulation.Items;

namespace StonePaperScissor.Service.Simulation;

public abstract class Item
{
    protected Random Random = new Random();
    public ItemType Type { get; set; }
    public bool Alive { get; set; }
    public string Sign { get; set; }
    public Position Position { get; set; }
  

    protected Item(ItemType type, string sign, Position position)
    {
        Type = type;
        Sign = sign;
        Position = position;
        Alive = true;
       
    }

    protected Item(string sign, Position position)
    {
        Sign = sign;
        Position = position;
        Alive = true;
        
    }
    

    public void Hit(Item item)
    {
        item.Alive = false;
        item.Sign = "";
    }

    

    public abstract void Move(int row, int column, List<Item> items);
}