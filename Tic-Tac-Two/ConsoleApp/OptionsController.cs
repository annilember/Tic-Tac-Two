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
            if (ConfigRepository.ConfigurationExists(input))
            {
                errorMessage = ControllerHelper.ConfigNameAlreadyInUseMessage;
                continue;
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
                if (!ConfigRepository.ConfigurationExists(input))
                    return SetNewStringValueProperty(config, propertyInfo, input);
                errorMessage = ControllerHelper.ConfigNameAlreadyInUseMessage;
            }
            else
            {
                errorMessage = ControllerHelper.InvalidInputMessage;
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
        
            config = ChangePropertyValueMode(config, propertyInfo);
            ConfigRepository.SaveConfigurationChanges(config);
            message = ControllerHelper.PropertySavedMessage;

        } while (true);
    }
    
    private static string ChooseConfigurationProperty(GameConfiguration config, string message)
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

        return propertyMenu.Run(message);
    }

    public static string ChangeExistingConfiguration()
    {
        var chosenConfigShortcut = GameController.ChooseConfigurationFromMenu(
            EMenuLevel.Deep,
            ControllerHelper.ChangeConfigMenuHeader
        );
        
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
        var chosenConfigShortcut = GameController.ChooseConfigurationFromMenu(
            EMenuLevel.Deep,
            ControllerHelper.DeleteConfigMenuHeader
        );
        
        if (!int.TryParse(chosenConfigShortcut, out var configNo))
        {
            return chosenConfigShortcut;
        }
        var chosenConfig = ConfigRepository.GetConfigurationByName(
            ConfigRepository.GetConfigurationNames()[configNo]);
        ConfigRepository.DeleteConfiguration(chosenConfig);
        
        return ControllerHelper.ConfigDeletedMessage;
    }
    
    public static string DeleteSavedGame()
    {
        var chosenGameShortcut = GameController.ChooseGameToLoadFromMenu(
            EMenuLevel.Deep, 
            ControllerHelper.DeleteGameMenuHeader
            );
        
        if (!int.TryParse(chosenGameShortcut, out var gameNo))
        {
            return chosenGameShortcut;
        }
        var gameName = GameRepository.GetGameNames()[gameNo];
        GameRepository.DeleteGame(gameName);
        
        return ControllerHelper.GameDeletedMessage;
    }
}
