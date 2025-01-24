namespace StonePaperScissor.Service.Simulation;

public class Field
{
    public Position Position { get; set; }
    public Item Item { get; set; }

    public Field(Position position, Item item)
    {
        Position = position;
        Item = item;
    }
    
 
    
}