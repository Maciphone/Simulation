using Microsoft.VisualBasic.CompilerServices;

namespace StonePaperScissor.Service.Simulation.Items;

public class Stone : Item
{
    public Stone(ItemType type, string sign, Position position, IUtil util) : base(type, sign, position, util)
    {
    }

    public Stone(string sign, Position position, IUtil util) : base(sign, position, util)
    {
        Type = ItemType.Stone;
    }

    public override void Move()
    {
        if (Alive)
        {
            List<Field> optionalFields = Util.FieldsAround(Position);
            if (optionalFields == null || optionalFields.Count == 0)
            {
                throw new InvalidOperationException("Optional fields are not available.");
            }
            
            var hitableFields = optionalFields.FindAll(f =>f.Item!=null && f.Item.Type == ItemType.Scissor);
            if (hitableFields.Count == 0)
            {
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