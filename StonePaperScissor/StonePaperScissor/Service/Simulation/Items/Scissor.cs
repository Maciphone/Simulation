namespace StonePaperScissor.Service.Simulation.Items;

public class Scissor : Item
{
    public Scissor(ItemType type, string sign, Position position, IUtil util) : base(type, sign, position, util)
    {
    }

    public Scissor(string sign, Position position, IUtil util) : base(sign, position, util)
    {
        Type = ItemType.Scissor;
    }

    public override void Move()
    {
        if (Alive)
        {
            List<Field> optionalFields = Util.FieldsAround(Position);
            if (optionalFields == null || optionalFields.Count == 0)
            {
                Console.WriteLine("No optional fields found.");
                return;
            }
            
            var hitableFields = optionalFields.FindAll(f =>f.Item!=null && f.Item.Type == ItemType.Paper);
            if (hitableFields.Count == 0)
            {
                Console.WriteLine("No hitable fields, choosing a random field.");
                Position = GetRandomField(optionalFields).Position;
            }
            else
            {
                var index = Random.Next(0, hitableFields.Count-1);
                var item = optionalFields[index].Item;
                Position = item.Position;
                Hit(item);
            }
            
        }
    }
}