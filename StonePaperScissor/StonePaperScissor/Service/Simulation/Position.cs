namespace StonePaperScissor.Service.Simulation;

public class Position
{
    public int Row { get; set; }
    public int Column { get; set; }

    public Position(int row, int column)
    {
        Row = row;
        Column = column;
    }

    public Position()
    {
    }
    
    public override bool Equals(object? obj)
    {
        if (obj is Position other)
        {
            return Row == other.Row && Column == other.Column;
        }
        return false;
    }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(Row, Column);
    }
}