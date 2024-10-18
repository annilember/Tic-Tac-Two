using System.Text.Json;
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
        return config;
    }

    private void CheckAndCreateInitialConfig()
    {
        if (!Directory.Exists(FileHelper.BasePath))
        {
            Directory.CreateDirectory(FileHelper.BasePath);
        }
        var data = Directory.GetFiles(FileHelper.BasePath, FileHelper.AsteriskSymbol + FileHelper.ConfigExtension).ToList();
        if (data.Count == 0)
        {
            var hardcodedRepo = new ConfigRepositoryHardCoded();
            var optionNames = hardcodedRepo.GetConfigurationNames();
            foreach (var optionName in optionNames)
            {
                var gameOption = hardcodedRepo.GetConfigurationByName(optionName);
                var optionJsonStr = JsonSerializer.Serialize(gameOption);
                File.WriteAllText($"{FileHelper.BasePath}{optionName}{FileHelper.ConfigExtension}", optionJsonStr);
            }
        }
    }
}