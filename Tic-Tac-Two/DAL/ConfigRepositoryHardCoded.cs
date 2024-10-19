using GameBrain;

namespace DAL;

public class ConfigRepositoryHardCoded: IConfigRepository
{
    private List<GameConfiguration> _gameConfigurations = new List<GameConfiguration>()
    {
        new GameConfiguration()
        {
            Name = "Classical"
        },
        new GameConfiguration()
        {
            Name = "Big board",
            BoardSizeWidth = 10,
            BoardSizeHeight = 10,
            GridSizeWidth = 4,
            GridSizeHeight = 4,
            GridStartPosX = 3,
            GridStartPosY = 3,
            WinCondition = 3,
            MovePieceAfterNMoves = 3
        }
    };

    public List<string> GetConfigurationNames()
    {
        return _gameConfigurations.Select(config => config.Name).ToList();
    }

    public GameConfiguration GetConfigurationByName(string name)
    {
        return _gameConfigurations.Single(c => c.Name == name);
    }

    public string Test()
    {
        return "Hello World!";
    }
}