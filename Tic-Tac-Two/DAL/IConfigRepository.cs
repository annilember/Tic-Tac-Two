using Domain;

namespace DAL;

public interface IConfigRepository
{
    List<GameConfiguration> GetConfigurations();
    List<string> GetConfigurationNames();
    GameConfiguration GetConfigurationByName(string name);
    bool ConfigurationExists(string name);
    void AddNewConfiguration(GameConfiguration gameConfiguration);
    void SaveConfigurationChanges(GameConfiguration gameConfiguration, string previousName);
    void DeleteConfiguration(GameConfiguration gameConfiguration);
}