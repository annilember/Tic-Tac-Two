﻿using Domain;

namespace DAL;

public class ConfigRepositoryHardCoded: IConfigRepository
{
    private readonly List<GameConfiguration> _gameConfigurations =
    [
        new()
        {
            Id = 1,
            Name = "Classical"
        },

        new()
        {
            Id = 2,
            Name = "Big board",
            BoardSizeWidth = 10,
            BoardSizeHeight = 10,
            GridSizeWidth = 4,
            GridSizeHeight = 4,
            GridStartPosX = 3,
            GridStartPosY = 3,
            NumberOfPieces = 6,
            WinCondition = 3,
            MaxGameRounds = 10,
            MoveGridAfterNMoves = 3,
            MovePieceAfterNMoves = 3
        },

        new()
        {
            Id = 3,
            Name = "Tic-Tac-Toe",
            BoardSizeWidth = 3,
            BoardSizeHeight = 3,
            GridSizeWidth = 3,
            GridSizeHeight = 3,
            GridStartPosX = 0,
            GridStartPosY = 0,
            NumberOfPieces = 5,
            WinCondition = 3,
            MaxGameRounds = 5,
            MoveGridAfterNMoves = 5,
            MovePieceAfterNMoves = 5
        }
    ];

    public List<GameConfiguration> GetConfigurations()
    {
        return _gameConfigurations;
    }
    
    public List<string> GetConfigurationNames()
    {
        return _gameConfigurations.Select(config => config.Name).ToList();
    }

    public GameConfiguration GetConfigurationByName(string name)
    {
        return _gameConfigurations.Single(c => c.Name == name);
    }
    
    public GameConfiguration GetConfigurationById(int id)
    {
        return _gameConfigurations.Single(c => c.Id == id);
    }
    
    public bool ConfigurationExists(string name)
    {
        return GetConfigurationNames().Any(configName => name == configName);
    }
    
    public void AddNewConfiguration(GameConfiguration config)
    {
        _gameConfigurations.Add(config);
    }
    
    public void SaveConfigurationChanges(GameConfiguration config, string previousName)
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
        
        if (previousName != config.Name && ConfigurationExists(previousName))
        {
            DeleteConfiguration(GetConfigurationByName(previousName));
        }
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
