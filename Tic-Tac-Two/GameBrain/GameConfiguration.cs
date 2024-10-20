namespace GameBrain;

public record struct GameConfiguration()
{
    public string Name { get; set; } = default!;
    
    public int BoardSizeWidth { get; set; } = 5;
    public int BoardSizeHeight { get; set; } = 5;
    
    public int GridSizeWidth { get; set; } = 3;
    public int GridSizeHeight { get; set; } = 3;

    public int GridStartPosX { get; set; } = 1;
    public int GridStartPosY { get; set; } = 1;
    
    public int NumberOfPieces { get; set; } = 4;
    
    public int WinCondition { get; set; } = 3;
    
    public int MoveGridAfterNMoves { get; set; } = 2;
    
    public int MovePieceAfterNMoves { get; set; } = 2;
    
    
    public override string ToString() =>
        $"Board: {BoardSizeWidth}x{BoardSizeHeight}, " +
        $"Grid: {GridSizeWidth}x{GridSizeHeight}, " +
        $"to win: {WinCondition}, " +
        $"can move grid after {MoveGridAfterNMoves} moves." +
        $"can move pieces after {MovePieceAfterNMoves} moves.";
}