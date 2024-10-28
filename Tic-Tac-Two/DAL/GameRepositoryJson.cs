using System.Text.Json;
using GameBrain;

namespace DAL;

public class GameRepositoryJson : IGameRepository
{
    public List<string> GetGameNames()
    {
        CheckAndCreateInitialFolder();
        return Directory
            .GetFiles(FileHelper.BasePath, FileHelper.AsteriskSymbol + FileHelper.GameExtension)
            .Select(fullFileName =>
                Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(fullFileName)))
            .ToList();
    }

    public GameState? GetGameStateByName(string name)
    {
        if (!GameExists(name)) return null;
        
        var gameStateJsonStr = File.ReadAllText(FileHelper.BasePath + name + FileHelper.GameExtension);
        var gameState = JsonSerializer.Deserialize<GameState>(gameStateJsonStr);
        return gameState;
    }

    public string GetGameStateJsonByName(string name)
    {
        return GameExists(name) ? File.ReadAllText(FileHelper.BasePath + name + FileHelper.GameExtension) : "";
    }

    public bool GameExists(string name)
    {
        return File.Exists(FileHelper.BasePath + name + FileHelper.GameExtension);
    }
    
    public void SaveGame(string jsonStateString, string gameConfigName, bool addDateTime)
    {
        var dateTime = "";
        if (addDateTime)
        {
            dateTime = " " + DateTime.Now.ToString("O");
        }
        var fileName = FileHelper.BasePath + 
                       gameConfigName + 
                       dateTime + 
                       FileHelper.GameExtension;
        
        File.WriteAllText(fileName, jsonStateString);
    }

    public void DeleteGame(string name)
    {
        File.Delete(FileHelper.BasePath + name + FileHelper.GameExtension);
    }
    
    private void CheckAndCreateInitialFolder()
    {
        if (!Directory.Exists(FileHelper.BasePath))
        {
            Directory.CreateDirectory(FileHelper.BasePath);
        }
    }
}