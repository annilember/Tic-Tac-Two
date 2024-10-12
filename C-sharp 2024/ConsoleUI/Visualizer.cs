using GameBrain;

namespace ConsoleUI;

public static class Visualizer
{
    public static void DrawBoard(TicTacTwoBrain gameInstance)
    {
        for (int y = 0; y < gameInstance.DimY; y++)
        {
            for (int x = 0; x < gameInstance.DimX; x++)
            {
                Console.Write(" " + DrawGamePiece(gameInstance.GameBoard[x, y]) + " ");
                if (x != gameInstance.DimX - 1)
                {
                    Console.Write("|");
                }
            }
            Console.WriteLine();
            if (y == gameInstance.DimY - 1) break;
            for (int x = 0; x < gameInstance.DimX; x++)
            {
                Console.Write("---");
                if (x != gameInstance.DimX - 1)
                {
                    Console.Write("+");
                }
            }
            Console.WriteLine();
        }
        
        Console.BackgroundColor = ConsoleColor.DarkRed;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.ResetColor();
    }
    
    private static string DrawGamePiece(EGamePiece piece) =>
        piece switch
        {
            EGamePiece.O => "O",
            EGamePiece.X => "X",
            _ => " "
        };
}