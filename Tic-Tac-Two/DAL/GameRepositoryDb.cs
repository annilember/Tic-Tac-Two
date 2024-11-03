using System.Text.Json;
using Domain;
using GameBrain;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class GameRepositoryDb : IGameRepository
{
    private static readonly string ConnectionString = $"Data Source={FileHelper.BasePath}app.db";

    private static readonly DbContextOptions<AppDbContext> ContextOptions = new DbContextOptionsBuilder<AppDbContext>()
        .UseSqlite(ConnectionString)
        .EnableDetailedErrors()
        .EnableSensitiveDataLogging()
        .Options;

    private readonly AppDbContext _context = new AppDbContext(ContextOptions);
    
    public List<string> GetGameNames()
    {
        return _context.SavedGames.Select(savedGame => savedGame.Name).ToList();
    }

    public SavedGame GetSavedGameByName(string name)
    {
        foreach (var savedGame in _context.SavedGames)
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
        _context.SavedGames.Update(savedGame);
        _context.SaveChanges();
    }

    public void RenameGame(SavedGame savedGame, string newName)
    {
        savedGame.Name = newName;
        _context.SaveChanges();
    }

    public void DeleteGame(string name)
    {
        var savedGame = GetSavedGameByName(name);
        _context.SavedGames.Remove(savedGame);
        _context.SaveChanges();
    }

    public GameConfiguration GetGameConfiguration(SavedGame savedGame)
    {
        return _context.Configurations.Find(savedGame.ConfigurationId)!;
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
