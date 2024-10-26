using System.Reflection;
using System.Runtime.CompilerServices;
using DAL;
using GameBrain;
using MenuSystem;

namespace ConsoleApp;

public static class OptionsController
{
    private static IConfigRepository _configRepository = new ConfigRepositoryJson();
    
    public static string CreateNewConfig()
    {
        var input = "";
        
        do
        {
            Console.Write("Enter configuration name or return <R>: ");
            input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                continue;
            }
            if (input.Equals("r", StringComparison.InvariantCultureIgnoreCase))
            {
                return "R";
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

    private static GameConfiguration ChangePropertyValue(GameConfiguration config, PropertyInfo propertyInfo)
    {
        var input = "";
        
        do
        {
            Console.Write("Enter new value for property or return <R>: ");
            input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                continue;
            }
            if (input.Equals("r", StringComparison.InvariantCultureIgnoreCase))
            {
                return config;
            }
            
            if (propertyInfo.PropertyType == typeof(int) && int.TryParse(input, out var value))
            {
                var boundsDictionary = GameConfigurationHelper.GetConfigPropertyBoundsDictionary(config);
                var bounds = boundsDictionary[propertyInfo.Name];
            
                if (value >= bounds[0] && value <= bounds[1])
                {
                    object boxedObject = RuntimeHelpers.GetObjectValue(config);
                    config.GetType().GetProperty(propertyInfo.Name)!.SetValue(boxedObject, value);
                    config = (GameConfiguration)boxedObject;
                    return config;
                }

                Console.WriteLine($"{propertyInfo.Name} value has to range from {bounds[0]} to {bounds[1]}!");

            }
            else if (propertyInfo.PropertyType == typeof(string))
            {
                object boxedObject = RuntimeHelpers.GetObjectValue(config);
                config.GetType().GetProperty(propertyInfo.Name)!.SetValue(boxedObject, input);
                config = (GameConfiguration)boxedObject;
                return config;
            }
            else
            {
                throw new ArgumentException("Invalid configuration provided.");
            }
            
        } while (true);

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
        
            config = ChangePropertyValue(config, propertyInfo);
            _configRepository.SaveConfigurationChanges(config);
            
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
            "TIC-TAC-TWO: - choose property",
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
        var chosenConfig = _configRepository.GetConfigurationByName(
            _configRepository.GetConfigurationNames()[configNo]);
        
        return ChangeConfiguration(chosenConfig);
    }

    public static string DeleteExistingConfiguration()
    {
        var chosenConfigShortcut = GameController.ChooseConfigurationFromMenu();
        
        if (!int.TryParse(chosenConfigShortcut, out var configNo))
        {
            return chosenConfigShortcut;
        }
        var chosenConfig = _configRepository.GetConfigurationByName(
            _configRepository.GetConfigurationNames()[configNo]);
        _configRepository.DeleteConfiguration(chosenConfig);
         return "R";
    }
}
