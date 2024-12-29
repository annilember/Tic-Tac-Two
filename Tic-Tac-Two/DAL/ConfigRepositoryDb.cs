using Domain;

namespace DAL;

public class ConfigRepositoryDb(AppDbContext db) : IConfigRepository
{
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
        db.Configurations.Update(config);
        db.SaveChanges();
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
            var configNames = hardcodedRepo.GetConfigurationNames();
            foreach (var configName in configNames)
            {
                var gameConfig = hardcodedRepo.GetConfigurationByName(configName);
                db.Configurations.Add(gameConfig);
            }
            db.SaveChanges();
    }
}