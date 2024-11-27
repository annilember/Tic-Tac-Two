using System.Text.Json;
using Domain;
using GameBrain;

namespace DAL;

public class GameRepositoryDb(AppDbContext db) : IGameRepository
{
    public List<string> GetGameNames()
    {
        return db.SavedGames.Select(savedGame => savedGame.Name).ToList();
    }

    public SavedGame GetSavedGameByName(string name)
    {
        foreach (var savedGame in db.SavedGames)
        {
            if (savedGame.Name == name)
            {
                return savedGame;
            }
        }
        return new SavedGame();
    }

    public GameState GetSavedGameState(SavedGame savedGame)
    {
        return JsonSerializer.Deserialize<GameState>(savedGame.State)!;
    }

    public string GetSavedGameJsonByName(string name)
    {
        var savedGame = GetSavedGameByName(name);
        return savedGame.State;
    }

    public bool GameExists(string name)
    {
        return GetGameNames().Any(gameName => name == gameName);
    }

    public void SaveGame(TicTacTwoBrain gameInstance, string name)
    {
        var savedGame = CreateNewSavedGame(gameInstance, name);
        
        // .Update works, but not .Add !?
        db.SavedGames.Update(savedGame);
        db.SaveChanges();
    }

    public void RenameGame(SavedGame savedGame, string newName)
    {
        savedGame.Name = newName;
        db.SaveChanges();
    }

    public void DeleteGame(string name)
    {
        var savedGame = GetSavedGameByName(name);
        db.SavedGames.Remove(savedGame);
        db.SaveChanges();
    }

    public GameConfiguration GetGameConfiguration(SavedGame savedGame)
    {
        return db.Configurations.Find(savedGame.ConfigurationId)!;
    }

    private SavedGame CreateNewSavedGame(TicTacTwoBrain gameInstance, string name)
    {
        var savedGame = new SavedGame
        {
            Name = name,
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
}
