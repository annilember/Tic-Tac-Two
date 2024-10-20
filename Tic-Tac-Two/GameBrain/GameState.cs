namespace GameBrain;

public class GameState
{
    public EGamePiece[][] GameBoard { get; set; }
    
    public bool[][] GameGrid { get; set; }
    
    public bool[][] GameGridMovingArea { get; set; }

    public bool MoveGridModeOn { get; set; } = false;
    
    public EGamePiece NextMoveBy { get; set; } = EGamePiece.X;
    
    public int RoundNumber { get; set; } = 1;

    public GameConfiguration GameConfiguration { get; set; }

    public GameState(GameConfiguration gameConfiguration, EGamePiece[][] gameBoard, bool[][] gameGrid, bool[][] gameGridMovingArea)
    {
        GameConfiguration = gameConfiguration;
        GameBoard = gameBoard;
        GameGrid = gameGrid;
        GameGridMovingArea = gameGridMovingArea;
    }

    public override string ToString()
    {
        return System.Text.Json.JsonSerializer.Serialize(this);
    }
}