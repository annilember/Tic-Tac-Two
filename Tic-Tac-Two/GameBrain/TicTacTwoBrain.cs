using System.Text.Json;
using Domain;
using DTO;

namespace GameBrain;

public class TicTacTwoBrain
{
    private readonly GameState _gameState;
    private readonly GameConfiguration _gameConfiguration;

    public TicTacTwoBrain(GameConfiguration gameConfiguration, EGameMode mode, string playerXName, string playerOName)
    {
        _gameConfiguration = gameConfiguration;
        _gameState = new GameState(
            gameConfiguration,
            mode,
            playerXName,
            playerOName);
    }
    
    public TicTacTwoBrain(SavedGame savedGame, GameConfiguration config)
    {
        _gameConfiguration = config;
        _gameState = JsonSerializer.Deserialize<GameState>(savedGame.State)!;
    }
    
    public string GetGameModeName() => _gameState.GameMode.ToString();

    public string GetGameStateJson() => _gameState.ToString();

    public string GetGameConfigName() => _gameConfiguration.Name;
    
    public GameConfiguration GetGameConfig() => _gameConfiguration;

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
    
    public string GetWinnerName()
    {
        return CheckForWinner() switch
        {
            EGamePiece.X => _gameState.PlayerXName,
            EGamePiece.O => _gameState.PlayerOName,
            _ => ""
        };
    }

    public bool GameOver()
    {
        return !string.IsNullOrEmpty(GetWinnerName()) || IsGameOverAnyway();
    }

    public bool IsGameOverAnyway()
    {
        return (_gameState.GameRoundsLeft == 0 && _gameState.NextMoveBy == EGamePiece.X) ||
               (GameBoardEmptySpacesCount() == 0 && !CanMoveGrid());
    }
    
    public EGamePiece[][] GameBoard
    {
        get => GetBoard();
        private set => _gameState.GameBoard = value;
    }
    
    public bool[][] GameGrid
    {
        get => GetGrid();
        private set => _gameState.GameGrid = value;
    }
    
    public bool MoveGridModeOn
    {
        get => GetMoveGridModeOn();
        set => _gameState.MoveGridModeOn = value;
    }

    private bool GetMoveGridModeOn()
    {
        return _gameState.MoveGridModeOn;
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

    public bool[][] GridMovingArea => _gameState.GridMovingArea;

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
    
    public int DimX => _gameState.GameBoard.Length;
    public int DimY => _gameState.GameBoard[0].Length;

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

    private int GridMovingLowerBoundX { get; set; }
    private int GridMovingLowerBoundY { get; set; }
    private int GridMovingUpperBoundX { get; set; }
    private int GridMovingUpperBoundY { get; set; }

    public EGamePiece GetNextMoveBy()
    {
        return _gameState.NextMoveBy;
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

    private void CountAsMove()
    {
        if (_gameState.NextMoveBy == EGamePiece.O)
        {
            _gameState.GameRoundNumber++;
            _gameState.GameRoundsLeft--;
        }
        _gameState.NextMoveBy = _gameState.NextMoveBy == EGamePiece.X ? EGamePiece.O : EGamePiece.X;
    }
    
    public int GameRoundNumber
    {
        get => GetRoundNumber();
        private set => _gameState.GameRoundNumber = value;
    }

    public int GetRoundsLeft()
    {
        return _gameState.GameRoundsLeft;
    }

    private int GetRoundNumber()
    {
        return _gameState.GameRoundNumber;
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
        // TODO: stop strike check when so little rows or columns left that can't make strike anymore anyway
        var countRowStrike = 0;
        var countColStrike = 0;

        for (int x = 0; x < DimX; x++)
        {
            countRowStrike = 0;
            for (int y = 0; y < DimY; y++)
            {
                countRowStrike = CountRowOrColumnStrike(player, countRowStrike, x, y);
                // if (y < DimX && x < DimY)
                // {
                //     countColStrike = CountRowOrColumnStrike(player, countColStrike, y, x);
                // }
                if (countRowStrike == _gameConfiguration.WinCondition ||
                    // countColStrike == _gameConfiguration.WinCondition ||
                    CheckDiagonalStreaks(player, x, y, (i) => i + 1) ||
                    CheckDiagonalStreaks(player, x, y, (i) => i - 1)) 
                {
                    // Console.WriteLine($"Row: {countRowStrike}");
                    // Console.WriteLine($"Col: {countColStrike}");
                    // Console.WriteLine($"Dia 1: {CheckDiagonalStreaks(player, x, y, (i) => i + 1)}");
                    // Console.WriteLine($"Dia 2: {CheckDiagonalStreaks(player, x, y, (i) => i - 1)}");
                    
                    return true;
                }
            }
        }
        
        for (int y = 0; y < DimY; y++)
        {
            countColStrike = 0;
            for (int x = 0; x < DimX; x++)
            {
                countColStrike = CountRowOrColumnStrike(player, countColStrike, x, y);
                // if (x < DimX && y < DimY)
                // {
                //     countColStrike = CountRowOrColumnStrike(player, countColStrike, x, y);
                // }
                if (
                    // countRowStrike == _gameConfiguration.WinCondition ||
                    countColStrike == _gameConfiguration.WinCondition
                    // CheckDiagonalStreaks(player, y, x, (i) => i + 1) ||
                    // CheckDiagonalStreaks(player, y, x, (i) => i - 1)
                    ) 
                {
                    // Console.WriteLine($"Row: {countRowStrike}");
                    // Console.WriteLine($"Col: {countColStrike}");
                    // Console.WriteLine($"Dia 1: {CheckDiagonalStreaks(player, x, y, (i) => i + 1)}");
                    // Console.WriteLine($"Dia 2: {CheckDiagonalStreaks(player, x, y, (i) => i - 1)}");
                    
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

    public int GameBoardEmptySpacesCount()
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

    public bool CanMoveGrid()
    {
        return GameRoundNumber > _gameConfiguration.MoveGridAfterNMoves &&
               (
                   _gameConfiguration.BoardSizeWidth > _gameConfiguration.GridSizeWidth ||
                   _gameConfiguration.BoardSizeHeight > _gameConfiguration.GridSizeHeight
                )
               && !MovePieceModeOn;
    }
    
    public bool CanMovePiece()
    {
        return GameRoundNumber > _gameConfiguration.MovePieceAfterNMoves;
    }
    
    public bool RemovePieceModeOn
    {
        get => _gameState.RemovePieceModeOn;
        set => _gameState.RemovePieceModeOn = value;
    }
    
    public void ActivateRemovePieceMode()
    {
        _gameState.RemovePieceModeOn = true;
    }
    
    public void DeActivateRemovePieceMode()
    {
        _gameState.RemovePieceModeOn = false;
    }
    
    public bool MovePieceModeOn
    {
        get => _gameState.MovePieceModeOn;
        set => _gameState.MovePieceModeOn = value;
    }
    
    public void ActivateMovePieceMode()
    {
        _gameState.MovePieceModeOn = true;
    }
    
    public void DeActivateMovePieceMode()
    {
        _gameState.MovePieceModeOn = false;
    }

    private void SaveCurrentGridState()
    {
        _gameState.MoveGridStartState = GetGrid();
        _gameState.MoveGridStartStateLocation[0] = _gameState.GridStartPosX;
        _gameState.MoveGridStartStateLocation[1] = _gameState.GridStartPosY;
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

    public void MakeAiMove()
    {
        var player = _gameState.NextMoveBy;
        var opponent = _gameState.NextMoveBy == EGamePiece.X ? EGamePiece.O : EGamePiece.X;
        var ai = new Ai(this, player, opponent);
        ai.FindBestMove();
        // Console.WriteLine($"AI chose <{bestMove[0]}, {bestMove[1]}>");
        // MakeAMove(bestMove[0], bestMove[1]);
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
