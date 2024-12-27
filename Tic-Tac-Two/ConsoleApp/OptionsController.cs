using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using ConsoleUI;
using DAL;
using Domain;
using GameBrain;
using MenuSystem;

namespace ConsoleApp;

public static class OptionsController
{
    private static IConfigRepository _configRepository = default!;
    private static IGameRepository _gameRepository = default!;

    public static void Init(IConfigRepository configRepository, IGameRepository gameRepository)
    {
        _configRepository = configRepository;
        _gameRepository = gameRepository;
    }
    
    public static string CreateNewConfig()
    {
        var input = "";
        var errorMessage = "";
        
        do
        {
            Console.Clear();
            Visualizer.WriteInsertConfigNameInstructions(errorMessage);
            input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                continue;
            }
            if (input.Equals(ControllerHelper.ReturnValue, StringComparison.InvariantCultureIgnoreCase))
            {
                return ControllerHelper.ReturnValue;
            }
            if (_configRepository.ConfigurationExists(input))
            {
                errorMessage = Message.ConfigNameAlreadyInUseMessage;
                continue;
            }
            break;
        } while (true);
        
        var newConfig = new GameConfiguration
        {
            Name = input
        };
        _configRepository.AddNewConfiguration(newConfig);
        
        return ChangeConfiguration(newConfig);;
    }

    private static GameConfiguration ChangePropertyValueMode(GameConfiguration config, PropertyInfo propertyInfo)
    {
        var errorMessage = "";
        
        do
        {
            Visualizer.WriteInsertNewPropertyValueInstructions(propertyInfo.Name, errorMessage);
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                continue;
            }
            if (input.Equals(ControllerHelper.ReturnValue, StringComparison.InvariantCultureIgnoreCase))
            {
                return config;
            }
            
            if (propertyInfo.PropertyType == typeof(int) && int.TryParse(input, out var value))
            {
                var propertyBoundsDictionary = GameConfigurationHelper.GetConfigPropertyBoundsDictionary(config);
                var minBound = propertyBoundsDictionary[propertyInfo.Name][0];
                var maxBound = propertyBoundsDictionary[propertyInfo.Name][1];
            
                if (value >= minBound && value <= maxBound)
                {
                    return SetNewIntValueProperty(config, propertyInfo, value);
                }
                errorMessage = $"{propertyInfo.Name} value has to range from {minBound} to {maxBound}!";
                
            }
            else if (propertyInfo.PropertyType == typeof(string) &&
                     propertyInfo.Name.Equals("Name", StringComparison.InvariantCultureIgnoreCase))
            {
                if (!_configRepository.ConfigurationExists(input))
                    return SetNewStringValueProperty(config, propertyInfo, input);
                errorMessage = Message.ConfigNameAlreadyInUseMessage;
            }
            else
            {
                errorMessage = Message.InvalidInputMessage;
            }
            
        } while (true);
    }

    private static GameConfiguration SetNewIntValueProperty(GameConfiguration config, PropertyInfo propertyInfo, int value)
    {
        object boxedObject = RuntimeHelpers.GetObjectValue(config);
        config.GetType().GetProperty(propertyInfo.Name)!.SetValue(boxedObject, value);
        config = (GameConfiguration)boxedObject;
        return config;
    }
    
    private static GameConfiguration SetNewStringValueProperty(GameConfiguration config, PropertyInfo propertyInfo, string value)
    {
        object boxedObject = RuntimeHelpers.GetObjectValue(config);
        config.GetType().GetProperty(propertyInfo.Name)!.SetValue(boxedObject, value);
        config = (GameConfiguration)boxedObject;
        return config;
    }
    
    private static string ChangeConfiguration(GameConfiguration config)
    {
        var message = "";
        do
        {
            Console.WriteLine(message);
            var chosenPropertyShortcut = ChooseConfigurationProperty(config, message);
            if (!int.TryParse(chosenPropertyShortcut, out var propertyNo))
            {
                return chosenPropertyShortcut;
            }

            var propertyInfo = GameConfigurationHelper.GetConfigPropertyInfo(config)[propertyNo];
        
            var configOldName = config.Name;
            config = ChangePropertyValueMode(config, propertyInfo);
            _configRepository.SaveConfigurationChanges(config, configOldName);
            message = Message.PropertySavedMessage;

        } while (true);
    }
    
    private static string ChooseConfigurationProperty(GameConfiguration config, string message)
    {
        var propertyMenuItems = new List<MenuItem>();

        for (var i = 1; i < GameConfigurationHelper.GetConfigPropertyInfo(config).Length - 1; i++)
        {
            var returnValue = i.ToString();
            var propertyName = GameConfigurationHelper.GetConfigPropertyInfo(config)[i].Name;
            var propertyValue = GameConfigurationHelper.GetConfigPropertyInfo(config)[i].GetValue(config);
            
            propertyMenuItems.Add(new MenuItem()
            {
                Shortcut = i.ToString(),
                Title = $"{propertyName}: {propertyValue}",
                MenuItemAction = () => returnValue
            });
        }
    
        var propertyMenu = new Menu(EMenuLevel.Deep,
            "TIC-TAC-TWO: - choose property to change",
            propertyMenuItems);

        return propertyMenu.Run(message);
    }

    public static string ChangeExistingConfiguration()
    {
        var chosenConfigShortcut = ChooseConfigurationFromMenu(
            EMenuLevel.Deep,
            VisualizerHelper.ChangeConfigMenuHeader
        );
        
        if (!int.TryParse(chosenConfigShortcut, out var configNo))
        {
            return chosenConfigShortcut;
        }
        var chosenConfig = _configRepository.GetConfigurationByName(
            _configRepository.GetConfigurationNames()[configNo]);
        
        return ChangeConfiguration(chosenConfig);
    }

    public static string DeleteExistingConfiguration()
    {
        var chosenConfigShortcut = ChooseConfigurationFromMenu(
            EMenuLevel.Deep,
            VisualizerHelper.DeleteConfigMenuHeader
        );
        
        if (!int.TryParse(chosenConfigShortcut, out var configNo))
        {
            return chosenConfigShortcut;
        }
        var chosenConfig = _configRepository.GetConfigurationByName(
            _configRepository.GetConfigurationNames()[configNo]);
        _configRepository.DeleteConfiguration(chosenConfig);
        
        return Message.ConfigDeletedMessage;
    }
    
    public static string ChooseConfigurationFromMenu(EMenuLevel menuLevel, string menuHeader)
    {
        var configMenuItems = new List<MenuItem>();

        for (var i = 0; i < _configRepository.GetConfigurationNames().Count; i++)
        {
            var returnValue = i.ToString();
            configMenuItems.Add(new MenuItem()
            {
                Shortcut = (i + 1).ToString(),
                Title = _configRepository.GetConfigurationNames()[i],
                MenuItemAction = () => returnValue
            });
        }
    
        var configMenu = new Menu(menuLevel,
            menuHeader,
            configMenuItems);

        return configMenu.Run();
    }

    public static string ChooseGameModeFromMenu()
    {
        var gameModeMenuItems = new List<MenuItem>();
        var gameModes = GameMode.GetGameModeNames();

        for (var i = 0; i < gameModes.Count; i++)
        {
            var returnValue = i.ToString();
            gameModeMenuItems.Add(new MenuItem()
            {
                Shortcut = (i + 1).ToString(),
                Title = gameModes[i],
                MenuItemAction = () => returnValue
            });
        }
    
        var gameModeMenu = new Menu(EMenuLevel.Deep,
            VisualizerHelper.ChooseGameModeMenuHeader,
            gameModeMenuItems);

        return gameModeMenu.Run();
    }

    public static string DeleteSavedGame()
    {
        var chosenGameShortcut = ChooseGameFromMenu(
            EMenuLevel.Deep, 
            VisualizerHelper.DeleteGameMenuHeader
            );
        
        if (!int.TryParse(chosenGameShortcut, out var gameNo))
        {
            return chosenGameShortcut;
        }
        var gameName = _gameRepository.GetGameNames()[gameNo];
        _gameRepository.DeleteGame(gameName);
        
        return Message.GameDeletedMessage;
    }
    
    public static string RenameSavedGame()
    {
        do
        {
            var chosenGameShortcut = ChooseGameFromMenu(
                EMenuLevel.Deep, 
                VisualizerHelper.RenameGameMenuHeader
            );
        
            if (!int.TryParse(chosenGameShortcut, out var gameNo))
            {
                return chosenGameShortcut;
            }
            var gameName = _gameRepository.GetGameNames()[gameNo];
            var savedGame = _gameRepository.GetSavedGameByName(gameName);
            var newGameName = GetNewGameName();
        
            if (newGameName == ControllerHelper.ReturnValue) continue;
        
            _gameRepository.RenameGame(savedGame, newGameName);
            return Message.GameRenamedMessage;
            
        } while (true);
    }
    
    public static string ChooseGameFromMenu(EMenuLevel menuLevel, string menuHeader)
    {
        var gameMenuItems = new List<MenuItem>();
        
        for (var i = 0; i < _gameRepository.GetGameNames().Count; i++)
        {
            var returnValue = i.ToString();
            gameMenuItems.Add(new MenuItem()
            {
                Shortcut = (i + 1).ToString(),
                Title = _gameRepository.GetGameNames()[i],
                MenuItemAction = () => returnValue
            });
        }

        if (gameMenuItems.Count == 0)
        {
            return Message.NoSavedGamesMessage;
        }
    
        var configMenu = new Menu(menuLevel,
            menuHeader,
            gameMenuItems);

        return configMenu.Run();
    }

    private static string GetNewGameName()
    {
        var errorMessage = "";
        do
        {
            Console.Clear();
            Visualizer.WriteInsertNewGameNameInstructions(errorMessage);
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                continue;
            }
            if (input.Equals(ControllerHelper.ReturnValue, StringComparison.InvariantCultureIgnoreCase))
            {
                return ControllerHelper.ReturnValue;
            }

            if (!_gameRepository.GameExists(input)) return input;
            
            errorMessage = Message.GameNameAlreadyInUseMessage;

        } while (true);
    }

    public static string DisplayGameRules()
    {
        Visualizer.WriteGameRulesPage();
        Console.ReadKey();
        return ControllerHelper.ReturnValue;
    }
}
