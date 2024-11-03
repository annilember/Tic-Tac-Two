using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class ConfigRepositoryDb  : IConfigRepository
{
    private static readonly string ConnectionString = $"Data Source={FileHelper.BasePath}app.db";

    private static readonly DbContextOptions<AppDbContext> ContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(ConnectionString)
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging()
            .Options;

    private readonly AppDbContext _context = new AppDbContext(ContextOptions);
    
    public List<string> GetConfigurationNames()
    {
        // using _context !?
        
        CheckAndCreateInitialConfig();

        return _context.Configurations.Select(config => config.Name).ToList();
    }

    public GameConfiguration GetConfigurationByName(string name)
    {
        // using _context !?

        foreach (var config in _context.Configurations)
        {
            if (config.Name == name)
            {
                return config;
            }
        }

        return new GameConfiguration();
    }

    public bool ConfigurationExists(string name)
    {
        return GetConfigurationNames().Any(configName => name == configName);
    }

    public void AddNewConfiguration(GameConfiguration gameConfig)
    {
        _context.Configurations.Add(gameConfig);
        _context.SaveChanges();
    }

    public void SaveConfigurationChanges(GameConfiguration config, string previousName)
    {
        _context.Configurations.Update(config);
        _context.SaveChanges();
    }

    public void DeleteConfiguration(GameConfiguration gameConfig)
    {
        _context.Configurations.Remove(gameConfig);
        _context.SaveChanges();
    }
    
    private void CheckAndCreateInitialConfig()
    {
            var configCount = _context.Configurations.Count();
            if (configCount != 0) return;
            
            var hardcodedRepo = new ConfigRepositoryHardCoded();
            var configNames = hardcodedRepo.GetConfigurationNames();
            foreach (var configName in configNames)
            {
                var gameConfig = hardcodedRepo.GetConfigurationByName(configName);
                _context.Configurations.Add(gameConfig);
            }
            _context.SaveChanges();
    }
}