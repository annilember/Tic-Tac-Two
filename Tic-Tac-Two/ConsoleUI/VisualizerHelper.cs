namespace ConsoleUI;

public static class VisualizerHelper
{
    public const string BoardLineHorizontal = "\u2550\u2550\u2550";
    
    public const string BoardLineVertical = "\u2551";
    
    public const string BoardCrossing = "\u256c";
    
    public const string BoardCrossingNorth = "\u2569";
    
    public const string BoardCrossingSouth = "\u2566";
    
    public const string BoardCrossingEast = "\u2563";
    
    public const string BoardCrossingWest = "\u2560";
    
    public const string BoardCornerNorthEast = "\u2554";
    
    public const string BoardCornerNorthWest = "\u2557";
    
    public const string BoardCornerSouthEast = "\u255a";
    
    public const string BoardCornerSouthWest = "\u255d";
    
    public const string ArrowUp = "\u2191";
    
    public const string ArrowDown = "\u2193";
    
    public const string ArrowLeft = "\u2190";
    
    public const string ArrowRight = "\u2192";
    
    public const ConsoleColor XAxisColor = ConsoleColor.Green;
    
    public const ConsoleColor YAxisColor = ConsoleColor.Blue;
    
    public const ConsoleColor GridColor = ConsoleColor.DarkYellow;
    
    public const ConsoleColor GridAllowedMoveAreaColor = ConsoleColor.DarkGreen;
    
    public const ConsoleColor MessageColor = ConsoleColor.Yellow;
    
    public const ConsoleColor ErrorMessageColor = ConsoleColor.Red;
    
    public const ConsoleColor AiColor = ConsoleColor.Cyan;
    
    public const ConsoleColor ActionColor = ConsoleColor.Magenta;
    
    public const string GameOverGraphics = "  ____    _    __  __ _____    _____     _______ ____  \n / ___|  / \\  |  \\/  | ____|  / _ \\ \\   / / ____|  _ \\ \n| |  _  / _ \\ | |\\/| |  _|   | | | \\ \\ / /|  _| | |_) |\n| |_| |/ ___ \\| |  | | |___  | |_| |\\ V / | |___|  _ < \n \\____/_/   \\_\\_|  |_|_____|  \\___/  \\_/  |_____|_| \\_\\";
    
    public const string Divider = "======================";
    
    public const string DeleteGameMenuHeader = "TIC-TAC-TWO - choose game to delete";
    
    public const string RenameGameMenuHeader = "TIC-TAC-TWO - choose game to rename";
    
    public const string LoadGameMenuHeader = "TIC-TAC-TWO - choose game to load";
    
    public const string DeleteConfigMenuHeader = "TIC-TAC-TWO - choose game configuration to delete";
    
    public const string ChangeConfigMenuHeader = "TIC-TAC-TWO - choose game configuration to change";
    
    public const string ChooseConfigForNewGameMenuHeader = "TIC-TAC-TWO - choose configuration for new game";
    
    public const string ChooseGameModeMenuHeader = "TIC-TAC-TWO - choose game mode";

    public const string ChoosePropertyMenuHeader = "TIC-TAC-TWO - choose property to change";

    public const string RulesPageHeader = "TIC-TAC-TWO - classical rules";

    public const string GameNameToModeArrow = " --> ";
    
    public const string GameRules = "Tic-Tac-Two closely follows the rules of Tic-Tac-Toe. To begin the game the moveable grid is placed on the gameboard. It can be placed on any location on the board as long as the whole grid is on the gameboard. The game begins like Tic-Tac-Toe where both players alternate placing their first two pieces.\n\nAfter both players have placed 2 tokens, each player then has a choice of what they would like to do on their turn. They can do one of the following:\n\n1. Place one of the pieces that are still in their hand in one of the spots in the grid.\n2. Move one of their pieces that are in the grid to another spot in the grid.\n3. They may move the grid one spot in any direction (horizontally, vertically, or diagonally).\n\nWhen both players are out of pieces and there is still no winner, additional 2 rounds of game are available where players can make moves 2. or 3. on their turn.\n\nCheck out other configurations or create an entirely new configurations to bend the rules ;)";

    public const string PressAnyKeyToContinue = "Press any key to return to the main menu...";

    public const string MenuItemParentheses = ") ";

    public const string MenuInputArrow = ">";
}
