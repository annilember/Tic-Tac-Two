using ConsoleUI;
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

        var errorMessage = "";

        do
        {
            Visualizer.DrawBoard(gameInstance);
            
            Console.WriteLine("'s turn!");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(errorMessage);
            Console.ResetColor();
            Console.Write("Give me coordinates <");
            Console.ForegroundColor = Visualizer.XAxisColor;
            Console.Write("x");
            Console.ResetColor();
            Console.Write(",");
            Console.ForegroundColor = Visualizer.YAxisColor;
            Console.Write("y");
            Console.ResetColor();
            Console.Write("> or save:");
            
            var input = Console.ReadLine()!;
            var inputSplit = input.Split(',');
            if (!int.TryParse(inputSplit[0], out var inputX))
            {
                errorMessage = "Insert valid X coordinate!";
                continue;
            }
            if (!int.TryParse(inputSplit[1], out var inputY))
            {
                errorMessage = "Insert valid Y coordinate!";
                continue;
            }

            try
            {
                if (!gameInstance.MakeAMove(inputX, inputY))
                {
                    errorMessage = "Space occupied! Try again!";
                    continue;
                }
            }
            catch (Exception)
            {
                errorMessage = "Invalid coordinates! Please stay inside the board!";
                continue;
            }
            errorMessage = "";
            
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