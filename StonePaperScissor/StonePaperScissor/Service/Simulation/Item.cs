using StonePaperScissor.Service.Simulation.Items;

namespace StonePaperScissor.Service.Simulation;

public abstract class Item
{
    protected Random Random = new Random();
    public ItemType Type { get; set; }
    public bool Alive { get; set; }
    public string Sign { get; }
    public Position Position { get; set; }
    protected IUtil Util;

    protected Item(ItemType type, string sign, Position position, IUtil util)
    {
        Type = type;
        Sign = sign;
        Position = position;
        Alive = true;
        SetUtil(util);
    }

    protected Item(string sign, Position position, IUtil util)
    {
        Sign = sign;
        Position = position;
        Alive = true;
        SetUtil(util);
    }
    
    public void SetUtil(IUtil util)
    {
        Util = util ?? throw new ArgumentNullException(nameof(util));
    }


    public void Hit(Item item)
    {
        item.Alive = false;
    }

    protected Field GetRandomField(List<Field> optionalFields)
    {
        
        
            int index = Random.Next(0,optionalFields.Count);
            var field = optionalFields[index];
        

        return field;
    }

    public abstract void Move();
}