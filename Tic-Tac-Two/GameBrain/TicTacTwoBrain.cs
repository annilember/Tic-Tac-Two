namespace GameBrain;

public class TicTacTwoBrain
{
    private readonly GameState _gameState;

    public TicTacTwoBrain(GameConfiguration gameConfiguration)
    {
        var gameBoard = new EGamePiece[gameConfiguration.BoardSizeWidth][];
        var gameGrid = InitializeGrid(gameConfiguration);
        var gridStartPosX = gameConfiguration.GridStartPosX;
        var gridStartPosY = gameConfiguration.GridStartPosY;
        
        for (var x = 0; x < gameBoard.Length; x++)
        {
            gameBoard[x] = new EGamePiece[gameConfiguration.BoardSizeHeight];
        }
        
        _gameState = new GameState(gameConfiguration, 
            gameBoard, 
            gameGrid,
            gridStartPosX,
            gridStartPosY);
        
        GridMovingAreaTest = GetGridMovingArea();
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
        return GameState.GameConfiguration.Name;
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
        GridMovingAreaTest = GetGridMovingArea();
    }
    
    public void DeActivateMoveGridMode()
    {
        _gameState.MoveGridModeOn = false;
        CountAsMove();
    }

    public bool[][] GridMovingAreaTest { get; set; }
    
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
        var endPosX = startPosX + GameState.GameConfiguration.GridSizeWidth;
        var endPosY = startPosY + GameState.GameConfiguration.GridSizeHeight;
        
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


    public bool MakeAMove(int x, int y)
    {
        if (_gameState.GameBoard[x][y] != EGamePiece.Empty)
        {
            return false;
        }
        _gameState.GameBoard[x][y] = _gameState.NextMoveBy;
        
        CountAsMove();

        return true;
    }

    private void CountAsMove()
    {
        if (_gameState.NextMoveBy == EGamePiece.O)
        {
            _gameState.GameRoundNumber++;
        }
        _gameState.NextMoveBy = _gameState.NextMoveBy == EGamePiece.X ? EGamePiece.O : EGamePiece.X;
    }
    
    public int GameRoundNumber
    {
        get => GetRoundNumber();
        private set => _gameState.GameRoundNumber = value;
    }

    private int GetRoundNumber()
    {
        return _gameState.GameRoundNumber;
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
        var countRowStrike = 0;
        var countColStrike = 0;

        for (int x = 0; x < DimX; x++)
        {
            for (int y = 0; y < DimY; y++)
            {
                countRowStrike = CountRowOrColumnStrike(player, countRowStrike, x, y);
                countColStrike = CountRowOrColumnStrike(player, countColStrike, y, x);
                if (countRowStrike == GameState.GameConfiguration.WinCondition ||
                    countColStrike == GameState.GameConfiguration.WinCondition ||
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
                if (countStrike == GameState.GameConfiguration.WinCondition) return true;
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
        return GameRoundNumber > GameState.GameConfiguration.MoveGridAfterNMoves;
    }

    public void MoveGrid(EMoveGridDirection direction)
    {
        var startPosX = _gameState.GridStartPosX;
        var startPosY = _gameState.GridStartPosY;
        var endPosX = startPosX + GameState.GameConfiguration.GridSizeWidth;
        var endPosY = startPosY + GameState.GameConfiguration.GridSizeHeight;
        
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
        endPosX = startPosX + GameState.GameConfiguration.GridSizeWidth;
        endPosY = startPosY + GameState.GameConfiguration.GridSizeHeight;
        
        _gameState.GameGrid = CreateGrid(
            DimX, DimY, 
            startPosX, startPosY, 
            endPosX, endPosY);
        
        _gameState.GridStartPosX = startPosX;
        _gameState.GridStartPosY = startPosY;
    }

    public void ResetGame()
    {
        // Can I use constructor for making this easier???
        var gameBoard = new EGamePiece[GameState.GameConfiguration.BoardSizeWidth][];
        var gameGrid = InitializeGrid(GameState.GameConfiguration);
        
        for (var i = 0; i < gameBoard.Length; i++)
        {
            gameBoard[i] = new EGamePiece[GameState.GameConfiguration.BoardSizeHeight];
        }
        
        _gameState.GameBoard = gameBoard;
        _gameState.GameGrid = gameGrid;
        _gameState.GridStartPosX = GameState.GameConfiguration.GridStartPosX;
        _gameState.GridStartPosY = GameState.GameConfiguration.GridStartPosY;
        _gameState.NextMoveBy = EGamePiece.X;
        _gameState.GameRoundNumber = 1;
    }
}