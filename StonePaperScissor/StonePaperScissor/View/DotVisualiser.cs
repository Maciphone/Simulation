using StonePaperScissor.Service.Simulation;

namespace StonePaperScissor.View;

public class DotVisualiser : IVisualiser
{
    public DotVisualiser()
    {
    }


    public void SimulationVisualisation(List<Item> items, int rows, int columns)
    {
        char[,] board = new char[rows, columns];
        DrawBoard(board, rows, columns);
        AddItemPosition(items, board);
        DrawPicture(board, rows, columns);
    }

    

    private void DrawPicture(char[,] board, int rows, int columns)
    {
 
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Console.Write(board[i, j] + " ");
            }
            Console.WriteLine();
        }

        Console.WriteLine();
    }


    private void DrawBoard(char[,] board, int rows, int columns)
    {
  
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                board[i, j] = '.'; 
            }
        }
    }
    
    void AddItemPosition(List<Item> items, char[,] board)
    {
        foreach (var item in items)
        {
            board[item.Position.Row, item.Position.Column] = item.Sign[0];
                
        }
    }
}