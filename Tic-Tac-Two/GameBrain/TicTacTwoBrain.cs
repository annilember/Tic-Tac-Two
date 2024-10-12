namespace GameBrain;

public class TicTacTwoBrain
{
    private EGamePiece[,] _gameBoard;
    
    private Boolean[,] _gameGrid;
    private EGamePiece _nextMoveBy { get; set; } = EGamePiece.X;

    private GameConfiguration _gameConfiguration;

    public TicTacTwoBrain(GameConfiguration gameConfiguration)
    {
        _gameConfiguration = gameConfiguration;
        _gameBoard = new EGamePiece[_gameConfiguration.BoardSizeWidth, _gameConfiguration.BoardSizeHeight];
        _gameGrid = new Boolean[_gameConfiguration.BoardSizeWidth, _gameConfiguration.BoardSizeHeight];
    }

    public EGamePiece[,] GameBoard
    {
        get => GetBoard();
        private set => _gameBoard = value;
    }
    
    public int DimX => _gameBoard.GetLength(0);
    public int DimY => _gameBoard.GetLength(1);

    private EGamePiece[,] GetBoard()
    {
        var copyOfBoard = new EGamePiece[DimX, DimY];
        
        for (var x = 0; x < DimX; x++)
        {
            for (var y = 0; y < DimY; y++)
            {
                copyOfBoard[x, y] = _gameBoard[x, y];
            }
        }
        
        return copyOfBoard;
    }
    
    public Boolean[,] GameGrid
    {
        get => GetGrid();
        private set => _gameGrid = value;
    }

    private int DimXGrid => _gameConfiguration.GridSizeWidth;
    private int DimYGrid => _gameConfiguration.GridSizeHeight;

    private int GridStartPosX => _gameConfiguration.GridStartPosX;
    private int GridStartPosY => _gameConfiguration.GridStartPosY;
    
    private Boolean[,] GetGrid()
    {
        var grid = new Boolean[DimX, DimY];
        
        for (var x = 0; x < DimX; x++)
        {
            for (var y = 0; y < DimY; y++)
            {
                var endPosGridX = GridStartPosX + DimXGrid;
                var endPosGridY = GridStartPosY + DimYGrid;
                
                if (x >= GridStartPosX && 
                    x < endPosGridX && 
                    y >= GridStartPosY && 
                    y < endPosGridY)
                {
                    grid[x, y] = true;
                }
                else grid[x, y] = false;
            }
        }
        return grid;
    }

    
    public bool MakeAMove(int x, int y)
    {
        if (_gameBoard[x, y] != EGamePiece.Empty)
        {
            return false;
        }

        _gameBoard[x, y] = _nextMoveBy;
        
        _nextMoveBy = _nextMoveBy == EGamePiece.X ? EGamePiece.O : EGamePiece.X;

        return true;
    }

    public void ResetGame()
    {
        _gameBoard = new EGamePiece[DimX, DimY];
        _nextMoveBy = EGamePiece.X;
    }
}