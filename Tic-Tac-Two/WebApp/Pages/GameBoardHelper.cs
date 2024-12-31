using GameBrain;

namespace WebApp.Pages;

public static class GameBoardHelper
{
    private const string ButtonBasic = "btn";
    private const string ButtonOutline = "btn-outline";
    private const string AddOutline = "-outline";
    private const string AddWarning = "-warning";
    private const string AddInfo = "-info";
    private const string AddLight = "-light";

    public static string StyleRemovablePiece(TicTacTwoBrain gameInstance, int x, int y)
    {
        var style = ButtonBasic;
        if (gameInstance.RemovePieceModeOn && gameInstance.GameBoard[x][y] == gameInstance.NextMoveBy)
        {
            style += AddOutline;
        }

        return style + StyleSpot(gameInstance, x, y);
    }

    public static string StyleFreeSpot(TicTacTwoBrain gameInstance, int x, int y)
    {
        return ButtonOutline + StyleSpot(gameInstance, x, y);
    }

    private static string StyleSpot(TicTacTwoBrain gameInstance, int x, int y)
    {
        var style = "";
        if (gameInstance.GameGrid[x][y])
        {
            style += AddWarning;
        }
        else if (gameInstance.MoveGridModeOn && gameInstance.GridMovingArea[x][y])
        {
            style += AddInfo;
        }
        else
        {
            style += AddLight;
        }
        return style;
    }
}
