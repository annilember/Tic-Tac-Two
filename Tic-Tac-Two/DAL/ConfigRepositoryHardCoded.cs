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
            NumberOfPieces = 6,
            WinCondition = 3,
            MoveGridAfterNMoves = 3,
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
    
    public void AddNewConfiguration(GameConfiguration config)
    {
        _gameConfigurations.Add(config);
    }
    
    public void SaveConfigurationChanges(GameConfiguration config)
    {
        for (int i = 0; i < _gameConfigurations.Count; i++)
        {
            if (_gameConfigurations[i].Name == config.Name)
            {
                _gameConfigurations[i] = config;
                return;
            }
        }
        _gameConfigurations.Add(config);
        
    }
    
    public void DeleteConfiguration(GameConfiguration config)
    {
        for (int i = 0; i < _gameConfigurations.Count; i++)
        {
            if (_gameConfigurations[i].Name == config.Name)
            {
                _gameConfigurations.RemoveAt(i);
                return;
            }
        }
    }
}
