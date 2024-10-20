namespace GameBrain;

public class GameState
{
    public EGamePiece[][] GameBoard { get; set; }
    
    public bool[][] GameGrid { get; set; }

    public bool MoveGridModeOn { get; set; } = false;
    
    public int GridStartPosX { get; set; }
    public int GridStartPosY { get; set; }
    
    public EGamePiece NextMoveBy { get; set; } = EGamePiece.X;
    
    public int RoundNumber { get; set; } = 1;

    public static GameConfiguration GameConfiguration { get; set; }

    public GameState(
        GameConfiguration gameConfiguration, 
        EGamePiece[][] gameBoard, 
        bool[][] gameGrid, 
        int gridStartPosX, 
        int gridStartPosY
        )
    {
        GameConfiguration = gameConfiguration;
        GameBoard = gameBoard;
        GameGrid = gameGrid;
        GridStartPosX = gridStartPosX;
        GridStartPosY = gridStartPosY;
    }

    public override string ToString()
    {
        return System.Text.Json.JsonSerializer.Serialize(this);
    }
}