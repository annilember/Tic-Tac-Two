using ConsoleUI;
using DAL;
using Domain;
using GameBrain;
using MenuSystem;

namespace ConsoleApp;

public static class GameController
{
    private static readonly IConfigRepository ConfigRepository = new ConfigRepositoryDb();
    private static readonly IGameRepository GameRepository = new GameRepositoryDb();
    
    public static string StartNewGame()
    {
        do
        {
            var chosenConfigShortcut = ChooseConfigurationFromMenu(
                EMenuLevel.Secondary,
                ControllerHelper.ChooseConfigForNewGameMenuHeader
                );
        
            if (!int.TryParse(chosenConfigShortcut, out var configNo))
            {
                return chosenConfigShortcut;
            }
            var chosenConfig = ConfigRepository.GetConfigurationByName(
                ConfigRepository.GetConfigurationNames()[configNo]);
        
            var input = "";
            Console.Clear();
            
            Visualizer.WriteInsertPlayerNameInstructions(EGamePiece.X);
            input = GetStringInputWithReturnOption();
            if (input == ControllerHelper.CancelValue) continue;
            var playerXName = input;

            Visualizer.WriteInsertPlayerNameInstructions(EGamePiece.O);
            input = GetStringInputWithReturnOption();
            if (input == ControllerHelper.CancelValue) continue;
            var playerOName = input;
        
            return MainGameLoop(new TicTacTwoBrain(chosenConfig, playerXName, playerOName));
        } while (true);
    }
    
    public static string LoadSavedGame()
    {
        do
        {
            var chosenGameShortcut = ChooseGameFromMenu(
                EMenuLevel.Secondary,
                ControllerHelper.LoadGameMenuHeader
                );

            if (chosenGameShortcut == ControllerHelper.NoSavedGamesMessage)
            {
                return ControllerHelper.NoSavedGamesMessage;
            }
            
            if (!int.TryParse(chosenGameShortcut, out var gameNo))
            {
                return chosenGameShortcut;
            }

            var chosenSavedGame = GameRepository.GetSavedGameByName(
                GameRepository.GetGameNames()[gameNo]
                );

            var returnValue = StartLoadedGame(chosenSavedGame);

            if (returnValue == ControllerHelper.CancelValue) continue;
            return returnValue;
        } while (true);

    }

    private static string StartLoadedGame(SavedGame savedGame)
    {
        do
        {
            Visualizer.WriteLoadGameModeInstructions();
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                continue;
            }
            if (input.Equals(ControllerHelper.CancelValue, StringComparison.InvariantCultureIgnoreCase))
            {
                return ControllerHelper.CancelValue;
            }

            var gameInstance = new TicTacTwoBrain(savedGame, GameRepository.GetGameConfiguration(savedGame));
            
            if (input.Equals(ControllerHelper.ResetGameValue, StringComparison.InvariantCultureIgnoreCase))
            {
                gameInstance.ResetGame();
                return MainGameLoop(gameInstance);
            }
            if (input.Equals(ControllerHelper.LoadGameValue, StringComparison.InvariantCultureIgnoreCase))
            {
                return MainGameLoop(gameInstance);
            }
                
        } while (true);
    }

    private static string MainGameLoop(TicTacTwoBrain gameInstance)
    {
        var errorMessage = "";

        do
        {
            Visualizer.DrawBoard(gameInstance);
            if (gameInstance.GetRoundsLeft() == 1)
            {
                Visualizer.DisplayFinalRoundMessage();
            }
            
            Visualizer.WriteGamePlayInstructions(gameInstance, errorMessage);
            var input = HandleInput(gameInstance, Console.ReadLine()!);
            if (input == ControllerHelper.ReturnValue) return ControllerHelper.ReturnValue;
            errorMessage = input;

            if (!string.IsNullOrEmpty(gameInstance.GetWinnerName()) || gameInstance.IsGameOverAnyway())
            {
                var returnValue = GameOverLoop(gameInstance);
                switch (returnValue)
                {
                    case ControllerHelper.ReturnValue:
                        return ControllerHelper.ReturnValue;
                    case ControllerHelper.ResetGameValue:
                        gameInstance.ResetGame();
                        break;
                }
            }

        } while (true);
    }

    private static string GameOverLoop(TicTacTwoBrain gameInstance)
    {
        var input = "";
        do
        {
            input = "";
            var message = "";
            Visualizer.DrawBoard(gameInstance);
            
            if (gameInstance.CheckForDraw())
            {
                message = ControllerHelper.GameOverDrawMessage;
            } 
            else if (!string.IsNullOrEmpty(gameInstance.GetWinnerName()))
            {
                message = $"The winner is {gameInstance.GetWinnerName()}! Whoop, whoop!";
            }
            else if (gameInstance.IsGameOverAnyway())
            {
                message = ControllerHelper.GameOverNoMoreRoundsMessage;
            }
            
            Visualizer.DisplayGameOverMessage(message);
            input = Console.ReadLine();

            if (input!.Equals(ControllerHelper.ReturnValue, StringComparison.InvariantCultureIgnoreCase))
            {
                return ControllerHelper.ReturnValue;
            }
            if (input.Equals(ControllerHelper.ResetGameValue, StringComparison.InvariantCultureIgnoreCase))
            {
                return ControllerHelper.ResetGameValue;
            }
            
        } while (true);
    }
    
    private static string HandleInput(TicTacTwoBrain gameInstance, string input)
    {
        var errorMessage = "";
        
        if (input.Equals(ControllerHelper.SaveGameValue, StringComparison.InvariantCultureIgnoreCase) && 
            !gameInstance.MovePieceModeOn &&
            !gameInstance.MoveGridModeOn)
        {
            GameRepository.SaveGame(gameInstance, "");
        }
        else if (input.Equals(ControllerHelper.ReturnValue, StringComparison.InvariantCultureIgnoreCase) &&
                 !gameInstance.MovePieceModeOn &&
                 !gameInstance.MoveGridModeOn)
        {
            return ControllerHelper.ReturnValue;
        }
        else if (input.Equals(ControllerHelper.MovePieceValue, StringComparison.InvariantCultureIgnoreCase) && 
                 gameInstance.CanMovePiece() && 
                 !gameInstance.MovePieceModeOn &&
                 !gameInstance.MoveGridModeOn)
        {
            MovePieceMode(gameInstance);
        }
        else if (input.Equals(ControllerHelper.MoveGridValue, StringComparison.InvariantCultureIgnoreCase) && 
                 gameInstance.CanMoveGrid() && 
                 !gameInstance.MoveGridModeOn &&
                 !gameInstance.MovePieceModeOn)
        {
            MoveGridMode(gameInstance);
        }
        else
        {
            errorMessage = HandleCoordinates(gameInstance, input);
        }

        return errorMessage;
    }

    private static string HandleCoordinates(TicTacTwoBrain gameInstance, string input)
    {
        var inputSplit = input.Split(',');
        if (inputSplit.Length != 2)
        {
            return ControllerHelper.InvalidCoordinatesMessage;
        }
        if (!int.TryParse(inputSplit[0], out var inputX))
        {
            return ControllerHelper.InvalidXCoordinateMessage;
        }
        if (!int.TryParse(inputSplit[1], out var inputY))
        {
            return ControllerHelper.InvalidYCoordinateMessage;
        }
        
        return TryAndMakeAMove(gameInstance, inputX, inputY);
    }

    private static string TryAndMakeAMove(TicTacTwoBrain gameInstance, int inputX, int inputY)
    {
        try
        {
            if (gameInstance.MovePieceModeOn)
            {
                if (!gameInstance.RemovePiece(inputX, inputY))
                {
                    return $"Coordinates <{inputX},{inputY}> do not contain your piece! Choose again!";
                }
                gameInstance.DeActivateMovePieceMode();
                return ControllerHelper.ReturnValue;
            }
            if (!gameInstance.HasGamePiece(gameInstance.GetNextMoveBy()))
            {
                return ControllerHelper.NotEnoughPiecesMessage;
            }
            if (!gameInstance.MakeAMove(inputX, inputY))
            {
                return ControllerHelper.SpaceOccupiedMessage;
            }
        }
        catch (Exception)
        {
            return ControllerHelper.CoordinatesOutOfBoundsMessage;
        }

        return "";
    }

    private static void MovePieceMode(TicTacTwoBrain gameInstance)
    {
        gameInstance.ActivateMovePieceMode();
        var errorMessage = "";
        do
        {
            Visualizer.DrawBoard(gameInstance);
            Visualizer.WriteMovePieceModeRemoveInstructions(gameInstance, errorMessage);
            var input = HandleInput(gameInstance, Console.ReadLine()!);
            if (input == ControllerHelper.ReturnValue) break;
            errorMessage = input;
            
        } while (true);
        // // gameInstance.DeActivateMovePieceMode();
        // do
        // {
        //     Visualizer.DrawBoard(gameInstance);
        //     Visualizer.WriteMovePieceModePlaceInstructions(gameInstance, errorMessage);
        //     var input = HandleInput(gameInstance, Console.ReadLine()!);
        //     if (input == ControllerHelper.ReturnValue) break;
        //     errorMessage = input;
        //     
        // } while (true);
    }
    
    private static void MoveGridMode(TicTacTwoBrain gameInstance)
    {
        gameInstance.ActivateMoveGridMode();
        gameInstance.SaveCurrentGridState();
        var errorMessage = "";
        
        do
        {
            Visualizer.DrawBoard(gameInstance);
            Visualizer.WriteMoveGridModeInstructions(gameInstance, errorMessage);

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
                    if (gameInstance.GridWasMoved())
                    {
                        gameInstance.DeActivateMoveGridMode();
                        return;
                    }
                    errorMessage = ControllerHelper.GridWasNotMovedMessage;
                    break;
            }
            
        } while (true);
    }

    public static string ChooseConfigurationFromMenu(EMenuLevel menuLevel, string menuHeader)
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
    
        var configMenu = new Menu(menuLevel,
            menuHeader,
            configMenuItems);

        return configMenu.Run();
    }

    public static string ChooseGameFromMenu(EMenuLevel menuLevel, string menuHeader)
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

        if (gameMenuItems.Count == 0)
        {
            return ControllerHelper.NoSavedGamesMessage;
        }
    
        var configMenu = new Menu(menuLevel,
            menuHeader,
            gameMenuItems);

        return configMenu.Run();
    }

    private static string GetStringInputWithReturnOption()
    {
        var input = "";
        do
        {
            input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                continue;
            }
            return input.Equals(ControllerHelper.CancelValue, StringComparison.InvariantCultureIgnoreCase) ? 
                ControllerHelper.CancelValue : input;
        } while (true);
    }
}
