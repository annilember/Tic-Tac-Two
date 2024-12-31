using DTO;

namespace GameBrain;

public class Ai(TicTacTwoBrain gameInstance, EGamePiece maximizer, EGamePiece minimizer)
{
    private const int MaxDepth = 3;

    public void FindBestMove()
    {
        if (gameInstance.GameOver())
        {
            return;
        }
        
        const int bestVal = -1000;
        var bestPlacePieceMove = new Dictionary<string, int>
        {
            { "Value", bestVal },
            { "X", -1 },
            { "Y", -1 }
        };
        if (gameInstance.HasGamePiece(gameInstance.NextMoveBy))
        {
            bestPlacePieceMove = FindBestPlacePieceMove(bestPlacePieceMove);
        }

        var bestGridMove = new Dictionary<string, int>()
        {
            { "Value", bestVal },
            { "Direction", 0 }
        };
        if (gameInstance.CanMoveGrid())
        {
            bestGridMove = FindBestGridMove(bestGridMove);
        }

        if (gameInstance.HasGamePiece(gameInstance.NextMoveBy) && 
            bestPlacePieceMove["Value"] > bestGridMove["Value"] || 
            !gameInstance.CanMoveGrid())
        {
            gameInstance.MakeAMove(bestPlacePieceMove["X"], bestPlacePieceMove["Y"]);
        }
        else
        {
            var direction = (EMoveGridDirection)Enum
                .ToObject(typeof(EMoveGridDirection), bestGridMove["Direction"]);
            gameInstance.MakeGridMove(direction);
        }
    }

    private Dictionary<string, int> FindBestGridMove(Dictionary<string, int> bestMove)
    {
        var directions = Enum
            .GetValues<EMoveGridDirection>().ToList();
        var i = 0;

        foreach (var direction in directions)
        {
            gameInstance.MakeGridMove(direction);
            if (gameInstance.GridWasMoved())
            {
                var moveVal = Minimax(MaxDepth, -1000, +1000, false);
                if (moveVal > bestMove["Value"])
                {
                    bestMove["Value"] = moveVal;
                    bestMove["Direction"] = i;
                }
            }
            gameInstance.CancelGridMove();
            i++;
        }

        return bestMove;
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
        if (
            Math.Abs(boardVal) == 10 ||
            depth == 0 ||
            gameInstance.CheckForDraw() ||
            gameInstance.IsGameOverAnyway()
        )
        {
            return boardVal;
        }
        
        var random = new Random();
        var canPlacePiece = gameInstance.HasGamePiece(gameInstance.NextMoveBy);
        var shouldMoveGrid = gameInstance.CanMoveGrid() && (!canPlacePiece || random.Next(2) == 0);

        if (isMaximizer)
        {
            var highestVal = -1000;
            
            if (shouldMoveGrid && gameInstance.CanMoveGrid())
            {
                var directions = Enum.GetValues<EMoveGridDirection>().ToList();
                foreach (var direction in directions)
                {
                    gameInstance.MakeGridMove(direction);
                    if (gameInstance.GridWasMoved())
                    {
                        highestVal = Math.Max(highestVal, Minimax(depth - 1, alpha, beta, !isMaximizer));
                        alpha = Math.Max(alpha, highestVal);
                    }
                    gameInstance.CancelGridMove();
                    if (alpha >= beta)
                    {
                        return highestVal;
                    }
                }
            }
            else if (canPlacePiece)
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
                            highestVal = Math.Max(highestVal, Minimax(depth - 1, alpha, beta, !isMaximizer));
                            gameInstance.CancelMove(x, y);
                            alpha = Math.Max(alpha, highestVal);
                            if (alpha >= beta)
                            {
                                return highestVal;
                            }
                        }
                    }
                }
            }

            return highestVal;
        }
        else
        {
            var lowestVal = 1000;

            if (shouldMoveGrid && gameInstance.CanMoveGrid())
            {
                var directions = Enum.GetValues<EMoveGridDirection>().ToList();
                foreach (var direction in directions)
                {
                    gameInstance.MakeGridMove(direction);
                    if (gameInstance.GridWasMoved())
                    {
                        lowestVal = Math.Min(lowestVal, Minimax(depth - 1, alpha, beta, !isMaximizer));
                        beta = Math.Min(beta, lowestVal);
                    }
                    gameInstance.CancelGridMove();
                    if (beta <= alpha)
                    {
                        return lowestVal;
                    }
                }
            }
            else if (canPlacePiece)
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
                            lowestVal = Math.Min(lowestVal, Minimax(depth - 1, alpha, beta, !isMaximizer));
                            gameInstance.CancelMove(x, y);
                            beta = Math.Min(beta, lowestVal);
                            if (beta <= alpha)
                            {
                                return lowestVal;
                            }
                        }
                    }
                }
            }

            return lowestVal;
        }
    }

    private int Evaluate()
    {
        if (gameInstance.CheckForDraw() || gameInstance.IsGameOverAnyway())
        {
            return 0;
        }

        if (gameInstance.CheckForWinner() == maximizer)
        {
            return +10;
        }

        if (gameInstance.CheckForWinner() == minimizer)
        {
            return -10;
        }

        return 0;
    }
}
