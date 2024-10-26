namespace GameBrain;

public class GameState
{
    public EGamePiece[][] GameBoard { get; set; }
    
    public bool[][] GameGrid { get; set; }

    public bool MoveGridModeOn { get; set; } = false;
    
    public bool MovePieceModeOn { get; set; } = false;
    
    public int GridStartPosX { get; set; }
    public int GridStartPosY { get; set; }
    
    public EGamePiece NextMoveBy { get; set; } = EGamePiece.X;
    
    public int NumberOfPiecesLeftX { get; set; }
    public int NumberOfPiecesLeftO { get; set; }
    
    public int GameRoundNumber { get; set; } = 1;

    public GameConfiguration GameConfiguration { get; set; }

    public GameState(
        GameConfiguration gameConfiguration, 
        EGamePiece[][] gameBoard, 
        bool[][] gameGrid, 
        int gridStartPosX, 
        int gridStartPosY,
        int numberOfPiecesLeftX,
        int numberOfPiecesLeftO
        )
    {
        GameConfiguration = gameConfiguration;
        GameBoard = gameBoard;
        GameGrid = gameGrid;
        GridStartPosX = gridStartPosX;
        GridStartPosY = gridStartPosY;
        NumberOfPiecesLeftX = numberOfPiecesLeftX;
        NumberOfPiecesLeftO = numberOfPiecesLeftO;
    }

    public override string ToString()
    {
        return System.Text.Json.JsonSerializer.Serialize(this);
    }
}