using DAL;
using GameBrain;
using MenuSystem;

namespace ConsoleApp;

public static class GameController
{
    private static readonly ConfigRepository ConfigRepository = new ConfigRepository();
    
    public static string MainLoop()
    {
        var chosenConfigShortcut = ChooseConfiguration();
        
        if (!int.TryParse(chosenConfigShortcut, out var configNo))
        {
            return chosenConfigShortcut;
        }
        var chosenConfig = ConfigRepository.GetConfigurationByName(
            ConfigRepository.GetConfigurationNames()[configNo]);
    
        var gameInstance = new TicTacTwoBrain(chosenConfig);
        
        
        // main loop of gameplay
        // draw the board again
        // ask input again, validate input
        // is game over?

        do
        {
            ConsoleUI.Visualizer.DrawBoard(gameInstance);
    
            Console.Write("Give me coordinates <x,y> or save:");
            var input = Console.ReadLine()!;
            var inputSplit = input.Split(',');
            var inputX = int.Parse(inputSplit[0]);
            var inputY = int.Parse(inputSplit[1]);
            gameInstance.MakeAMove(inputX, inputY);
            
        } while (true);
    
        return "";
    }

    private static string ChooseConfiguration()
    {
        var configMenuItems = new List<MenuItem>();

        for (var i = 0; i < ConfigRepository.GetConfigurationNames().Count; i++)
        {
            var returnValue = i.ToString();
            configMenuItems.Add(new MenuItem()
            {
                Shortcut = (i + 1).ToString(),
                Title = ConfigRepository.GetConfigurationNames()[i],
                MenuItemAction = () => returnValue
            });
        }
    
        var configMenu = new Menu(EMenuLevel.Deep,
            "TIC-TAC-TWO - choose game config",
            configMenuItems);

        return configMenu.Run();
    }
}