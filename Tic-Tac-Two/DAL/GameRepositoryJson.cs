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
    
    public void SaveGame(SavedGame savedGame)
    {
        //TODO: added this because of DB, but haven't checked if works.
        CreateNewSavedGameFile(savedGame);
    }
    
    public void SaveGame(TicTacTwoBrain gameInstance, string name)
    {
        var savedGame = CreateNewSavedGame(gameInstance, name);
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
    
    public void CreateGame(SavedGame savedGame)
    {
        // TODO: added with WebApp. Check that it works.
        CreateNewSavedGameFile(savedGame);
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
    
    private SavedGame CreateNewSavedGame(TicTacTwoBrain gameInstance, string name)
    {
        var savedGame = new SavedGame
        {
            Name = name,
            ModeName = gameInstance.GetGameModeName(),
            PlayerXName = gameInstance.GetPlayerName(EGamePiece.X),
            PlayerOName = gameInstance.GetPlayerName(EGamePiece.O),
            PlayerXPassword = "xxx",
            PlayerOPassword = "ooo",
            CreatedAtDateTime = DateTime.Now.ToString("O"),
            State = gameInstance.GetGameStateJson(),
            Configuration = gameInstance.GetGameConfig()
        };
        
        if (name == "")
        {
            savedGame.Name = gameInstance.GetGameConfigName() + " " + savedGame.CreatedAtDateTime;
        }
        return savedGame;
    }

    private void CreateNewSavedGameFile(SavedGame savedGame)
    {
        var fileName = FileHelper.BasePath + 
                       savedGame.Name + 
                       FileHelper.GameExtension;
        
        File.WriteAllText(fileName, JsonSerializer.Serialize(savedGame));
    }
}