using GameBrain;

namespace DAL;

public interface IConfigRepository
{
    List<string> GetConfigurationNames();
    GameConfiguration GetConfigurationByName(string name);
    bool ConfigurationExists(string name);
    void AddNewConfiguration(GameConfiguration gameConfiguration);
    void SaveConfigurationChanges(GameConfiguration gameConfiguration);
    void DeleteConfiguration(GameConfiguration gameConfiguration);
}