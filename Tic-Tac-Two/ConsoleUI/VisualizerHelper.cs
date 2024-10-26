namespace ConsoleUI;

public static class VisualizerHelper
{
    public const string BoardLineHorisontal = "\u2550\u2550\u2550";
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
    
    public const ConsoleColor XAxisColor = ConsoleColor.Green;
    
    public const ConsoleColor YAxisColor = ConsoleColor.Blue;
    
    public const ConsoleColor GridColor = ConsoleColor.DarkYellow;
    
    public const ConsoleColor GridAllowedMoveAreaColor = ConsoleColor.DarkGreen;
    
    public const ConsoleColor ErrorMessageColor = ConsoleColor.Red;
    
    public const string GameOverMessage = "  ____    _    __  __ _____    _____     _______ ____  \n / ___|  / \\  |  \\/  | ____|  / _ \\ \\   / / ____|  _ \\ \n| |  _  / _ \\ | |\\/| |  _|   | | | \\ \\ / /|  _| | |_) |\n| |_| |/ ___ \\| |  | | |___  | |_| |\\ V / | |___|  _ < \n \\____/_/   \\_\\_|  |_|_____|  \\___/  \\_/  |_____|_| \\_\\";
}