namespace Domain;

public class GameState()
{
    public GameState(
        EGameMode gameMode,
        EGamePiece[][] gameBoard, 
        bool[][] gameGrid, 
        int gridStartPosX, 
        int gridStartPosY,
        int numberOfPiecesLeftX,
        int numberOfPiecesLeftO,
        int gameRoundsLeft,
        string playerXName,
        string playerOName
    ) : this()
    {
        GameMode = gameMode;
        PlayerXType = Domain.GameMode.GetPlayerType(gameMode, EGamePiece.X);
        PlayerOType = Domain.GameMode.GetPlayerType(gameMode, EGamePiece.O);
        PlayerXName = playerXName;
        PlayerOName = playerOName;
        GameBoard = gameBoard;
        GameGrid = gameGrid;
        GridStartPosX = gridStartPosX;
        GridStartPosY = gridStartPosY;
        NumberOfPiecesLeftX = numberOfPiecesLeftX;
        NumberOfPiecesLeftO = numberOfPiecesLeftO;
        GameRoundsLeft = gameRoundsLeft;
    }
    
    public EGameMode GameMode { get; init; }

    public EPlayerType PlayerXType { get; init; }
    public EPlayerType PlayerOType { get; init; }
    
    public string PlayerXName { get; init; } = default!;
    public string PlayerOName { get; init; } = default!;

    public EGamePiece[][] GameBoard { get; set; } = default!;

    public bool[][] GameGrid { get; set; } = default!;

    public bool MoveGridModeOn { get; set; } = false;
    
    public bool RemovePieceModeOn { get; set; } = false;
    public bool MovePieceModeOn { get; set; } = false;
    
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
}