namespace GameBrain;

public class GameState
{
    public EGamePiece[][] GameBoard { get; set; }
    
    public bool[][] GameGrid { get; set; }
    
    public EGamePiece NextMoveBy { get; set; } = EGamePiece.X;

    public GameConfiguration GameConfiguration { get; set; }

    public GameState(GameConfiguration gameConfiguration, EGamePiece[][] gameBoard, bool[][] gameGrid)
    {
        GameConfiguration = gameConfiguration;
        GameBoard = gameBoard;
        GameGrid = gameGrid;
    }

    public override string ToString()
    {
        return System.Text.Json.JsonSerializer.Serialize(this);
    }
}