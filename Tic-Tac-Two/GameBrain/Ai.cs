using DTO;

namespace GameBrain;

public class Ai(TicTacTwoBrain gameInstance, EGamePiece maximizer, EGamePiece minimizer)
{
    private const int MaxDepth = 3;

    public void FindBestMove() 
    { 
        var bestVal = -1000; 
        // int[] bestMove = [-1,-1]; 
        var bestMove = new Dictionary<string, int>
        {
            { "Value", bestVal },
            { "X", -1 },
            { "Y", -1 }
        };
        bestMove = FindBestPlacePieceMove(bestMove);
        bestVal = bestMove["Value"];
        var moveType = "Place piece";

        var bestGridVal = -1000;
        var bestGridMove = new Dictionary<string, int>();
        if (gameInstance.CanMoveGrid())
        {
            bestGridMove = FindBestGridMove(bestVal);
            bestGridVal = bestGridMove["Value"];
        }
        
        bestVal = Math.Max(bestVal, bestGridVal);
        if (bestGridVal > bestVal)
        {
            moveType = "Move grid";
        }

        switch (moveType)
        {
            case "Place piece":
                gameInstance.MakeAMove(bestMove["X"], bestMove["Y"]);
                break;
            case "Move grid":
            {
                var direction = (EMoveGridDirection)Enum
                    .ToObject(typeof(EMoveGridDirection), bestGridMove["Direction"]);
                gameInstance.MakeGridMove(direction);
                break;
            }
        }
    }
    
    private Dictionary<string, int> FindBestGridMove(int bestVal)
    {
        var directions = Enum
            .GetValues(typeof(EMoveGridDirection))
            .Cast<EMoveGridDirection>().ToList();
        var bestDirection = 0;
        var i = 0;

        foreach (var direction in directions)
        {
            gameInstance.MakeGridMove(direction);
            var moveVal = Minimax(MaxDepth, -1000, +1000, false);
            gameInstance.CancelGridMove();
                
            if (moveVal > bestVal)
            {
                bestVal = moveVal;
                bestDirection = i;
            }
            i++;
        }

        return new Dictionary<string, int>
        {
            { "Value", bestVal },
            { "Direction",  bestDirection }
        };
    }

    private Dictionary<string, int> FindBestPlacePieceMove(Dictionary<string, int> bestMove)
    {
        for (int x = 0; x < gameInstance.DimX; x++) 
        { 
            for (int y = 0; y < gameInstance.DimY; y++) 
            { 
                if (
                    gameInstance.GameBoard[x][y] == EGamePiece.Empty &&
                    gameInstance.GameGrid[x][y]
                ) 
                { 
                    gameInstance.MakeAMove(x, y);
                    var moveVal = Minimax(MaxDepth, -1000, +1000, false);
                    // Console.WriteLine($"<{x}, {y}> value: {moveVal}");
                    // Console.WriteLine($"Evaluation: {Evaluate()}");
                    // DrawBoard(gameInstance);
                    gameInstance.CancelMove(x, y);
                    
                    if (moveVal > bestMove["Value"]) 
                    { 
                        bestMove["X"] = x; 
                        bestMove["Y"] = y; 
                        bestMove["Value"] = moveVal; 
                    } 
                } 
            } 
        }

        return bestMove;
    }
    
    private int Minimax(int depth, int alpha, int beta, bool isMaximizer)
    {
        var boardVal = Evaluate();
        // Console.WriteLine($"Evaluation: {boardVal}");
        if (
            Math.Abs(boardVal) == 10 || 
            depth == 0 || 
            gameInstance.CheckForDraw() || 
            gameInstance.IsGameOverAnyway()
            ) {
            return boardVal;
        }
        
        var directions = Enum
            .GetValues(typeof(EMoveGridDirection))
            .Cast<EMoveGridDirection>().ToList();
        
        
        if (isMaximizer)
        { 
            var highestVal = -1000;
            
            for (int x = 0; x < gameInstance.DimX; x++) 
            { 
                for (int y = 0; y < gameInstance.DimY; y++)
                {
                    if (
                        gameInstance.GameBoard[x][y] == EGamePiece.Empty &&
                        gameInstance.GameGrid[x][y]
                    )
                    {
                        gameInstance.MakeAMove(x, y);
                        highestVal = Math.Max(highestVal, Minimax(depth - 1, alpha, beta, !isMaximizer)); 
                        gameInstance.CancelMove(x, y);
                        alpha = Math.Max(alpha, highestVal);
                        if (alpha >= beta) {
                            return highestVal;
                        }
                    }
                } 
            }

            if (!gameInstance.CanMoveGrid()) return highestVal;
            
            foreach (var direction in directions)
            {
                gameInstance.MakeGridMove(direction);
                highestVal = Math.Max(highestVal, Minimax(depth - 1, alpha, beta, !isMaximizer)); 
                gameInstance.CancelGridMove();
                alpha = Math.Max(alpha, highestVal);
                if (alpha >= beta) {
                    return highestVal;
                }
            }

            return highestVal; 
        } 
        
        else
        { 
            var lowestVal = 1000; 
            
            for (int x = 0; x < gameInstance.DimX; x++) 
            { 
                for (int y = 0; y < gameInstance.DimY; y++)
                {
                    if (
                        gameInstance.GameBoard[x][y] == EGamePiece.Empty &&
                        gameInstance.GameGrid[x][y]
                    )
                    {
                        gameInstance.MakeAMove(x, y);
                        lowestVal = Math.Min(lowestVal, Minimax(depth - 1, alpha, beta, !isMaximizer)); 
                        gameInstance.CancelMove(x, y);
                        beta = Math.Min(beta, lowestVal);
                        if (beta <= alpha) {
                            return lowestVal;
                        }
                    }
                } 
            }

            if (!gameInstance.CanMoveGrid()) return lowestVal;
            
            foreach (var direction in directions)
            {
                gameInstance.MakeGridMove(direction);
                lowestVal = Math.Min(lowestVal, Minimax(depth - 1, alpha, beta, !isMaximizer)); 
                gameInstance.CancelGridMove();
                beta = Math.Min(beta, lowestVal);
                if (beta <= alpha) {
                    return lowestVal;
                }
            }


            return lowestVal; 
        } 
    }

    private int Evaluate()
    {
        if (gameInstance.CheckForDraw() || gameInstance.IsGameOverAnyway())
        {
            // Console.WriteLine($"Draw! or Game over!");
            return 0;
        }
        if (gameInstance.CheckForWinner() == maximizer)
        {
            // Console.WriteLine($"Maximizer won");
            return +10;
        }
        if (gameInstance.CheckForWinner() == minimizer)
        {
            // Console.WriteLine($"Minimizer won");
            return -10;
        }
        
        // Console.WriteLine($"None of the above!");
        // Console.WriteLine("Check for winner returns: " + gameInstance.CheckForWinner());
        return 0;
    }
    
    public static void DrawBoard(TicTacTwoBrain gameInstance)
    {

        Console.Write("   ");
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
        
        Console.Write($"   +");
        for (int x = 0; x < gameInstance.DimX; x++)
        {
            Console.Write("---");
            if (x != gameInstance.DimX - 1)
            {
                Console.Write("+");
            }
        }
        Console.WriteLine("+");
        
        

        for (int y = 0; y < gameInstance.DimY; y++)
        {
            if (y < 10)
            {
                Console.Write(" ");
            }
            Console.Write($"{y}");
            Console.ResetColor();
            Console.Write($" |");
            for (int x = 0; x < gameInstance.DimX; x++)
            {
                if (gameInstance.MoveGridModeOn && gameInstance.GridMovingArea[x][y])
                {
                    Console.BackgroundColor = ConsoleColor.DarkYellow;
                }
                if (gameInstance.GameGrid[x][y])
                {
                    Console.BackgroundColor = ConsoleColor.DarkYellow;
                }
                Console.Write(" " + gameInstance.GameBoard[x][y] + " ");
                Console.ResetColor();
                Console.Write("---");
            }
            Console.WriteLine();
            if (y == gameInstance.DimY - 1) break;
            
            Console.Write($"   +");
            for (int x = 0; x < gameInstance.DimX; x++)
            {
                Console.Write("---");
                if (x != gameInstance.DimX - 1)
                {
                    Console.Write("+");
                }
            }
            Console.WriteLine("+");
        }
        
        
        
        Console.Write($"   +");
        for (int x = 0; x < gameInstance.DimX; x++)
        {
            Console.Write("---");
            if (x != gameInstance.DimX - 1)
            {
                Console.Write("+");
            }
        }
        Console.WriteLine("+");
    }
}