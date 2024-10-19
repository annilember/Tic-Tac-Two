using ConsoleUI;
using DAL;
using GameBrain;
using MenuSystem;

namespace ConsoleApp;

public static class GameController
{
    private static readonly IConfigRepository ConfigRepository = new ConfigRepositoryJson();
    private static readonly IGameRepository GameRepository = new GameRepositoryJson();
    
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
        // add whose turn it is
        // is game over?

        var errorMessage = "";
        // var winner = EGamePiece.Empty;

        do
        {
            Visualizer.DrawBoard(gameInstance);
            // Console.WriteLine($"{winner} wins!");
            Visualizer.WriteInstructions(errorMessage);
            var input = HandleInput(gameInstance, Console.ReadLine()!);
            if (input == "R") break;
            errorMessage = input;
            // check if game is over
            // winner = gameInstance.CheckForWinner();

        } while (gameInstance.CheckForWinner() == EGamePiece.Empty);
    
        // Console.WriteLine($"{gameInstance.CheckForWinner()} wins!");
        return "R";
    }

    private static string HandleInput(TicTacTwoBrain gameInstance, string input)
    {
        // TODO: separate 2 functionalities if can - input validation and making a move.
        
        var errorMessage = "";
        
        if (input.Equals("save", StringComparison.InvariantCultureIgnoreCase))
        {
            GameRepository.SaveGame(
                gameInstance.GetGameStateJson(), 
                gameInstance.GetGameConfigName()
            );
        }
        else if (input.Equals("r", StringComparison.InvariantCultureIgnoreCase))
        {
            return "R";
        }
        else
        {
            var inputSplit = input.Split(',');
            if (inputSplit.Length != 2)
            {
                return "One number for X and one number for Y please!";
            }
            if (!int.TryParse(inputSplit[0], out var inputX))
            {
                return "Insert valid X coordinate!";
            }
            if (!int.TryParse(inputSplit[1], out var inputY))
            {
                return "Insert valid Y coordinate!";
            }
                
            try
            {
                if (!gameInstance.MakeAMove(inputX, inputY))
                {
                    return "Space occupied! Try again!";
                }
            }
            catch (Exception)
            {
                return "Invalid coordinates! Please stay inside the board!";
            }
            errorMessage = "";
        }

        return errorMessage;
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