namespace DTO;

public record GameMode
{
    private const string HumanPlayerTypeName = "Human";
    private const string AiPlayerTypeName = "AI";
    private const string HumanVsHumanGameModeName = "Human vs Human";
    private const string HumanVsAiGameModeName = "Human vs AI";
    private const string AiVsHumanGameModeName = "AI vs Human";
    private const string AiVsAiGameModeName = "AI vs AI";
    private const string HumanVsHumanConstrictedName = "humanvshuman";
    private const string HumanVsAiConstrictedName = "humanvsai";
    private const string AiVsHumanConstrictedName = "aivshuman";
    private const string AiVsAiConstrictedName = "aivsai";
    
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
                return HumanPlayerTypeName;
            }
            case EPlayerType.Ai:
            {
                return AiPlayerTypeName;
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
            HumanVsHumanConstrictedName => EGameMode.HumanVsHuman,
            HumanVsAiConstrictedName => EGameMode.HumanVsAi,
            AiVsHumanConstrictedName => EGameMode.AiVsHuman,
            AiVsAiConstrictedName => EGameMode.AiVsAi,
            _ => throw new InvalidOperationException("Game Mode does not exist: " + modeName)
        };
    }

    public static string GetModeName(string modeName)
    {
        modeName = modeName.Replace(" ", "").ToLower();
        return modeName switch
        {
            HumanVsHumanConstrictedName => HumanVsHumanGameModeName,
            HumanVsAiConstrictedName => HumanVsAiGameModeName,
            AiVsHumanConstrictedName => AiVsHumanGameModeName,
            AiVsAiConstrictedName => AiVsAiGameModeName,
            _ => throw new InvalidOperationException("Game Mode does not exist: " + modeName)
        };
    }
}
