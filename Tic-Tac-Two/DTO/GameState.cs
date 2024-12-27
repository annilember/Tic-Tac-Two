namespace Domain;

public class GameState()
{
    public GameState(
        GameConfiguration gameConfiguration,
        EGameMode gameMode,
        string playerXName,
        string playerOName
    ) : this()
    {
        GameMode = gameMode;
        PlayerXType = Domain.GameMode.GetPlayerType(gameMode, EGamePiece.X);
        PlayerOType = Domain.GameMode.GetPlayerType(gameMode, EGamePiece.O);
        PlayerXName = playerXName;
        PlayerOName = playerOName;
        GameBoard = new EGamePiece[gameConfiguration.BoardSizeWidth][];
        GameGrid = InitializeGrid(gameConfiguration);
        GridStartPosX = gameConfiguration.GridStartPosX;
        GridStartPosY = gameConfiguration.GridStartPosY;
        NumberOfPiecesLeftX = gameConfiguration.NumberOfPieces;
        NumberOfPiecesLeftO = gameConfiguration.NumberOfPieces;
        GameRoundsLeft = gameConfiguration.MaxGameRounds;
        
        for (var x = 0; x < GameBoard.Length; x++)
        {
            GameBoard[x] = new EGamePiece[gameConfiguration.BoardSizeHeight];
        }
    }
    
    public EGameMode GameMode { get; init; }

    public EPlayerType PlayerXType { get; init; }
    public EPlayerType PlayerOType { get; init; }
    
    public string PlayerXName { get; init; } = default!;
    public string PlayerOName { get; init; } = default!;

    public EGamePiece[][] GameBoard { get; set; } = default!;

    public bool[][] GameGrid { get; set; } = default!;

    public bool MoveGridModeOn { get; set; }
    
    public bool[][] MoveGridStartState { get; set; } = default!;
    
    public int[] MoveGridStartStateLocation { get; set; } = new int[2];
    
    public bool[][] GridMovingArea { get; set; } = default!;
    
    public bool RemovePieceModeOn { get; set; }
    public bool MovePieceModeOn { get; set; }
    
    public int[] RemovedPieceLocation { get; set; } = new int[2];
    
    public int GridStartPosX { get; set; }
    public int GridStartPosY { get; set; }

    public EGamePiece NextMoveBy { get; set; } = EGamePiece.X;
    
    public int NumberOfPiecesLeftX { get; set; }
    public int NumberOfPiecesLeftO { get; set; }

    public int GameRoundNumber { get; set; } = 1;
    
    public int GameRoundsLeft { get; set; }

    public override string ToString()
    {
        return System.Text.Json.JsonSerializer.Serialize(this);
    }
    
    public bool[][] InitializeGrid(GameConfiguration gameConfiguration)
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

    public bool[][] CreateGrid(int boardDimX, int boardDimY, int startPosX, int startPosY, int endPosX, int endPosY)
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
}