using System.Text.Json;
using Domain;

namespace GameBrain;

public class TicTacTwoBrain
{
    private readonly GameState _gameState;
    private readonly GameConfiguration _gameConfiguration;

    public TicTacTwoBrain(GameConfiguration gameConfiguration, string playerXName, string playerOName)
    {
        var gameBoard = new EGamePiece[gameConfiguration.BoardSizeWidth][];
        var gameGrid = InitializeGrid(gameConfiguration);
        var gridStartPosX = gameConfiguration.GridStartPosX;
        var gridStartPosY = gameConfiguration.GridStartPosY;
        var numberOfPiecesLeftX = gameConfiguration.NumberOfPieces;
        var numberOfPiecesLeftO = gameConfiguration.NumberOfPieces;
        var gameRoundsLeft = gameConfiguration.MaxGameRounds;
        
        for (var x = 0; x < gameBoard.Length; x++)
        {
            gameBoard[x] = new EGamePiece[gameConfiguration.BoardSizeHeight];
        }
        
        _gameState = new GameState(
            gameBoard, 
            gameGrid,
            gridStartPosX,
            gridStartPosY,
            numberOfPiecesLeftX,
            numberOfPiecesLeftO,
            gameRoundsLeft,
            playerXName,
            playerOName);
        
        _gameConfiguration = gameConfiguration;
        
        GridMovingArea = GetGridMovingArea();
        CurrentGridState = GetGrid();
        _currentGridStateStartPos = new Dictionary<string, int>
        {
            { "Xpos", _gameState.GridStartPosX },
            { "Ypos", _gameState.GridStartPosY }
        };
    }
    
    public TicTacTwoBrain(SavedGame savedGame, GameConfiguration config)
    {
        _gameConfiguration = config;
        _gameState = JsonSerializer.Deserialize<GameState>(savedGame.State)!;
        Console.WriteLine(_gameConfiguration.BoardSizeWidth);
        GridMovingArea = GetGridMovingArea();
        CurrentGridState = GetGrid();
        _currentGridStateStartPos = new Dictionary<string, int>
        {
            { "Xpos", _gameState.GridStartPosX },
            { "Ypos", _gameState.GridStartPosY }
        };
    }

    private bool[][] InitializeGrid(GameConfiguration gameConfiguration)
    {
        var gridEndPosX = gameConfiguration.GridStartPosX + gameConfiguration.GridSizeWidth;
        var gridEndPosY = gameConfiguration.GridStartPosY + gameConfiguration.GridSizeHeight;
        
        return CreateGrid(gameConfiguration.BoardSizeWidth,
            gameConfiguration.BoardSizeHeight,
            gameConfiguration.GridStartPosX,
            gameConfiguration.GridStartPosY,
            gridEndPosX,
            gridEndPosY);
    }
    
    private bool[][] CreateGrid(int boardDimX, int boardDimY, int startPosX, int startPosY, int endPosX, int endPosY)
    {
        var gameGrid = new bool[boardDimX][];
        
        for (var x = 0; x < gameGrid.Length; x++)
        {
            gameGrid[x] = new bool[boardDimY];

            for (int y = 0; y < gameGrid[x].Length; y++)
            {
                if (x >= startPosX && 
                    x < endPosX && 
                    y >= startPosY && 
                    y < endPosY)
                {
                    gameGrid[x][y] = true;
                } 
            }
        }

        return gameGrid;
    }

    public string GetGameStateJson()
    {
        return _gameState.ToString();
    }

    public string GetGameConfigName()
    {
        return _gameConfiguration.Name;
    }
    
    public GameConfiguration GetGameConfig()
    {
        return _gameConfiguration;
    }

    public string GetNextMoveByPlayerName()
    {
        return _gameState.NextMoveBy switch
        {
            EGamePiece.X => _gameState.PlayerXName,
            EGamePiece.O => _gameState.PlayerOName,
            _ => ""
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

    public bool IsGameOverAnyway()
    {
        return _gameState.GameRoundsLeft == 0 && _gameState.NextMoveBy == EGamePiece.X;
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
        _gameState.MoveGridModeOn = true;
        GridMovingArea = GetGridMovingArea();
    }
    
    public void DeActivateMoveGridMode()
    {
        _gameState.MoveGridModeOn = false;
        CountAsMove();
    }

    public void RestoreGridState()
    {
        _gameState.MoveGridModeOn = false;
        _gameState.GameGrid = CurrentGridState;
        _gameState.GridStartPosX = _currentGridStateStartPos["Xpos"];
        _gameState.GridStartPosY = _currentGridStateStartPos["Ypos"];
    }

    public bool[][] GridMovingArea { get; set; }
    
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
    
    private bool[][] GetGridMovingArea()
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
        
        var gridMovingArea = CreateGrid(DimX, DimY, startPosX, startPosY, endPosX, endPosY);
        
        GridMovingLowerBoundX = startPosX;
        GridMovingLowerBoundY = startPosY;
        GridMovingUpperBoundX = endPosX;
        GridMovingUpperBoundY = endPosY;
        
        return gridMovingArea;
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
            x == _removedPieceLocation[0] && 
            y == _removedPieceLocation[1])
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

    public bool RemovedPieceCoordinateClash(int x, int y)
    {
        if (!MovePieceModeOn) return false;
        return x == _removedPieceLocation[0] && 
               y == _removedPieceLocation[1];
    }

    private readonly int[] _removedPieceLocation = new int[2];
    
    public bool RemovePiece(int x, int y)
    {
        if (_gameState.GameBoard[x][y] != _gameState.NextMoveBy) return false;
        _gameState.GameBoard[x][y] = EGamePiece.Empty;
        _removedPieceLocation[0] = x;
        _removedPieceLocation[1] = y;
        
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

    private EGamePiece CheckForWinner()
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
            for (int y = 0; y < DimY; y++)
            {
                countRowStrike = CountRowOrColumnStrike(player, countRowStrike, x, y);
                if (y < DimX && x < DimY)
                {
                    countColStrike = CountRowOrColumnStrike(player, countColStrike, y, x);
                }
                if (countRowStrike == _gameConfiguration.WinCondition ||
                    countColStrike == _gameConfiguration.WinCondition ||
                    CheckDiagonalStreaks(player, x, y, (i) => i + 1) ||
                    CheckDiagonalStreaks(player, x, y, (i) => i - 1)) 
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

    public bool CanMoveGrid()
    {
        return GameRoundNumber > _gameConfiguration.MoveGridAfterNMoves;
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
    
    private bool[][] CurrentGridState { get; set; }
    private readonly Dictionary<string, int> _currentGridStateStartPos;
    
    public void SaveCurrentGridState()
    {
        CurrentGridState = GetGrid();
        _currentGridStateStartPos["Xpos"] = _gameState.GridStartPosX;
        _currentGridStateStartPos["Ypos"] = _gameState.GridStartPosY;
    }

    public bool GridWasMoved()
    {
        for (int x = 0; x < DimX; x++)
        {
            for (int y = 0; y < DimY; y++)
            {
                if (CurrentGridState[x][y] != GameGrid[x][y])
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
        
        _gameState.GameGrid = CreateGrid(
            DimX, DimY, 
            startPosX, startPosY, 
            endPosX, endPosY);
        
        _gameState.GridStartPosX = startPosX;
        _gameState.GridStartPosY = startPosY;
    }

    public void ResetGame()
    {
        var gameBoard = new EGamePiece[_gameConfiguration.BoardSizeWidth][];
        var gameGrid = InitializeGrid(_gameConfiguration);
        
        for (var i = 0; i < gameBoard.Length; i++)
        {
            gameBoard[i] = new EGamePiece[_gameConfiguration.BoardSizeHeight];
        }
        
        _gameState.GameBoard = gameBoard;
        _gameState.GameGrid = gameGrid;
        _gameState.MoveGridModeOn = false;
        _gameState.RemovePieceModeOn = false;
        _gameState.GridStartPosX = _gameConfiguration.GridStartPosX;
        _gameState.GridStartPosY = _gameConfiguration.GridStartPosY;
        _gameState.NextMoveBy = EGamePiece.X;
        _gameState.NumberOfPiecesLeftX = _gameConfiguration.NumberOfPieces;
        _gameState.NumberOfPiecesLeftO = _gameConfiguration.NumberOfPieces;
        _gameState.GameRoundNumber = 1;
        _gameState.GameRoundsLeft = _gameConfiguration.MaxGameRounds;
    }
}