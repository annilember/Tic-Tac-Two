using Domain;
using DTO;
using GameBrain;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class GameRepositoryDb(AppDbContext db) : IGameRepository
{
    public List<SavedGame> GetSavedGames()
    {
        return db.SavedGames.Include(s => s.Configuration).ToList();
    }
    
    public List<string> GetGameNames()
    {
        return db.SavedGames.Select(savedGame => savedGame.Name).ToList();
    }

    public SavedGame GetSavedGameByName(string name)
    {
        var savedGame = db.SavedGames
            .Include(game => game.Configuration)
            .FirstOrDefault(g => g.Name == name);
        return savedGame ?? new SavedGame();
    }

    public bool GameExists(string name)
    {
        return GetGameNames().Any(gameName => name == gameName);
    }

    public void SaveGame(TicTacTwoBrain gameInstance)
    {
        var savedGame = gameInstance.SavedGame;
        savedGame.State = gameInstance.GetGameStateJson();
        
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
        
        db.SavedGames.Add(savedGame);
        db.SaveChangesAsync();
        
        return savedGame;
    }

    public GameConfiguration GetGameConfiguration(SavedGame savedGame)
    {
        return db.Configurations.Find(savedGame.ConfigurationId)!;
    }
}
