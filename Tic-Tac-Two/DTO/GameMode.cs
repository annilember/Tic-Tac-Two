namespace Domain;

public record GameMode()
{
    public static List<string> GetGameModeNames()
    {
        return Enum.GetNames<EGameMode>().Select(GetModeName).ToList();
    }
    
    public static List<EGameMode> GetGameModes()
    {
        return Enum.GetNames<EGameMode>().Select(GetMode).ToList();
    }

    public static string GetPlayerTypeName(EGameMode mode, EGamePiece piece)
    {
        var playerType = GetPlayerType(mode, piece);
        switch (playerType)
        {
            case EPlayerType.Human:
            {
                return "Human";
            }
            case EPlayerType.Ai:
            {
                return "AI";
            }
        }

        throw new InvalidOperationException("Player type not supported!");
    }
    
    public static EPlayerType GetPlayerType(EGameMode mode, EGamePiece piece)
    {
        var playerX = EPlayerType.Human;
        var playerO = EPlayerType.Human;
        
        switch (mode)
        {
            case EGameMode.HumanVsHuman:
            {
                playerX = EPlayerType.Human;
                playerO = EPlayerType.Human;
                break;
            }
            case EGameMode.HumanVsAi:
            {
                playerX = EPlayerType.Human;
                playerO = EPlayerType.Ai;
                break;
            }
            case EGameMode.AiVsHuman:
            {
                playerX = EPlayerType.Ai;
                playerO = EPlayerType.Human;
                break;
            }
            case EGameMode.AiVsAi:
            {
                playerX = EPlayerType.Ai;
                playerO = EPlayerType.Ai;
                break;
            }
        }

        return piece switch
        {
            EGamePiece.X => playerX,
            EGamePiece.O => playerO,
            _ => throw new InvalidOperationException("Could not determine player type!")
        };
    }

    public static EGameMode GetMode(string modeName)
    {
        if (string.IsNullOrEmpty(modeName))
            throw new ArgumentNullException(nameof(modeName));
        
        modeName = modeName.Replace(" ", "").ToLower();
        return modeName switch
        {
            "humanvshuman" => EGameMode.HumanVsHuman,
            "humanvsai" => EGameMode.HumanVsAi,
            "aivshuman" => EGameMode.AiVsHuman,
            "aivsai" => EGameMode.AiVsAi,
            _ => throw new InvalidOperationException("Game Mode does not exist: " + modeName)
        };
    }

    public static string GetModeName(string modeName)
    {
        return modeName switch
        {
            "HumanVsHuman" => "Human vs Human",
            "HumanVsAi" => "Human vs AI",
            "AiVsHuman" => "AI vs Human",
            "AiVsAi" => "AI vs AI",
            _ => throw new InvalidOperationException("Game Mode does not exist: " + modeName)
        };
    }
}
