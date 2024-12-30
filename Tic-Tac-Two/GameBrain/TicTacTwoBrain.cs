using System.Text.Json;
using Domain;
using DTO;

namespace GameBrain;

public class TicTacTwoBrain
{
    private readonly GameState _gameState;
    private readonly GameConfiguration _gameConfiguration;
    public readonly SavedGame SavedGame;

    public TicTacTwoBrain(SavedGame savedGame, GameConfiguration config)
    {
        _gameConfiguration = config;
        _gameState = JsonSerializer.Deserialize<GameState>(savedGame.State)!;
        SavedGame = savedGame;
    }

    public string GetGameStateJson() => _gameState.ToString();
    
    public int DimX => _gameState.GameBoard.Length;
    
    public int DimY => _gameState.GameBoard[0].Length;
    
    public EGamePiece[][] GameBoard => GetBoard();
    
    public bool[][] GameGrid => GetGrid();
    
    public EGamePiece GetNextMoveBy()
    {
        return _gameState.NextMoveBy;
    }

    public string GetNextMoveByPlayerName() => GetPlayerName(_gameState.NextMoveBy);

    public string GetPlayerName(EGamePiece piece)
    {
        return piece switch
        {
            EGamePiece.X => _gameState.PlayerXName,
            EGamePiece.O => _gameState.PlayerOName,
            _ => ""
        };
    }
    
    public EPlayerType GetNextMoveByPlayerType()
    {
        return _gameState.NextMoveBy switch
        {
            EGamePiece.X => _gameState.PlayerXType,
            EGamePiece.O => _gameState.PlayerOType,
            _ => throw new ArgumentException(message: "Player type not defined", paramName: nameof(_gameState.NextMoveBy)),
        };
    }
    
    public EChosenMove GetChosenMove()
    {
        if (_gameState.MoveGridModeOn)
        {
            return EChosenMove.MoveGrid;
        }
        if (_gameState.RemovePieceModeOn)
        {
            return EChosenMove.RemovePiece;
        }
        if (_gameState.MovePieceModeOn)
        {
            return EChosenMove.MovePiece;
        }
        return EChosenMove.PlacePiece;
    }
    
    public bool PlacePieceModeOn()
    {
        return GetChosenMove() == EChosenMove.PlacePiece;
    }
        
    public bool HasGamePiece(EGamePiece piece)
    {
        return piece switch
        {
            EGamePiece.X => _gameState.NumberOfPiecesLeftX > 0,
            EGamePiece.O => _gameState.NumberOfPiecesLeftO > 0,
            _ => false
        };
    }
    
    public int GamePiecesLeft(EGamePiece piece)
    {
        return piece switch
        {
            EGamePiece.X => _gameState.NumberOfPiecesLeftX,
            EGamePiece.O => _gameState.NumberOfPiecesLeftO,
            _ => 0
        };
    }
    
    public void MakeAiMove()
    {
        var player = _gameState.NextMoveBy;
        var opponent = _gameState.NextMoveBy == EGamePiece.X ? EGamePiece.O : EGamePiece.X;
        var ai = new Ai(this, player, opponent);
        ai.FindBestMove();
        // Console.WriteLine($"AI chose <{bestMove[0]}, {bestMove[1]}>");
        // MakeAMove(bestMove[0], bestMove[1]);
    }
    
    public bool MakeAMove(int x, int y)
    {
        if (MovePieceModeOn && 
            x == _gameState.RemovedPieceLocation[0] && 
            y == _gameState.RemovedPieceLocation[1])
        {
            return false;
        }
        
        if (_gameState.GameBoard[x][y] != EGamePiece.Empty)
        {
            return false;
        }
        _gameState.GameBoard[x][y] = _gameState.NextMoveBy;
        
        switch (_gameState.NextMoveBy)
        {
            case EGamePiece.X:
                _gameState.NumberOfPiecesLeftX--;
                break;
            case EGamePiece.O:
                _gameState.NumberOfPiecesLeftO--;
                break;
        }

        CountAsMove();

        return true;
    }
    
    public void CancelMove(int x, int y)
    {
        _gameState.GameBoard[x][y] = EGamePiece.Empty;
        _gameState.NextMoveBy = _gameState.NextMoveBy == EGamePiece.X ? EGamePiece.O : EGamePiece.X;
        
        switch (_gameState.NextMoveBy)
        {
            case EGamePiece.X:
                _gameState.NumberOfPiecesLeftX++;
                break;
            case EGamePiece.O:
                _gameState.NumberOfPiecesLeftO++;
                _gameState.GameRoundNumber--;
                _gameState.GameRoundsLeft++;
                break;
        }
    }

    private void CountAsMove()
    {
        if (_gameState.NextMoveBy == EGamePiece.O)
        {
            _gameState.GameRoundNumber++;
            _gameState.GameRoundsLeft--;
        }
        _gameState.NextMoveBy = _gameState.NextMoveBy == EGamePiece.X ? EGamePiece.O : EGamePiece.X;
    }
    
    public bool[][] GridMovingArea => _gameState.GridMovingArea;

    public bool MoveGridModeOn => _gameState.MoveGridModeOn;
    
    public bool CanMoveGrid()
    {
        return GameRoundNumber > _gameConfiguration.MoveGridAfterNMoves &&
               (
                   _gameConfiguration.BoardSizeWidth > _gameConfiguration.GridSizeWidth ||
                   _gameConfiguration.BoardSizeHeight > _gameConfiguration.GridSizeHeight
               )
               && !MovePieceModeOn;
    }

    public void ActivateMoveGridMode()
    {
        SaveCurrentGridState();
        _gameState.MoveGridModeOn = true;
        _gameState.GridMovingArea = SetGridMovingArea();
    }
    
    public void DeActivateMoveGridMode()
    {
        _gameState.MoveGridModeOn = false;
        CountAsMove();
    }

    public void RestoreGridState()
    {
        _gameState.MoveGridModeOn = false;
        _gameState.GameGrid = _gameState.MoveGridStartState;
        _gameState.GridStartPosX = _gameState.MoveGridStartStateLocation[0];
        _gameState.GridStartPosY = _gameState.MoveGridStartStateLocation[1];
    }

    public void MakeGridMove(EMoveGridDirection direction)
    {
        ActivateMoveGridMode();
        MoveGrid(direction);
        DeActivateMoveGridMode();
    }

    public void CancelGridMove()
    {
        RestoreGridState();
        _gameState.NextMoveBy = _gameState.NextMoveBy == EGamePiece.X ? EGamePiece.O : EGamePiece.X;
        if (_gameState.NextMoveBy == EGamePiece.X) return;
        _gameState.GameRoundNumber--;
        _gameState.GameRoundsLeft++;
    }

    private EGamePiece[][] GetBoard()
    {
        var copyOfBoard = new EGamePiece[DimX][];
        
        for (var x = 0; x < DimX; x++)
        {
            copyOfBoard[x] = new EGamePiece[DimY];
            for (var y = 0; y < DimY; y++)
            {
                copyOfBoard[x][y] = _gameState.GameBoard[x][y];
            }
        }
        
        return copyOfBoard;
    }
    
    private bool[][] GetGrid()
    {
        var copyOfGrid = new bool[DimX][];
        
        for (var x = 0; x < DimX; x++)
        {
            copyOfGrid[x] = new bool[DimY];
            for (var y = 0; y < DimY; y++)
            {
                copyOfGrid[x][y] = _gameState.GameGrid[x][y];
            }
        }
        return copyOfGrid;
    }
    
    private void SaveCurrentGridState()
    {
        _gameState.MoveGridStartState = GetGrid();
        _gameState.MoveGridStartStateLocation[0] = _gameState.GridStartPosX;
        _gameState.MoveGridStartStateLocation[1] = _gameState.GridStartPosY;
    }
    
    private bool[][] SetGridMovingArea()
    {
        var startPosX = _gameState.GridStartPosX;
        var startPosY = _gameState.GridStartPosY;
        var endPosX = startPosX + _gameConfiguration.GridSizeWidth;
        var endPosY = startPosY + _gameConfiguration.GridSizeHeight;
        
        if (startPosX > 0)
        {
            startPosX--;
        }
        if (startPosY > 0)
        {
            startPosY--;
        }
        if (endPosX < DimX)
        {
            endPosX++;
        }
        if (endPosY < DimY)
        {
            endPosY++;
        }
        
        return _gameState.CreateGrid(DimX, DimY, startPosX, startPosY, endPosX, endPosY);
    }
    
    private int GridMovingLowerBoundX { get; set; }
    private int GridMovingLowerBoundY { get; set; }
    private int GridMovingUpperBoundX { get; set; }
    private int GridMovingUpperBoundY { get; set; }

    private void SetGridMovingBounds()
    {
        var startPosX = _gameState.MoveGridStartStateLocation[0];
        var startPosY = _gameState.MoveGridStartStateLocation[1];
        var endPosX = startPosX + _gameConfiguration.GridSizeWidth;
        var endPosY = startPosY + _gameConfiguration.GridSizeHeight;
        
        if (startPosX > 0)
        {
            startPosX--;
        }
        if (startPosY > 0)
        {
            startPosY--;
        }
        if (endPosX < DimX)
        {
            endPosX++;
        }
        if (endPosY < DimY)
        {
            endPosY++;
        }
        
        GridMovingLowerBoundX = startPosX;
        GridMovingLowerBoundY = startPosY;
        GridMovingUpperBoundX = endPosX;
        GridMovingUpperBoundY = endPosY;
    }

    public void MoveGrid(EMoveGridDirection direction)
    {
        var startPosX = _gameState.GridStartPosX;
        var startPosY = _gameState.GridStartPosY;
        var endPosX = startPosX + _gameConfiguration.GridSizeWidth;
        var endPosY = startPosY + _gameConfiguration.GridSizeHeight;
        SetGridMovingBounds();
        
        switch (direction)
        {
            case EMoveGridDirection.Up:
                if (startPosY > 0 && startPosY > GridMovingLowerBoundY)
                {
                    startPosY--;
                }
                break;
            case EMoveGridDirection.Down:
                if (endPosY < DimY && endPosY < GridMovingUpperBoundY)
                {
                    startPosY++;
                }
                break;
            case EMoveGridDirection.Left:
                if (startPosX > 0 && startPosX > GridMovingLowerBoundX)
                {
                    startPosX--;
                }
                break;
            case EMoveGridDirection.Right:
                if (endPosX < DimX && endPosX < GridMovingUpperBoundX)
                {
                    startPosX++;
                }
                break;
        }
        endPosX = startPosX + _gameConfiguration.GridSizeWidth;
        endPosY = startPosY + _gameConfiguration.GridSizeHeight;
        
        _gameState.GameGrid = _gameState.CreateGrid(
            DimX, DimY, 
            startPosX, startPosY, 
            endPosX, endPosY);
        
        _gameState.GridStartPosX = startPosX;
        _gameState.GridStartPosY = startPosY;
    }
    
    public bool GridWasMoved()
    {
        for (int x = 0; x < DimX; x++)
        {
            for (int y = 0; y < DimY; y++)
            {
                if (_gameState.MoveGridStartState[x][y] != GameGrid[x][y])
                {
                    return true;
                }
            }
        }

        return false;
    }
    
    public bool CanMovePiece()
    {
        return GameRoundNumber > _gameConfiguration.MovePieceAfterNMoves;
    }
    
    public bool RemovePieceModeOn => _gameState.RemovePieceModeOn;
    
    public void ActivateRemovePieceMode()
    {
        _gameState.RemovePieceModeOn = true;
    }
    
    public void DeActivateRemovePieceMode()
    {
        _gameState.RemovePieceModeOn = false;
    }
    
    public bool RemovedPieceCoordinateClash(int x, int y)
    {
        if (!MovePieceModeOn) return false;
        return x == _gameState.RemovedPieceLocation[0] && 
               y == _gameState.RemovedPieceLocation[1];
    }
    
    public bool RemovePiece(int x, int y)
    {
        if (_gameState.GameBoard[x][y] != _gameState.NextMoveBy) return false;
        _gameState.GameBoard[x][y] = EGamePiece.Empty;
        _gameState.RemovedPieceLocation[0] = x;
        _gameState.RemovedPieceLocation[1] = y;
        
        switch (_gameState.NextMoveBy)
        {
            case EGamePiece.X:
                _gameState.NumberOfPiecesLeftX++;
                break;
            case EGamePiece.O:
                _gameState.NumberOfPiecesLeftO++;
                break;
        }
        return true;
    }
    
    public bool MovePieceModeOn => _gameState.MovePieceModeOn;

    public void ActivateMovePieceMode()
    {
        _gameState.MovePieceModeOn = true;
    }
    
    public void DeActivateMovePieceMode()
    {
        _gameState.MovePieceModeOn = false;
    }
    
        public bool GameOver()
    {
        return !string.IsNullOrEmpty(GetWinnerName()) || IsGameOverAnyway();
    }
    
    public int GameRoundNumber => _gameState.GameRoundNumber;

    public int GetRoundsLeft()
    {
        return _gameState.GameRoundsLeft;
    }

    private string GetWinnerName()
    {
        return CheckForWinner() switch
        {
            EGamePiece.X => _gameState.PlayerXName,
            EGamePiece.O => _gameState.PlayerOName,
            _ => ""
        };
    }
    
    public bool CheckForDraw()
    {
        return CheckForWinnerByPlayer(EGamePiece.X) && CheckForWinnerByPlayer(EGamePiece.O);
    }

    public EGamePiece CheckForWinner()
    {
        if (CheckForWinnerByPlayer(EGamePiece.X))
        {
            return EGamePiece.X;
        }
        if (CheckForWinnerByPlayer(EGamePiece.O))
        {
            return EGamePiece.O;
        }

        return EGamePiece.Empty;
    }
    
    private bool CheckForWinnerByPlayer(EGamePiece player)
    {
        for (int x = 0; x < DimX; x++)
        {
            var countRowStrike = 0;
            for (int y = 0; y < DimY; y++)
            {
                countRowStrike = CountRowOrColumnStrike(player, countRowStrike, x, y);
                if (countRowStrike == _gameConfiguration.WinCondition ||
                    CheckDiagonalStreaks(player, x, y, (i) => i + 1) ||
                    CheckDiagonalStreaks(player, x, y, (i) => i - 1)) 
                {
                    return true;
                }
            }
        }
        
        for (int y = 0; y < DimY; y++)
        {
            var countColStrike = 0;
            for (int x = 0; x < DimX; x++)
            {
                countColStrike = CountRowOrColumnStrike(player, countColStrike, x, y);
                if (
                    countColStrike == _gameConfiguration.WinCondition
                ) 
                {
                    return true;
                }
            }
        }
        return false;
    }
    
    private int CountRowOrColumnStrike(EGamePiece player, int countStrike, int x, int y)
    {
        if (_gameState.GameGrid[x][y])
        {
            if (_gameState.GameBoard[x][y] == player) countStrike++;
            else countStrike = 0;
        }
        else countStrike = 0;

        return countStrike;
    }
    
    private bool CheckDiagonalStreaks(EGamePiece player, int x, int y, Func<int, int> action)
    {
        var countStrike = 0;
        while (true)
        {
            try
            {
                if (_gameState.GameGrid[x][y] && _gameState.GameBoard[x][y] == player) countStrike++;
                if (countStrike == _gameConfiguration.WinCondition) return true;
                x++;
                y = action(y);
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }

    public bool IsGameOverAnyway()
    {
        return (_gameState.GameRoundsLeft == 0 && _gameState.NextMoveBy == EGamePiece.X) ||
               (GameBoardEmptySpacesCount() == 0 && !CanMoveGrid());
    }
    
    private int GameBoardEmptySpacesCount()
    {
        var count = 0;
        for (int x = 0; x < DimX; x++)
        {
            for (int y = 0; y < DimY; y++)
            {
                if (_gameState.GameBoard[x][y] == EGamePiece.Empty)
                {
                    count++;
                }
            }
        }

        return count;
    }
    
    public string GetGameOverMessage()
    {
        if (CheckForDraw())
        {
            return Message.GameOverDrawMessage;
        } 
        if (!string.IsNullOrEmpty(GetWinnerName()))
        {
            return Message.GetTheWinnerIsMessage(GetWinnerName());
        }
        if (IsGameOverAnyway())
        {
            return Message.GameOverNoMoreRoundsMessage;
        }
        return string.Empty;
    }

    public void ResetGame()
    {
        var gameBoard = new EGamePiece[_gameConfiguration.BoardSizeWidth][];
        var gameGrid = _gameState.InitializeGrid(_gameConfiguration);
        
        for (var i = 0; i < gameBoard.Length; i++)
        {
            gameBoard[i] = new EGamePiece[_gameConfiguration.BoardSizeHeight];
        }
        
        _gameState.GameBoard = gameBoard;
        _gameState.GameGrid = gameGrid;
        _gameState.MoveGridModeOn = false;
        _gameState.RemovePieceModeOn = false;
        _gameState.MovePieceModeOn = false;
        _gameState.GridStartPosX = _gameConfiguration.GridStartPosX;
        _gameState.GridStartPosY = _gameConfiguration.GridStartPosY;
        _gameState.NextMoveBy = EGamePiece.X;
        _gameState.NumberOfPiecesLeftX = _gameConfiguration.NumberOfPieces;
        _gameState.NumberOfPiecesLeftO = _gameConfiguration.NumberOfPieces;
        _gameState.GameRoundNumber = 1;
        _gameState.GameRoundsLeft = _gameConfiguration.MaxGameRounds;
    }
}
