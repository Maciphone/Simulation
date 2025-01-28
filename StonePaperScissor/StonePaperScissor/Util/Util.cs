using StonePaperScissor.Service.Simulation;

namespace StonePaperScissor.Util;

public  static class Util
{
    private static readonly Random Random = new Random();
    

    public  static List<Field> FieldsAround(Position position, int row, int column,List<Item> items)
    {
        List<Field> result = new List<Field>();
        int startRowIndex = position.X;
        int startColumnIndex = position.Y;

        int[] dRow =
            [-1, -1, -1, 0, 0, 1, 1, 1];
        int[] dCol = [-1, 0, 1, -1, 1, -1, 0, 1];
        for (int i = 0; i < dRow.Length; i++)
        {
            int newRow = startRowIndex + dRow[i];
            int newCol = startColumnIndex + dCol[i];

            if (newRow >= 0 && newRow < row && newCol >= 0 && newCol <column)
            {
                var tempPosition = new Position(newRow, newCol);
                var item =  items.FirstOrDefault(it => it.Position.Equals(tempPosition));
                result.Add(new Field(tempPosition, item));
            }
        }

        return result;
    }
    
    public static Field GetRandomField(List<Field> optionalFields)
    {
        
        
        int index = Random.Next(0,optionalFields.Count);
        var field = optionalFields[index];
        

        return field;
    }
}
    
    
