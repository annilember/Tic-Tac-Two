using GameBrain;

namespace ConsoleUI;

public static class Visualizer
{
    
    public static void DrawBoard(TicTacTwoBrain gameInstance)
    {
        Console.Clear();
        DrawBoardBeginning(gameInstance);
        DrawBoardMain(gameInstance);
        DrawBoardEnd(gameInstance);
    }

    public static void WriteInstructions(TicTacTwoBrain gameInstance, string errorMessage)
    {
        Console.WriteLine("Someone's turn!");
        Console.ForegroundColor = VisualizerHelper.ErrorMessageColor;
        Console.WriteLine(errorMessage);
        Console.ResetColor();
        Console.Write("Give me coordinates <");
        Console.ForegroundColor = VisualizerHelper.XAxisColor;
        Console.Write("x");
        Console.ResetColor();
        Console.Write(",");
        Console.ForegroundColor = VisualizerHelper.YAxisColor;
        Console.Write("y");
        Console.ResetColor();
        Console.Write($">, <save>{WriteGridMovingInstructions(gameInstance)} or return <R> to main menu:");
    }

    private static string WriteGridMovingInstructions(TicTacTwoBrain gameInstance)
    {
        return gameInstance.CanMoveGrid() ? ", move grid <G>" : "";
    }
    
    public static void WriteMoveGridModeInstructions()
    {
        Console.Write("Move grid with arrow keys and press <Enter> to lock position:");
    }
    
    private static string DrawGamePiece(EGamePiece piece) =>
        piece switch
        {
            EGamePiece.O => "O",
            EGamePiece.X => "X",
            _ => " "
        };

    private static void DrawBoardBeginning(TicTacTwoBrain gameInstance)
    {
        Console.Write("  ");
        Console.ForegroundColor = VisualizerHelper.XAxisColor;
        for (int x = 0; x < gameInstance.DimX; x++)
        {
            Console.Write($"  {x} ");
        }
        Console.ResetColor();
        Console.WriteLine("");
        
        Console.Write("  \u2554");
        for (int x = 0; x < gameInstance.DimX; x++)
        {
            Console.Write("\u2550\u2550\u2550");
            if (x != gameInstance.DimX - 1)
            {
                Console.Write("\u2566");
            }
        }
        Console.WriteLine("\u2557");
    }

    private static void DrawBoardMain(TicTacTwoBrain gameInstance)
    {
        for (int y = 0; y < gameInstance.DimY; y++)
        {
            Console.ForegroundColor = VisualizerHelper.YAxisColor;
            Console.Write($"{y}");
            Console.ResetColor();
            Console.Write(" \u2551");
            for (int x = 0; x < gameInstance.DimX; x++)
            {
                if (gameInstance.MoveGridModeOn && gameInstance.GridMovingAreaTest[x][y])
                {
                    Console.BackgroundColor = VisualizerHelper.GridAllowedMoveAreaColor;
                }
                if (gameInstance.GameGrid[x][y])
                {
                    Console.BackgroundColor = VisualizerHelper.GridColor;
                }
                Console.Write(" " + DrawGamePiece(gameInstance.GameBoard[x][y]) + " ");
                Console.ResetColor();
                Console.Write("\u2551");
            }
            Console.WriteLine();
            if (y == gameInstance.DimY - 1) break;
            
            Console.Write("  \u2560");
            for (int x = 0; x < gameInstance.DimX; x++)
            {
                Console.Write("\u2550\u2550\u2550");
                if (x != gameInstance.DimX - 1)
                {
                    Console.Write("\u256c");
                }
            }
            Console.WriteLine("\u2563");
        }
    }

    private static void DrawBoardEnd(TicTacTwoBrain gameInstance)
    {
        Console.Write("  \u255a");
        for (int x = 0; x < gameInstance.DimX; x++)
        {
            Console.Write("\u2550\u2550\u2550");
            if (x != gameInstance.DimX - 1)
            {
                Console.Write("\u2569");
            }
        }
        Console.WriteLine("\u255d");
    }

    public static void DisplayGameOverMessage()
    {
        Console.WriteLine(VisualizerHelper.GameOverMessage);
        Console.WriteLine("Press <R> to return to main menu:");
    }
}