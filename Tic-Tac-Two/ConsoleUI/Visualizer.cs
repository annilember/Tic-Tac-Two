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
        Console.Write("Give me coordinates ");
        WriteCoordinates();
        Console.Write(", save <");
        Console.ForegroundColor = VisualizerHelper.ActionColor;
        Console.Write("S");
        Console.ResetColor();
        Console.Write(">");
        WriteConditionalInstructions(gameInstance);
        Console.Write(" or return <");
        Console.ForegroundColor = VisualizerHelper.ActionColor;
        Console.Write("R");
        Console.ResetColor();
        Console.Write("> to main menu:");
    }
    
    public static void WriteMovePieceModeInstructions(TicTacTwoBrain gameInstance, string errorMessage)
    {
        WriteBasicGamePlayInstructions(gameInstance, errorMessage);
        Console.Write("Give me coordinates of the piece you want to move ");
        WriteCoordinates();
        Console.Write(":");
    }
    
    public static void WriteMoveGridModeInstructions(TicTacTwoBrain gameInstance, string errorMessage)
    {
        WriteBasicGamePlayInstructions(gameInstance, errorMessage);
        Console.Write("Move grid with arrow keys ");
        WriteArrows();
        Console.Write(" and press <");
        Console.ForegroundColor = VisualizerHelper.ActionColor;
        Console.Write("Enter");
        Console.ResetColor();
        Console.Write("> to lock position:");
    }

    public static void WriteInsertPlayerNameInstructions(EGamePiece gamePiece)
    {
        Console.Write("Enter player (");
        Console.Write(gamePiece.ToString());
        Console.Write(") <");
        Console.ForegroundColor = VisualizerHelper.ActionColor;
        Console.Write("name");
        Console.ResetColor();
        Console.Write("> or cancel <");
        Console.ForegroundColor = VisualizerHelper.ActionColor;
        Console.Write("C");
        Console.ResetColor();
        Console.Write(">: ");
    }
    
    public static void WriteInsertConfigNameInstructions(string errorMessage)
    {
        WriteErrorMessage(errorMessage);
        Console.WriteLine();
        Console.Write("Enter configuration <");
        Console.ForegroundColor = VisualizerHelper.ActionColor;
        Console.Write("name");
        Console.ResetColor();
        Console.Write("> or return <");
        Console.ForegroundColor = VisualizerHelper.ActionColor;
        Console.Write("R");
        Console.ResetColor();
        Console.Write(">: ");
    }
    
    public static void WriteInsertNewPropertyValueInstructions(string propertyName, string errorMessage)
    {
        Console.Clear();
        WriteErrorMessage(errorMessage);
        Console.WriteLine();
        Console.Write("Enter new <");
        Console.ForegroundColor = VisualizerHelper.ActionColor;
        Console.Write("value");
        Console.ResetColor();
        Console.Write($"> for {propertyName} or return <");
        Console.ForegroundColor = VisualizerHelper.ActionColor;
        Console.Write("R");
        Console.ResetColor();
        Console.Write(">: ");
    }

    private static void WriteBasicGamePlayInstructions(TicTacTwoBrain gameInstance, string errorMessage)
    {
        Console.ForegroundColor = VisualizerHelper.MessageColor;
        Console.WriteLine($"{gameInstance.GetNextMoveByPlayerName()}'s turn!");
        WriteErrorMessage(errorMessage);
        Console.WriteLine();
    }
    
    public static void WriteErrorMessage(string message)
    {
        Console.ForegroundColor = VisualizerHelper.ErrorMessageColor;
        Console.Write(message);
        Console.ResetColor();
    }

    private static void WriteCoordinates()
    {
        Console.Write("<");
        Console.ForegroundColor = VisualizerHelper.XAxisColor;
        Console.Write("x");
        Console.ResetColor();
        Console.Write(",");
        Console.ForegroundColor = VisualizerHelper.YAxisColor;
        Console.Write("y");
        Console.ResetColor();
        Console.Write(">");
    }
    
    private static void WriteArrows()
    {
        Console.Write("< ");
        Console.ForegroundColor = VisualizerHelper.ActionColor;
        Console.Write(VisualizerHelper.ArrowLeft);
        Console.Write(" ");
        Console.Write(VisualizerHelper.ArrowRight);
        Console.Write(" ");
        Console.Write(VisualizerHelper.ArrowUp);
        Console.Write(" ");
        Console.Write(VisualizerHelper.ArrowDown);
        Console.ResetColor();
        Console.Write(" >");
    }

    private static void WriteConditionalInstructions(TicTacTwoBrain gameInstance)
    {
        if (gameInstance.CanMovePiece())
        {
            WriteMovePieceInstructions();
        }
        if (gameInstance.CanMoveGrid())
        {
            WriteMoveGridInstructions();
        }
    }
    
    private static void WriteMovePieceInstructions()
    {
        Console.Write(", move piece <");
        Console.ForegroundColor = VisualizerHelper.ActionColor;
        Console.Write("P");
        Console.ResetColor();
        Console.Write(">");
    }
    
    private static void WriteMoveGridInstructions()
    {
        Console.Write(", move grid <");
        Console.ForegroundColor = VisualizerHelper.ActionColor;
        Console.Write("G");
        Console.ResetColor();
        Console.Write(">");
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
                if (gameInstance.MoveGridModeOn && gameInstance.GridMovingArea[x][y])
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