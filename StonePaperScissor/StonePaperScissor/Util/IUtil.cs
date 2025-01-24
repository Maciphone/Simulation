namespace StonePaperScissor.Service.Simulation.Items;

public interface IUtil
{
    List<Field> FieldsAround(Position position);
}