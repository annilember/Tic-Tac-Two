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
        var chosenConfigShortcut = ChooseConfigurationFromMenu();
        
        if (!int.TryParse(chosenConfigShortcut, out var configNo))
        {
            return chosenConfigShortcut;
        }
        var chosenConfig = ConfigRepository.GetConfigurationByName(
            ConfigRepository.GetConfigurationNames()[configNo]);
        
        return MainLoopMethod(new TicTacTwoBrain(chosenConfig));
    }
    
    public static string LoadGameMainLoop()
    {
        var chosenGameShortcut = LoadGame();
        
        if (!int.TryParse(chosenGameShortcut, out var gameNo))
        {
            return chosenGameShortcut;
        }
        var chosenGameState = GameRepository.GetGameStateByName(
            GameRepository.GetGameNames()[gameNo]);

        if (chosenGameState != null)
        {
            return MainLoopMethod(new TicTacTwoBrain(chosenGameState));
        }

        return "R";
    }

    private static string MainLoopMethod(TicTacTwoBrain gameInstance)
    {
        // main loop of gameplay
        // add whose turn it is
        
        var errorMessage = "";
        var input = "";
        
        do
        {
            Visualizer.DrawBoard(gameInstance);
            Console.WriteLine($"Round number: {gameInstance.GameRoundNumber}");
            
            Visualizer.WriteInstructions(gameInstance, errorMessage);
            input = HandleInput(gameInstance, Console.ReadLine()!);
            if (input == "R") break;
            errorMessage = input;

        } while (gameInstance.CheckForWinner() == EGamePiece.Empty);
        
        do
        {
            input = "";
            Visualizer.DrawBoard(gameInstance);
            Visualizer.DisplayGameOverMessage();
            input = HandleGameOverPageInput(gameInstance, Console.ReadLine()!);
        } while (input == "");

        // Console.WriteLine($"{gameInstance.CheckForWinner()} wins!");
        return "R";
    }

    private static string HandleGameOverPageInput(TicTacTwoBrain gameInstance, string input)
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
        else if (input.Equals("p", StringComparison.InvariantCultureIgnoreCase) && gameInstance.CanMovePiece())
        {
            MovePieceMode(gameInstance);
        }
        else if (input.Equals("g", StringComparison.InvariantCultureIgnoreCase) && gameInstance.CanMoveGrid())
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
                if (gameInstance.MovePieceModeOn)
                {
                    if (!gameInstance.RemovePiece(inputX, inputY))
                    {
                        return $"Coordinates <{inputX},{inputY}> do not contain your piece! Choose again!";
                    }
                    gameInstance.DeActivateMovePieceMode();
                    return "R";
                }

                if (!gameInstance.HasGamePiece(gameInstance.GetNextMoveBy()))
                {
                    return "Not enough pieces to make a move! You can move a piece, or move grid.";
                }
                
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

    private static void MovePieceMode(TicTacTwoBrain gameInstance)
    {
        gameInstance.ActivateMovePieceMode();
        do
        {
            Visualizer.DrawBoard(gameInstance);
            Visualizer.WriteMovePieceModeInstructions();
            var input = HandleInput(gameInstance, Console.ReadLine()!);
            if (input == "R") break;
            var errorMessage = input;
            Console.WriteLine(errorMessage);
            
        } while (true);
    }
    
    private static void MoveGridMode(TicTacTwoBrain gameInstance)
    {
        gameInstance.ActivateMoveGridMode();
        
        do
        {
            Visualizer.DrawBoard(gameInstance);
            Visualizer.WriteMoveGridModeInstructions();

            var key = Console.ReadKey(true).Key;
            switch(key)
            {
                case ConsoleKey.RightArrow:
                    gameInstance.MoveGrid(EMoveGridDirection.Right);
                    break;
                case ConsoleKey.LeftArrow:
                    gameInstance.MoveGrid(EMoveGridDirection.Left);
                    break;
                case ConsoleKey.UpArrow:
                    gameInstance.MoveGrid(EMoveGridDirection.Up);
                    break;
                case ConsoleKey.DownArrow:
                    gameInstance.MoveGrid(EMoveGridDirection.Down);
                    break;
                case ConsoleKey.Enter:
                    gameInstance.DeActivateMoveGridMode();
                    return;
            }
            
        } while (true);
    }

    public static string ChooseConfigurationFromMenu()
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
    
        var configMenu = new Menu(EMenuLevel.Secondary,
            "TIC-TAC-TWO - choose game config",
            configMenuItems);

        return configMenu.Run();
    }

    private static string LoadGame()
    {
        var gameMenuItems = new List<MenuItem>();
        
        for (var i = 0; i < GameRepository.GetGameNames().Count; i++)
        {
            var returnValue = i.ToString();
            gameMenuItems.Add(new MenuItem()
            {
                Shortcut = (i + 1).ToString(),
                Title = GameRepository.GetGameNames()[i],
                MenuItemAction = () => returnValue
            });
        }
    
        var configMenu = new Menu(EMenuLevel.Secondary,
            "TIC-TAC-TWO - choose saved game",
            gameMenuItems);

        return configMenu.Run();
    }
}
