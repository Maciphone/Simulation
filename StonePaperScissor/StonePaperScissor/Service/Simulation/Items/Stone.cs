using Microsoft.VisualBasic.CompilerServices;

namespace StonePaperScissor.Service.Simulation.Items;

public class Stone : Item
{
    public Stone(ItemType type, string sign, Position position) : base(type, sign, position)
    {
    }

    public Stone(string sign, Position position) : base(sign, position)
    {
        Type = ItemType.Stone;
    }

    public override Item Move(int row, int column,  List<Item> items)
    {
        if (Alive)
        {
            List<Field> optionalFields = Util.Util.FieldsAround(Position, row,column,items);
            var hittableFields = optionalFields
                .FindAll(f => f.Item is { Type: ItemType.Scissor });
            if (hittableFields.Count == 0)
            {
                Field randomField = Util.Util.GetRandomField(optionalFields);
                Position = randomField.Position;
            }
            else
            {
                var index = Random.Next(0, hittableFields.Count);
                var item = hittableFields[index].Item;
                Position = item.Position;
                Hit(item);
                return item;
            }
        }

        return null;
    }
}