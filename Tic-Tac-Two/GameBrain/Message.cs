using DTO;

namespace GameBrain;

public static class Message
{
    public const string NoSavedGamesMessage = "No saved games available!";
    
    public const string GameOverDrawMessage = "It's a draw!";
    
    public const string GameOverNoMoreRoundsMessage = "No more rounds left!";

    public const string InvalidXCoordinateMessage = "Insert valid X coordinate!";

    public const string InvalidYCoordinateMessage = "Insert valid Y coordinate!";
    
    public const string InvalidCoordinatesMessage = "One number for X and one number for Y please!";

    public const string NotEnoughPiecesMessage = "Not enough pieces to make a move! You can move a piece, or move grid.";
    
    public const string SpaceOccupiedMessage = "Space occupied! Try again!";
    
    public const string RemovedPieceCoordinateClashMessage = "Can't insert piece back to where it was! Try again!";
    
    public const string CoordinatesOutOfBoundsMessage = "Invalid coordinates! Please stay inside the board!";
    
    public const string GridWasNotMovedMessage = "Grid was not moved! Use arrow keys to move the grid.";
    
    public const string ConfigNameAlreadyInUseMessage = "Configuration with this name already exists. Please try another name.";
    
    public const string GameNameAlreadyInUseMessage = "Game with this name already exists. Please try another name.";

    public const string InvalidInputMessage = "Input is invalid! Try again!";
    
    public const string ConfigDeletedMessage = "Configuration deleted!";
    
    public const string GameDeletedMessage = "Game deleted!";
    
    public const string GameRenamedMessage = "Game renamed!";
    
    public const string PropertySavedMessage = "Property saved!";
    
    public const string FinalRoundMessage = "Final round!";
    
    public const string UnknownPlayerName = "Alien";
    
    public const string NewConfigCreatedMessage = "New configuration created!";
    
    public const string ConfigUpdatedMessage = "Configuration updated!";
    
    public const string MenuChooseOptionMessage = "Please choose an option.";
    
    public const string MenuChooseValidOptionMessage = "Please choose a valid option.";

    public static string GetTheWinnerIsMessage(string name)
    {
        return $"The winner is {name}!";
    }
    
    public static string GetValueOutOfBoundsError(string propertyName, int min, int max)
    {
        return $"{propertyName} value has to range from {min} to {max}!";
    }

    public static string GetInvalidCoordinatesError(int x, int y)
    {
        return $"Coordinates <{x},{y}> do not contain your piece! Choose again!";
    }
    
    public static string GamePieceAsString(EGamePiece piece) =>
        piece switch
        {
            EGamePiece.O => "O",
            EGamePiece.X => "X",
            _ => " "
        };
}