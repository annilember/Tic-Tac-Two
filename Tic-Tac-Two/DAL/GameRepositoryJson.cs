using System.Text.Json;
using Domain;
using DTO;
using GameBrain;

namespace DAL;

public class GameRepositoryJson : IGameRepository
{
    public List<SavedGame> GetSavedGames()
    {
        //TODO: vaata, et selle SavedGame sees, mis siit võetakse, oleks config olemas! (nagu db JOIN puhul).
        var gameNames = GetGameNames();
        return gameNames.Select(GetSavedGameByName).ToList();
    }
    
    public List<string> GetGameNames()
    {
        CheckAndCreateInitialFolder();
        return Directory
            .GetFiles(FileHelper.BasePath, FileHelper.AsteriskSymbol + FileHelper.GameExtension)
            .Select(fullFileName =>
                Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(fullFileName)))
            .ToList();
    }

    public SavedGame GetSavedGameByName(string name)
    {
        //TODO: vaata, et selle SavedGame sees, mis siit võetakse, oleks config olemas! (nagu db JOIN puhul).
        return JsonSerializer.Deserialize<SavedGame>(GetSavedGameJsonByName(name))!;
    }

    public GameState GetSavedGameState(SavedGame savedGame)
    {
        return JsonSerializer.Deserialize<GameState>(savedGame.State)!;
    }

    public string GetSavedGameJsonByName(string name)
    {
        return GameExists(name) ? File.ReadAllText(FileHelper.BasePath + name + FileHelper.GameExtension) : "";
    }

    public bool GameExists(string name)
    {
        return File.Exists(FileHelper.BasePath + name + FileHelper.GameExtension);
    }
    
    public void SaveGame(TicTacTwoBrain gameInstance)
    {
        var savedGame = gameInstance.SavedGame;
        savedGame.State = gameInstance.GetGameStateJson();
        
        //TODO: added this because of DB, but haven't checked if works.
        CreateNewSavedGameFile(savedGame);
    }

    public void RenameGame(SavedGame savedGame, string newName)
    {
        var oldName = savedGame.Name;
        savedGame.Name = newName;
        CreateNewSavedGameFile(savedGame);
        DeleteGame(oldName);
    }

    public void DeleteGame(string name)
    {
        File.Delete(FileHelper.BasePath + name + FileHelper.GameExtension);
    }
    
    public SavedGame CreateGame(GameConfiguration config, EGameMode gameMode, string playerXName, string playerOName)
    {
        return CreateGame(config, gameMode, "", playerXName, playerOName, playerXName, playerOName);
    }

    public SavedGame CreateGame(
        GameConfiguration config, 
        EGameMode gameMode, 
        string gameName,
        string playerXName, 
        string playerOName,
        string playerXPassword,
        string playerOPassword)
    {
        var createdAtDateTime = DateTime.Now.ToString("O");
        if (gameName == "")
        {
            gameName = playerXName + " & " + playerOName + " " + createdAtDateTime;
        }
        
        var savedGame = new SavedGame
        {
            Name = gameName,
            ModeName = GameMode.GetModeName(gameMode.ToString()),
            PlayerXName = playerXName,
            PlayerOName = playerOName,
            PlayerXPassword = playerXPassword,
            PlayerOPassword = playerOPassword,
            CreatedAtDateTime = createdAtDateTime,
            State = new GameState(
                config,
                gameMode,
                playerXName,
                playerOName
            ).ToString(),
            Configuration = config
        };
        
        // TODO: Check that it works with files.
        CreateNewSavedGameFile(savedGame);
        
        return savedGame;
    }

    public GameConfiguration GetGameConfiguration(SavedGame savedGame)
    {
        return savedGame.Configuration!;
    }

    private void CheckAndCreateInitialFolder()
    {
        if (!Directory.Exists(FileHelper.BasePath))
        {
            Directory.CreateDirectory(FileHelper.BasePath);
        }
    }

    private void CreateNewSavedGameFile(SavedGame savedGame)
    {
        var fileName = FileHelper.BasePath + 
                       savedGame.Name + 
                       FileHelper.GameExtension;
        
        File.WriteAllText(fileName, JsonSerializer.Serialize(savedGame));
    }
}