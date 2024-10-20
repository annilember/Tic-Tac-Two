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
        // add whose turn it is

        var errorMessage = "";
        // var winner = EGamePiece.Empty;
        var input = "";
        
        do
        {
            Visualizer.DrawBoard(gameInstance);
            // Console.WriteLine($"{winner} wins!");
            Console.WriteLine($"Round number: {gameInstance.RoundNumber}");
            
            Visualizer.WriteInstructions(gameInstance, errorMessage);
            input = HandleInput(gameInstance, Console.ReadLine()!);
            if (input == "R") break;
            errorMessage = input;
            // check if game is over
            // winner = gameInstance.CheckForWinner();

        } while (gameInstance.CheckForWinner() == EGamePiece.Empty);

        input = "";
        
        do
        {
            Visualizer.DrawBoard(gameInstance);
            Visualizer.DisplayGameOverMessage();
            input = HandleInput2(gameInstance, Console.ReadLine()!);
        } while (input == "");

        // Console.WriteLine($"{gameInstance.CheckForWinner()} wins!");
        return "R";
    }

    private static string HandleInput2(TicTacTwoBrain gameInstance, string input)
    {
        if (input.Equals("r", StringComparison.InvariantCultureIgnoreCase))
        {
            return "R";
        }

        return "";
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
        else if (input.Equals("g", StringComparison.InvariantCultureIgnoreCase))
        {
            MoveGridMode(gameInstance);
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

    private static void MoveGridMode(TicTacTwoBrain gameInstance)
    {
        // gameInstance.MoveGridModeOn peaks muutma kuskil teises kohas?? turvalisemalt?
        gameInstance.MoveGridModeOn = true;
        var input = "";
        
        do
        {
            Visualizer.DrawBoard(gameInstance);
            Visualizer.WriteMoveGridModeInstructions();
            // input = HandleMoveGridModeInput(gameInstance, Console.ReadLine()!);

            while (true)
            {
                var key = Console.ReadKey(true).Key;
                switch(key)
                {
                    
                    case ConsoleKey.RightArrow:
                        Console.WriteLine("Right");
                        break;
                    case ConsoleKey.LeftArrow:
                        Console.WriteLine("Left");
                        break;
                    case ConsoleKey.UpArrow:
                        Console.WriteLine("Up");
                        break;
                    case ConsoleKey.DownArrow:
                        Console.WriteLine("Down");
                        break;
                    case ConsoleKey.Enter:
                        Console.WriteLine("Enter");
                        gameInstance.MoveGridModeOn = false;
                        return;
                    default:
                        // HandleMoveGridModeInput(gameInstance, key);
                        break;
                }
            }

            
        } while (true);
        return;
    }
    
    private static string HandleMoveGridModeInput(TicTacTwoBrain gameInstance, string input)
    {
        var errorMessage = "";
        
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