using DAL;
using Domain;
using DTO;
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

    public static void WriteAisTurnMessage(TicTacTwoBrain gameInstance)
    {
        Console.ForegroundColor = VisualizerHelper.MessageColor;
        Console.Write($"{gameInstance.GetNextMoveByPlayerName()}'s turn ({gameInstance.NextMoveBy})! ");
        Console.Write($"{gameInstance.GamePiecesLeft(gameInstance.NextMoveBy)} pieces left, ");
        Console.WriteLine($"{gameInstance.GameRoundsLeft} rounds left!");
        Console.ForegroundColor = VisualizerHelper.AiColor;
        Console.Write($"{gameInstance.GetNextMoveByPlayerName()} is thinking.");
        Thread.Sleep(1000);
        Console.Write(".");
        Thread.Sleep(1000);
        Console.Write(".");
        Thread.Sleep(1000);
        Console.Write(".");
        Console.ResetColor();
    }
    
    public static void WriteRemovePieceInstructions(TicTacTwoBrain gameInstance, string errorMessage)
    {
        WriteBasicGamePlayInstructions(gameInstance, errorMessage);
        Console.Write("Give me coordinates of the piece you want to move ");
        WriteCoordinates();
        Console.Write(" (or <");
        Console.ForegroundColor = VisualizerHelper.ActionColor;
        Console.Write("R");
        Console.ResetColor();
        Console.Write("> to cancel):");
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
        Console.Write("> to lock position (or <");
        Console.ForegroundColor = VisualizerHelper.ActionColor;
        Console.Write("R");
        Console.ResetColor();
        Console.Write("> to cancel).");
    }
    
    public static void WriteLoadGameModeInstructions()
    {
        Console.Clear();
        Console.Write("Load from saved state <");
        Console.ForegroundColor = VisualizerHelper.ActionColor;
        Console.Write("L");
        Console.ResetColor();
        Console.Write(">, reset game <");
        Console.ForegroundColor = VisualizerHelper.ActionColor;
        Console.Write("G");
        Console.ResetColor();
        Console.Write("> or cancel <");
        Console.ForegroundColor = VisualizerHelper.ActionColor;
        Console.Write("C");
        Console.ResetColor();
        Console.Write(">: ");
    }

    public static void WriteInsertPlayerNameInstructions(EGamePiece gamePiece, EGameMode gameMode)
    {
        var playerType = GameMode.GetPlayerTypeName(gameMode, gamePiece);
        Console.Write("Enter player (");
        Console.Write(Message.GamePieceAsString(gamePiece));
        Console.Write(" / ");
        Console.Write(playerType);
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
    
    public static void WriteInsertNewGameNameInstructions(string errorMessage)
    {
        WriteErrorMessage(errorMessage);
        Console.WriteLine();
        Console.Write("Enter new <");
        Console.ForegroundColor = VisualizerHelper.ActionColor;
        Console.Write("name");
        Console.ResetColor();
        Console.Write("> for game or return <");
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
        Console.Write($"{gameInstance.GetNextMoveByPlayerName()}'s turn ({gameInstance.NextMoveBy})! ");
        Console.Write($"You have {gameInstance.GamePiecesLeft(gameInstance.NextMoveBy)} pieces left, ");
        Console.WriteLine($"{gameInstance.GameRoundsLeft} rounds left!");
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
        if (gameInstance.CanMovePiece() && !gameInstance.MovePieceModeOn)
        {
            WriteMovePieceInstructions();
        }
        if (gameInstance.CanMoveGrid() && !gameInstance.MovePieceModeOn)
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
            Console.Write(VisualizerHelper.BoardLineHorizontal);
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
                Console.Write(" " + Message.GamePieceAsString(gameInstance.GameBoard[x][y]) + " ");
                Console.ResetColor();
                Console.Write(VisualizerHelper.BoardLineVertical);
            }
            Console.WriteLine();
            if (y == gameInstance.DimY - 1) break;
            
            Console.Write($"   {VisualizerHelper.BoardCrossingWest}");
            for (int x = 0; x < gameInstance.DimX; x++)
            {
                Console.Write(VisualizerHelper.BoardLineHorizontal);
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
            Console.Write(VisualizerHelper.BoardLineHorizontal);
            if (x != gameInstance.DimX - 1)
            {
                Console.Write(VisualizerHelper.BoardCrossingNorth);
            }
        }
        Console.WriteLine(VisualizerHelper.BoardCornerSouthWest);
    }

    public static void DisplayGameOverMessage(string message)
    {
        Console.ForegroundColor = VisualizerHelper.MessageColor;
        Console.WriteLine(message);
        Console.ResetColor();
        Console.WriteLine(VisualizerHelper.GameOverGraphics);
        Console.Write("Press <");
        Console.ForegroundColor = VisualizerHelper.ActionColor;
        Console.Write("R");
        Console.ResetColor();
        Console.Write("> to return to main menu or <");
        Console.ForegroundColor = VisualizerHelper.ActionColor;
        Console.Write("G");
        Console.ResetColor();
        Console.Write("> to reset game: ");
    }

    public static void DisplayFinalRoundMessage()
    {
        Console.ForegroundColor = VisualizerHelper.ErrorMessageColor;
        Console.WriteLine(Message.FinalRoundMessage);
        Console.ResetColor();
    }

    public static void WriteGameRulesPage()
    {
        Console.Clear();
        Console.WriteLine("TIC-TAC-TWO - classical rules");
        Console.WriteLine(VisualizerHelper.Divider);
        Console.WriteLine("Tic-Tac-Two closely follows the rules of Tic-Tac-Toe. To begin the game the moveable grid is placed on the gameboard. It can be placed on any location on the board as long as the whole grid is on the gameboard. The game begins like Tic-Tac-Toe where both players alternate placing their first two pieces.\n\nAfter both players have placed 2 tokens, each player then has a choice of what they would like to do on their turn. They can do one of the following:\n\n1. Place one of the pieces that are still in their hand in one of the spots in the grid.\n2. Move one of their pieces that are in the grid to another spot in the grid.\n3. They may move the grid one spot in any direction (horizontally, vertically, or diagonally).\n\nWhen both players are out of pieces and there is still no winner, additional 2 rounds of game are available where players can make moves 2. or 3. on their turn.\n\nCheck out other configurations or create an entirely new configurations to bend the rules ;)");
        Console.WriteLine(VisualizerHelper.Divider);
        Console.WriteLine("Press any key to return to the main menu...");
    }
}