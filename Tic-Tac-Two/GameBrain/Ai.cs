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
        
        if (gameInstance.CanMoveGrid() && MakeWinningGridMove())
        {
            return;
        }

        if (gameInstance.CanMovePiece() && MakeWinningMovePieceMove())
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
            gameInstance.MakeAMove(bestPlacePieceMove["X"], bestPlacePieceMove["Y"]);
            return;
        }
        
        var rnd = new Random();
        var chosenMove  = rnd.Next(0, 2);
        if (chosenMove == 0 && gameInstance.CanMoveGrid() || !gameInstance.CanMovePiece())
        {
            MakeRandomGridMove();
        }
        else if (gameInstance.CanMovePiece())
        {
            MakeRandomMovePieceMove();
        }
    }

    private bool MakeWinningGridMove()
    {
        var player = gameInstance.NextMoveBy;
        var directions = GetGridMoveDirections();

        foreach (var direction in directions)
        {
            gameInstance.MakeAiGridMove(direction[0], direction[1]);

            if (gameInstance.GridWasMoved() && gameInstance.CheckForWinnerByPlayer(player))
            {
                return true;
            }

            gameInstance.CancelGridMove();
        }
        return false;
    }
    
    private bool MakeWinningMovePieceMove()
    {
        var player = gameInstance.NextMoveBy;

        for (int x = 0; x < gameInstance.DimX; x++)
        {
            for (int y = 0; y < gameInstance.DimY; y++)
            {
                if (
                    gameInstance.GameBoard[x][y] == player
                )
                {
                    gameInstance.ActivateRemovePieceMode();
                    gameInstance.RemovePiece(x, y);
                    gameInstance.DeActivateRemovePieceMode();

                    if (PlaceWinningPiece())
                    {
                        return true;
                    }
                    
                    gameInstance.CancelRemovePiece(x, y);
                }
            }
        }
        
        return false;
    }

    private bool PlaceWinningPiece()
    {
        var player = gameInstance.NextMoveBy;
        gameInstance.ActivateMovePieceMode();
        for (int x = 0; x < gameInstance.DimX; x++)
        {
            for (int y = 0; y < gameInstance.DimY; y++)
            {
                if (
                    gameInstance.GameBoard[x][y] == EGamePiece.Empty
                )
                {
                    var moveMade = gameInstance.MakeAMove(x, y);
                    switch (moveMade)
                    {
                        case true when gameInstance.CheckForWinnerByPlayer(player):
                            gameInstance.DeActivateMovePieceMode();
                            return true;
                        case true:
                            gameInstance.CancelMove(x, y);
                            break;
                    }
                }
            }
        }
        gameInstance.DeActivateMovePieceMode();
        return false;
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
            gameInstance.IsGameOverAnyway() ||
            !gameInstance.HasGamePiece(gameInstance.NextMoveBy)
        )
        {
            return boardVal;
        }

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
                        if (alpha >= beta)
                        {
                            return highestVal;
                        }
                    }
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
                        if (beta <= alpha)
                        {
                            return lowestVal;
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
    
    private void MakeRandomGridMove()
    {
        var directions = GetGridMoveDirections();
        var rnd = new Random();
        var direction  = rnd.Next(0, directions.Length);
        gameInstance.MakeAiGridMove(directions[direction][0], directions[direction][1]);
    }
    
    private EMoveGridDirection[][] GetGridMoveDirections()
    {
        return [
            [EMoveGridDirection.Left, EMoveGridDirection.None],
            [EMoveGridDirection.Left, EMoveGridDirection.Up],
            [EMoveGridDirection.Up, EMoveGridDirection.None],
            [EMoveGridDirection.Up, EMoveGridDirection.Right],
            [EMoveGridDirection.Right, EMoveGridDirection.None],
            [EMoveGridDirection.Right, EMoveGridDirection.Down],
            [EMoveGridDirection.Down, EMoveGridDirection.None],
            [EMoveGridDirection.Down, EMoveGridDirection.Left]
        ];
    }
    
    private void MakeRandomMovePieceMove()
    {
        var player = gameInstance.NextMoveBy;
        var rnd = new Random();
        var chosenPiece  = rnd.Next(0, gameInstance.GamePiecesPlaced(player));
        var pieceFinder = 0;

        for (int x = 0; x < gameInstance.DimX; x++)
        {
            for (int y = 0; y < gameInstance.DimY; y++)
            {
                if (gameInstance.GameBoard[x][y] == player && pieceFinder == chosenPiece)
                {
                    
                    gameInstance.ActivateRemovePieceMode();
                    gameInstance.RemovePiece(x, y);
                    gameInstance.DeActivateRemovePieceMode();
                    PlacePieceInRandomSlot();
                }
                else if (gameInstance.GameBoard[x][y] == player)
                {
                    pieceFinder++;
                }
            }
        }
    }
    
    private void PlacePieceInRandomSlot()
    {
        var rnd = new Random();
        var chosenSlot  = rnd.Next(0, gameInstance.GameBoardEmptySpacesCount() - 1);
        var slotFinder = 0;
        gameInstance.ActivateMovePieceMode();
        
        for (int x = 0; x < gameInstance.DimX; x++)
        {
            for (int y = 0; y < gameInstance.DimY; y++)
            {
                if (gameInstance.GameBoard[x][y] == EGamePiece.Empty && 
                    !gameInstance.RemovedPieceCoordinateClash(x, y) &&
                    slotFinder == chosenSlot)
                {
                    gameInstance.MakeAMove(x, y);
                    gameInstance.DeActivateMovePieceMode();
                }
                else if (gameInstance.GameBoard[x][y] == EGamePiece.Empty && 
                         !gameInstance.RemovedPieceCoordinateClash(x, y))
                {
                    slotFinder++;
                }
            }
        }
    }
}