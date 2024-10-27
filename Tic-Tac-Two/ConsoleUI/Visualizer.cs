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

    public static void WriteGamePlayInstructions(TicTacTwoBrain gameInstance, string errorMessage)
    {
        WriteBasicGamePlayInstructions(gameInstance, errorMessage);
        Console.Write("Give me coordinates <");
        Console.ForegroundColor = VisualizerHelper.XAxisColor;
        Console.Write("x");
        Console.ResetColor();
        Console.Write(",");
        Console.ForegroundColor = VisualizerHelper.YAxisColor;
        Console.Write("y");
        Console.ResetColor();
        Console.Write($">, save <S>{WriteConditionalInstructions(gameInstance)} or return <R> to main menu:");
    }
    
    public static void WriteMovePieceModeInstructions(TicTacTwoBrain gameInstance, string errorMessage)
    {
        WriteBasicGamePlayInstructions(gameInstance, errorMessage);
        Console.Write("Give me coordinates of the piece you want to move <x,y>:");
    }
    
    public static void WriteMoveGridModeInstructions(TicTacTwoBrain gameInstance, string errorMessage)
    {
        WriteBasicGamePlayInstructions(gameInstance, errorMessage);
        Console.Write("Move grid with arrow keys and press <Enter> to lock position:");
    }

    private static void WriteBasicGamePlayInstructions(TicTacTwoBrain gameInstance, string errorMessage)
    {
        Console.ForegroundColor = VisualizerHelper.MessageColor;
        Console.WriteLine($"{gameInstance.GetNextMoveByPlayerName()}'s turn!");
        Console.ForegroundColor = VisualizerHelper.ErrorMessageColor;
        Console.WriteLine(errorMessage);
        Console.ResetColor();
    }

    private static string WriteConditionalInstructions(TicTacTwoBrain gameInstance)
    {
        var pieceMovingInstructions = gameInstance.CanMovePiece() ? ", move piece <P>" : "";
        var gridMovingInstructions = gameInstance.CanMoveGrid() ? ", move grid <G>" : "";
        return pieceMovingInstructions + gridMovingInstructions;
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
        Console.Write("   ");
        Console.ForegroundColor = VisualizerHelper.XAxisColor;
        for (int x = 0; x < gameInstance.DimX; x++)
        {
            if (x < 10)
            {
                Console.Write(" ");
            }
            Console.Write($" {x} ");
        }
        Console.ResetColor();
        Console.WriteLine("");
        
        Console.Write($"   {VisualizerHelper.BoardCornerNorthEast}");
        for (int x = 0; x < gameInstance.DimX; x++)
        {
            Console.Write(VisualizerHelper.BoardLineHorisontal);
            if (x != gameInstance.DimX - 1)
            {
                Console.Write(VisualizerHelper.BoardCrossingSouth);
            }
        }
        Console.WriteLine(VisualizerHelper.BoardCornerNorthWest);
    }

    private static void DrawBoardMain(TicTacTwoBrain gameInstance)
    {
        for (int y = 0; y < gameInstance.DimY; y++)
        {
            Console.ForegroundColor = VisualizerHelper.YAxisColor;
            if (y < 10)
            {
                Console.Write(" ");
            }
            Console.Write($"{y}");
            Console.ResetColor();
            Console.Write($" {VisualizerHelper.BoardLineVertical}");
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
                Console.Write(VisualizerHelper.BoardLineVertical);
            }
            Console.WriteLine();
            if (y == gameInstance.DimY - 1) break;
            
            Console.Write($"   {VisualizerHelper.BoardCrossingWest}");
            for (int x = 0; x < gameInstance.DimX; x++)
            {
                Console.Write(VisualizerHelper.BoardLineHorisontal);
                if (x != gameInstance.DimX - 1)
                {
                    Console.Write(VisualizerHelper.BoardCrossing);
                }
            }
            Console.WriteLine(VisualizerHelper.BoardCrossingEast);
        }
    }

    private static void DrawBoardEnd(TicTacTwoBrain gameInstance)
    {
        Console.Write($"   {VisualizerHelper.BoardCornerSouthEast}");
        for (int x = 0; x < gameInstance.DimX; x++)
        {
            Console.Write(VisualizerHelper.BoardLineHorisontal);
            if (x != gameInstance.DimX - 1)
            {
                Console.Write(VisualizerHelper.BoardCrossingNorth);
            }
        }
        Console.WriteLine(VisualizerHelper.BoardCornerSouthWest);
    }

    public static void DisplayGameOverMessage()
    {
        Console.WriteLine(VisualizerHelper.GameOverMessage);
        Console.WriteLine("Press <R> to return to main menu:");
    }

    public static void DisplayFinalRoundMessage()
    {
        Console.ForegroundColor = VisualizerHelper.ErrorMessageColor;
        Console.WriteLine(VisualizerHelper.FinalRoundMessage);
        Console.ResetColor();
    }
}