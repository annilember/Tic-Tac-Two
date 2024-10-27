using System.Reflection;
using System.Runtime.CompilerServices;
using ConsoleUI;
using DAL;
using GameBrain;
using MenuSystem;

namespace ConsoleApp;

public static class OptionsController
{
    private static readonly IConfigRepository ConfigRepository = new ConfigRepositoryJson();
    private static readonly IGameRepository GameRepository = new GameRepositoryJson();
    
    public static string CreateNewConfig()
    {
        var input = "";
        
        do
        {
            Visualizer.WriteInsertConfigNameInstructions();
            input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                continue;
            }
            if (input.Equals(ControllerHelper.ReturnValue, StringComparison.InvariantCultureIgnoreCase))
            {
                return ControllerHelper.ReturnValue;
            }
            break;
        } while (true);
        
        var newConfig = new GameConfiguration
        {
            Name = input
        };
        ConfigRepository.AddNewConfiguration(newConfig);
        
        return ChangeConfiguration(newConfig);;
    }

    private static GameConfiguration ChangePropertyValueMode(GameConfiguration config, PropertyInfo propertyInfo)
    {
        var input = "";
        var errormessage = "";
        
        do
        {
            Visualizer.WriteInsertNewPropertyValueInstructions(propertyInfo.Name, errormessage);
            input = Console.ReadLine();
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
                errormessage = $"{propertyInfo.Name} value has to range from {minBound} to {maxBound}!";
                
            }
            else if (propertyInfo.PropertyType == typeof(string))
            {
                return SetNewStringValueProperty(config, propertyInfo, input);
            }
            else
            {
                errormessage = "Input type is invalid! Try again!";
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
        do
        {
            var chosenPropertyShortcut = ChooseConfigurationProperty(config);
            if (!int.TryParse(chosenPropertyShortcut, out var propertyNo))
            {
                return chosenPropertyShortcut;
            }

            var propertyInfo = GameConfigurationHelper.GetConfigPropertyInfo(config)[propertyNo];
        
            config = ChangePropertyValueMode(config, propertyInfo);
            ConfigRepository.SaveConfigurationChanges(config);
            
        } while (true);
    }
    
    private static string ChooseConfigurationProperty(GameConfiguration config)
    {
        var propertyMenuItems = new List<MenuItem>();

        for (var i = 0; i < GameConfigurationHelper.GetConfigPropertyInfo(config).Length; i++)
        {
            var returnValue = i.ToString();
            var propertyName = GameConfigurationHelper.GetConfigPropertyInfo(config)[i].Name;
            var propertyValue = GameConfigurationHelper.GetConfigPropertyInfo(config)[i].GetValue(config);
            
            propertyMenuItems.Add(new MenuItem()
            {
                Shortcut = (i + 1).ToString(),
                Title = $"{propertyName}: {propertyValue}",
                MenuItemAction = () => returnValue
            });
        }
    
        var propertyMenu = new Menu(EMenuLevel.Deep,
            "TIC-TAC-TWO: - choose property to change",
            propertyMenuItems);

        return propertyMenu.Run();
    }

    public static string ChangeExistingConfiguration()
    {
        var chosenConfigShortcut = GameController.ChooseConfigurationFromMenu();
        
        if (!int.TryParse(chosenConfigShortcut, out var configNo))
        {
            return chosenConfigShortcut;
        }
        var chosenConfig = ConfigRepository.GetConfigurationByName(
            ConfigRepository.GetConfigurationNames()[configNo]);
        
        return ChangeConfiguration(chosenConfig);
    }

    public static string DeleteExistingConfiguration()
    {
        var chosenConfigShortcut = GameController.ChooseConfigurationFromMenu();
        
        if (!int.TryParse(chosenConfigShortcut, out var configNo))
        {
            return chosenConfigShortcut;
        }
        var chosenConfig = ConfigRepository.GetConfigurationByName(
            ConfigRepository.GetConfigurationNames()[configNo]);
        ConfigRepository.DeleteConfiguration(chosenConfig);
         return ControllerHelper.ReturnValue;
    }
    
    public static string DeleteSavedGame()
    {
        var chosenGameShortcut = GameController.ChooseGameToLoadFromMenu();
        
        if (!int.TryParse(chosenGameShortcut, out var gameNo))
        {
            return chosenGameShortcut;
        }
        var gameName = GameRepository.GetGameNames()[gameNo];
        GameRepository.DeleteGame(gameName);
        
        return ControllerHelper.ReturnValue;
    }
}
