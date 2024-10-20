namespace GameBrain;

public class TicTacTwoBrain
{

    private readonly GameState _gameState;

    public TicTacTwoBrain(GameConfiguration gameConfiguration)
    {
        var gameBoard = new EGamePiece[gameConfiguration.BoardSizeWidth][];
        var gameGrid = InitializeGrid(gameConfiguration);
        var gameGridMovingArea = InitializeGridMovingArea(gameConfiguration);
        
        for (var x = 0; x < gameBoard.Length; x++)
        {
            gameBoard[x] = new EGamePiece[gameConfiguration.BoardSizeHeight];
        }
        
        _gameState = new GameState(gameConfiguration, 
            gameBoard, 
            gameGrid,
            gameGridMovingArea);
    }

    private bool[][] InitializeGrid(GameConfiguration gameConfiguration)
    {
        return CreateGrid(gameConfiguration.BoardSizeWidth,
            gameConfiguration.BoardSizeHeight,
            gameConfiguration.GridSizeWidth,
            gameConfiguration.GridSizeHeight,
            gameConfiguration.GridStartPosX,
            gameConfiguration.GridStartPosY);
    }
    
    private bool[][] InitializeGridMovingArea(GameConfiguration gameConfiguration)
    {
        int gridDimX = gameConfiguration.GridSizeWidth + 2;
        int gridDimY = gameConfiguration.GridSizeHeight + 2;
        int startPosX = gameConfiguration.GridStartPosX - 1;
        int startPosY = gameConfiguration.GridStartPosY - 1;
        
        return CreateGrid(
            gameConfiguration.BoardSizeWidth,
            gameConfiguration.BoardSizeHeight, 
            gridDimX, 
            gridDimY, 
            startPosX, 
            startPosY);
    }
    
    private bool[][] CreateGrid(int boardDimX, int boardDimY, int gridDimX, int gridDimY, int startPosX, int startPosY)
    {
        var gameGrid = new bool[boardDimX][];
        var gridEndPosX = startPosX + gridDimX;
        var gridEndPosY = startPosY + gridDimY;
        
        for (var x = 0; x < gameGrid.Length; x++)
        {
            gameGrid[x] = new bool[boardDimY];

            for (int y = 0; y < gameGrid[x].Length; y++)
            {
                if (x >= startPosX && 
                    x < gridEndPosX && 
                    y >= startPosY && 
                    y < gridEndPosY)
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
        return _gameState.GameConfiguration.Name;
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
    
    public bool[][] GameGridMovingArea
    {
        get => GetGridMovingArea();
        private set => _gameState.GameGridMovingArea = value;
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
    
    private bool[][] GetGridMovingArea()
    {
        int gridDimX = _gameState.GameConfiguration.GridSizeWidth + 2;
        int gridDimY = _gameState.GameConfiguration.GridSizeHeight + 2;
        int startPosX = _gameState.GameConfiguration.GridStartPosX - 1;
        int startPosY = _gameState.GameConfiguration.GridStartPosY - 1;
        return CreateGrid(DimX, DimY, gridDimX, gridDimY, startPosX, startPosY);
    }

    
    public bool MakeAMove(int x, int y)
    {
        if (_gameState.GameBoard[x][y] != EGamePiece.Empty)
        {
            return false;
        }

        _gameState.GameBoard[x][y] = _gameState.NextMoveBy;

        if (_gameState.NextMoveBy == EGamePiece.O)
        {
            _gameState.RoundNumber++;
        }
    
        _gameState.NextMoveBy = _gameState.NextMoveBy == EGamePiece.X ? EGamePiece.O : EGamePiece.X;

        return true;
    }
    
    public int RoundNumber
    {
        get => GetRoundNumber();
        private set => _gameState.RoundNumber = value;
    }

    private int GetRoundNumber()
    {
        return _gameState.RoundNumber;
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
        int countRowStrike = 0;
        int countColStrike = 0;

        for (int x = 0; x < DimX; x++)
        {
            for (int y = 0; y < DimY; y++)
            {
                countRowStrike = CountRowOrColumnStrike(player, countRowStrike, x, y);
                countColStrike = CountRowOrColumnStrike(player, countColStrike, y, x);
                if (countRowStrike == _gameState.GameConfiguration.WinCondition ||
                    countColStrike == _gameState.GameConfiguration.WinCondition ||
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
        int countStrike = 0;
        while (_gameState.GameGrid[x][y])
        {
            if (_gameState.GameBoard[x][y] == player) countStrike++;
            if (countStrike == _gameState.GameConfiguration.WinCondition) return true;
            x++;
            y = action(y);
        }

        return false;
    }

    public bool CanMoveGrid()
    {
        return RoundNumber > _gameState.GameConfiguration.MoveGridAfterNMoves;
    }

    private bool MoveGrid(int gridStartPosX, int gridStartPosY)
    {
        // TODO: add conditions when false.
        // Should I make a combo method that is used for initializing AND moving?
        
        var endPosGridX = gridStartPosX + _gameState.GameConfiguration.GridSizeWidth;
        var endPosGridY = gridStartPosY + _gameState.GameConfiguration.GridSizeHeight;
        
        for (var x = 0; x < DimX; x++)
        {
            for (var y = 0; y < DimY; y++)
            {
                if (x >= gridStartPosX && 
                    x < endPosGridX && 
                    y >= gridStartPosY && 
                    y < endPosGridY)
                {
                    _gameState.GameGrid[x][y] = true;
                }
                else
                {
                    _gameState.GameGrid[x][y] = false;
                }
            }
        }

        return true;
    }

    public void ResetGame()
    {
        // Can I use constructor for making this easier???
        var gameBoard = new EGamePiece[_gameState.GameConfiguration.BoardSizeWidth][];
        var gameGrid = InitializeGrid(_gameState.GameConfiguration);
        
        for (var i = 0; i < gameBoard.Length; i++)
        {
            gameBoard[i] = new EGamePiece[_gameState.GameConfiguration.BoardSizeHeight];
        }
        
        _gameState.GameBoard = gameBoard;
        _gameState.GameGrid = gameGrid;
        _gameState.NextMoveBy = EGamePiece.X;
    }
}