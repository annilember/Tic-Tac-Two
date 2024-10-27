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
        var gameStateJsonStr = File.ReadAllText(FileHelper.BasePath + name + FileHelper.GameExtension);
        var gameState = JsonSerializer.Deserialize<GameState>(gameStateJsonStr);
        return gameState;
    }
    
    public void SaveGame(string jsonStateString, string gameConfigName)
    {
        var fileName = FileHelper.BasePath + 
                       gameConfigName + " " + 
                       DateTime.Now.ToString("O") + 
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