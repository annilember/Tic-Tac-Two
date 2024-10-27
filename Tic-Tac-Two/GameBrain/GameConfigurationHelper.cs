using System.Reflection;
using System.Security.Cryptography;

namespace GameBrain;

public static class GameConfigurationHelper
{
    public static PropertyInfo[] GetConfigPropertyInfo(GameConfiguration config)
    {
        return config.GetType().GetProperties();
    }

    public static Dictionary<string, int[]> GetConfigPropertyBoundsDictionary(GameConfiguration config)
    {
        var maxNumberOfPieces = (int)Math.Ceiling((decimal)(config.BoardSizeWidth * config.BoardSizeHeight) / 2);
        
        return new Dictionary<string, int[]>()
        {
            { "BoardSizeWidth", [3, 20] },
            { "BoardSizeHeight", [3, 20] },
            { "GridSizeWidth", [3, config.BoardSizeWidth] },
            { "GridSizeHeight", [3, config.BoardSizeHeight] },
            { "GridStartPosX", [0, config.BoardSizeWidth - config.GridSizeWidth] },
            { "GridStartPosY", [0, config.BoardSizeHeight - config.GridSizeHeight] },
            { "NumberOfPieces", [3, maxNumberOfPieces] },
            { "WinCondition", [3, Math.Min(config.GridSizeWidth, config.GridSizeHeight)] },
            { "MaxGameRounds", [config.NumberOfPieces, config.NumberOfPieces * 2] },
            { "MoveGridAfterNMoves", [0, config.MaxGameRounds] },
            { "MovePieceAfterNMoves", [0, config.MaxGameRounds] }
        };
    }
}