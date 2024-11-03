using System.Text.Json;
using Domain;
using GameBrain;

namespace DAL;

public class ConfigRepositoryJson : IConfigRepository
{
    public List<string> GetConfigurationNames()
    {
        CheckAndCreateInitialConfig();

        return Directory
            .GetFiles(FileHelper.BasePath, FileHelper.AsteriskSymbol + FileHelper.ConfigExtension)
            .Select(fullFileName =>
                Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(fullFileName)))
            .ToList();
    }

    public GameConfiguration GetConfigurationByName(string name)
    {
        var configJsonStr = File.ReadAllText(FileHelper.BasePath + name + FileHelper.ConfigExtension);
        var config = JsonSerializer.Deserialize<GameConfiguration>(configJsonStr);
        return config!;
    }

    public bool ConfigurationExists(string name)
    {
        return GetConfigurationNames().Any(configName => name == configName);
    }

    public void AddNewConfiguration(GameConfiguration config)
    {
        CreateNewConfigFile(config);
    }

    public void SaveConfigurationChanges(GameConfiguration config, string previousName)
    {
        CreateNewConfigFile(config);
        if (previousName != config.Name && ConfigurationExists(previousName))
        {
            DeleteConfiguration(GetConfigurationByName(previousName));
        }
    }

    public void DeleteConfiguration(GameConfiguration config)
    {
        File.Delete(FileHelper.BasePath + config.Name + FileHelper.ConfigExtension);
    }

    private void CheckAndCreateInitialConfig()
    {
        if (!Directory.Exists(FileHelper.BasePath))
        {
            Directory.CreateDirectory(FileHelper.BasePath);
        }
        var data = Directory.GetFiles(FileHelper.BasePath, FileHelper.AsteriskSymbol + FileHelper.ConfigExtension).ToList();
        if (data.Count != 0) return;
        
        var hardcodedRepo = new ConfigRepositoryHardCoded();
        var configNames = hardcodedRepo.GetConfigurationNames();
        foreach (var configName in configNames)
        {
            var gameConfig = hardcodedRepo.GetConfigurationByName(configName);
            CreateNewConfigFile(gameConfig);
        }
    }

    private void CreateNewConfigFile(GameConfiguration config)
    {
        var configJsonStr = JsonSerializer.Serialize(config);
        File.WriteAllText($"{FileHelper.BasePath}{config.Name}{FileHelper.ConfigExtension}", configJsonStr);
    }
}