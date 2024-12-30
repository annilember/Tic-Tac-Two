using System.ComponentModel.DataAnnotations;

namespace Domain;

public record GameConfiguration
{
    public int Id { get; set; }
    
    [MaxLength(128)]
    public string Name { get; set; } = default!;
    
    public int BoardSizeWidth { get; set; } = 5;
    public int BoardSizeHeight { get; set; } = 5;
    
    public int GridSizeWidth { get; set; } = 3;
    public int GridSizeHeight { get; set; } = 3;

    public int GridStartPosX { get; set; } = 1;
    public int GridStartPosY { get; set; } = 1;
    
    public int NumberOfPieces { get; set; } = 4;
    
    public int WinCondition { get; set; } = 3;
    
    public int MaxGameRounds { get; set; } = 6;
    
    public int MoveGridAfterNMoves { get; set; } = 2;
    
    public int MovePieceAfterNMoves { get; set; } = 2;

    public ICollection<SavedGame>? SavedGames { get; set; } = default;
    
    
    public override string ToString() =>
        $"Name: {Name}, " +
        $"board: {BoardSizeWidth}x{BoardSizeHeight}, " +
        $"grid: {GridSizeWidth}x{GridSizeHeight}, " +
        $"grid starts at position: <{GridStartPosX};{GridStartPosY}>, " +
        $"number of pieces per player: {NumberOfPieces}, " +
        $"to win: {WinCondition}, " +
        $"maximum number of game rounds: {MaxGameRounds}, " +
        $"can move grid after {MoveGridAfterNMoves} moves, " +
        $"can move pieces after {MovePieceAfterNMoves} moves.";
}