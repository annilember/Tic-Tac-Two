using Domain;

namespace DAL;

public class ConfigRepositoryDb(AppDbContext db) : IConfigRepository
{
    public List<GameConfiguration> GetConfigurations()
    {
        CheckAndCreateInitialConfig();
        return db.Configurations.ToList();
    }
    public List<string> GetConfigurationNames()
    {
        CheckAndCreateInitialConfig();
        return db.Configurations.Select(config => config.Name).ToList();
    }

    public GameConfiguration GetConfigurationByName(string name)
    {
        foreach (var config in db.Configurations)
        {
            if (config.Name == name)
            {
                return config;
            }
        }

        return new GameConfiguration();
    }
    
    public GameConfiguration GetConfigurationById(int id)
    {
        return db.Configurations.FirstOrDefault(config => config.Id == id) ?? new GameConfiguration();
    }

    public bool ConfigurationExists(string name)
    {
        return GetConfigurationNames().Any(configName => name == configName);
    }

    public void AddNewConfiguration(GameConfiguration gameConfig)
    {
        db.Configurations.Add(gameConfig);
        db.SaveChanges();
    }

    public void SaveConfigurationChanges(GameConfiguration config, string previousName)
    {
        var updatedConfig = SetNewPropertyValues(config, previousName);
        if (updatedConfig != null)
        {
            config = updatedConfig;
        }
        db.Configurations.Update(config);
        db.SaveChanges();
    }

    private GameConfiguration? SetNewPropertyValues(GameConfiguration newConfig, string previousName)
    {
        var config = GetConfigurationByName(previousName);
        if (config.Id == 0)
        {
            return null;
        }
        config.Name = newConfig.Name;
        config.BoardSizeWidth = newConfig.BoardSizeWidth;
        config.BoardSizeHeight = newConfig.BoardSizeHeight;
        config.GridSizeWidth = newConfig.GridSizeWidth;
        config.GridSizeHeight = newConfig.GridSizeHeight;
        config.GridStartPosX = newConfig.GridStartPosX;
        config.GridStartPosY = newConfig.GridStartPosY;
        config.NumberOfPieces = newConfig.NumberOfPieces;
        config.WinCondition = newConfig.WinCondition;
        config.MaxGameRounds = newConfig.MaxGameRounds;
        config.MoveGridAfterNMoves = newConfig.MoveGridAfterNMoves;
        config.MovePieceAfterNMoves = newConfig.MovePieceAfterNMoves;
        return config;
    }

    public void DeleteConfiguration(GameConfiguration gameConfig)
    {
        db.Configurations.Remove(gameConfig);
        db.SaveChanges();
    }
    
    public void CheckAndCreateInitialConfig()
    {
            var configCount = db.Configurations.Count();
            if (configCount != 0) return;
            
            var hardcodedRepo = new ConfigRepositoryHardCoded();
            var configs = hardcodedRepo.GetConfigurations();
            foreach (var config in configs)
            {
                db.Configurations.Add(config);
            }
            
            db.SaveChanges();
    }
}