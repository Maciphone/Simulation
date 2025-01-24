namespace StonePaperScissor.Service.Simulation.Items;

public  class Util : IUtil
{
    private  ISimulatorService _simulatorService;

    public Util(ISimulatorService simulatorService)
    {
        _simulatorService = simulatorService;
    }

    public  List<Field> FieldsAround(Position position)
    {
        List<Field> result = new List<Field>();
        int startRowIndex = position.Row;
        int startColumnIndex = position.Column;

        int[] dRow =
            [-1, -1, -1, 0, 0, 1, 1, 1];
        int[] dCol = [-1, 0, 1, -1, 1, -1, 0, 1];
        for (int i = 0; i < dRow.Length; i++)
        {
            int newRow = startRowIndex + dRow[i];
            int newCol = startColumnIndex + dCol[i];

            if (newRow >= 0 && newRow < _simulatorService.Simulator.Rows-1 && newCol >= 0 && newCol < _simulatorService.Simulator.Columns-1)
            {
                var tempPosition = new Position(newRow, newCol);
                var item = _simulatorService.Simulator.GetItemByPosition(tempPosition);
                result.Add(new Field(tempPosition, item));
            }
        }

        return result;
    }
}
    
    
