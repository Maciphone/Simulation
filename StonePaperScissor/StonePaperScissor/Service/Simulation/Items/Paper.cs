namespace StonePaperScissor.Service.Simulation.Items;

public class Paper : Item
{
    public Paper(ItemType type, string sign, Position position, IUtil util) : base(type, sign, position, util)
    {
    }

    public Paper(string sign, Position position, IUtil util) : base(sign, position, util)
    {
        Type = ItemType.Paper;
    }

    public override void Move()
    {
        if (Alive)
        {
            List<Field> optionalFields = Util.FieldsAround(Position);
            var hitableFields = optionalFields
                .FindAll(f => f.Item!=null &&  f.Item.Type == ItemType.Stone);
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