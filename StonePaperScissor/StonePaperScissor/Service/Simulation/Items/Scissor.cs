namespace StonePaperScissor.Service.Simulation.Items;

public class Scissor : Item
{
    public Scissor(ItemType type, string sign, Position position) : base(type, sign, position)
    {
    }

    public Scissor(string sign, Position position) : base(sign, position)
    {
        Type = ItemType.Scissor;
    }

    public override void Move(int row, int column,  List<Item> items)
    {
        if (Alive)
        {
            List<Field> optionalFields = Util.Util.FieldsAround(Position, row,column,items);
            var hittableFields = optionalFields
                .FindAll(f => f.Item is { Type: ItemType.Paper });
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
            }
        }
    }
}