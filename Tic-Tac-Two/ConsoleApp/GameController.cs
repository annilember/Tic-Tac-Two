using ConsoleUI;
using DAL;
using Domain;
using DTO;
using GameBrain;
using MenuSystem;

namespace ConsoleApp;

public static class GameController
{
    private static IConfigRepository _configRepository = default!;
    private static IGameRepository _gameRepository = default!;

    public static void Init(IConfigRepository configRepository, IGameRepository gameRepository)
    {
        _configRepository = configRepository;
        _gameRepository = gameRepository;
    }
    
    public static string StartNewGame()
    {
        do
        {
            var chosenConfigShortcut = OptionsController.ChooseConfigurationFromMenu(
                EMenuLevel.Secondary,
                VisualizerHelper.ChooseConfigForNewGameMenuHeader
                );
        
            if (!int.TryParse(chosenConfigShortcut, out var configNo))
            {
                return chosenConfigShortcut;
            }
            var chosenConfig = _configRepository.GetConfigurationByName(
                _configRepository.GetConfigurationNames()[configNo]);
            
            
            var gameModeShortCut = OptionsController.ChooseGameModeFromMenu();
            if (!int.TryParse(gameModeShortCut, out var gameModeNo))
            {
                if (gameModeShortCut == ControllerHelper.ReturnValue)
                {
                    continue;
                }
                return gameModeShortCut;
            }
            
            var gameMode = GameMode.GetGameModes()[gameModeNo];
            
            Console.Clear();
            
            Visualizer.WriteInsertPlayerNameInstructions(EGamePiece.X, gameMode);
            var input = GetStringInputWithCancelOption();
            if (input == ControllerHelper.CancelValue) continue;
            var playerXName = input;

            Visualizer.WriteInsertPlayerNameInstructions(EGamePiece.O, gameMode);
            input = GetStringInputWithCancelOption();
            if (input == ControllerHelper.CancelValue) continue;
            var playerOName = input;
            
            var savedGame = _gameRepository.CreateGame(chosenConfig, gameMode, playerXName, playerOName);
        
            return MainGameLoop(new TicTacTwoBrain(savedGame, chosenConfig));
        } while (true);
    }
    
    public static string LoadSavedGame()
    {
        do
        {
            var chosenGameShortcut = OptionsController.ChooseGameFromMenu(
                EMenuLevel.Secondary,
                VisualizerHelper.LoadGameMenuHeader
                );

            if (chosenGameShortcut == Message.NoSavedGamesMessage)
            {
                return Message.NoSavedGamesMessage;
            }
            
            if (!int.TryParse(chosenGameShortcut, out var gameNo))
            {
                return chosenGameShortcut;
            }

            var chosenSavedGame = _gameRepository.GetSavedGameByName(
                _gameRepository.GetGameNames()[gameNo]
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

            var gameInstance = new TicTacTwoBrain(savedGame, _gameRepository.GetGameConfiguration(savedGame));
            
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
            if (gameInstance.GameOver())
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
            Visualizer.DrawBoard(gameInstance);
            if (gameInstance.GameRoundsLeft == 1)
            {
                Visualizer.DisplayFinalRoundMessage();
            }

            if (gameInstance.GetNextMoveByPlayerType() == EPlayerType.Human)
            {
                Visualizer.WriteGamePlayInstructions(gameInstance, errorMessage);
                var input = HandleInput(gameInstance, Console.ReadLine()!);
                if (input == ControllerHelper.ReturnValue) return ControllerHelper.ReturnValue;
                errorMessage = input;
            }
            else if (gameInstance.GetNextMoveByPlayerType() == EPlayerType.Ai)
            {
                Visualizer.WriteAisTurnMessage(gameInstance);
                gameInstance.MakeAiMove();
            }

        } while (true);
    }

    private static string GameOverLoop(TicTacTwoBrain gameInstance)
    {
        do
        {
            Visualizer.DrawBoard(gameInstance);
            Visualizer.DisplayGameOverMessage(gameInstance.GetGameOverMessage());
            var input = Console.ReadLine();

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
            !gameInstance.RemovePieceModeOn &&
            !gameInstance.MoveGridModeOn)
        {
            _gameRepository.SaveGame(gameInstance);
        }
        else if (input.Equals(ControllerHelper.ReturnValue, StringComparison.InvariantCultureIgnoreCase)
                 )
        {
            if (gameInstance.RemovePieceModeOn)
            {
                gameInstance.DeActivateRemovePieceMode();
            }
            return ControllerHelper.ReturnValue;
        }
        else if (input.Equals(ControllerHelper.MovePieceValue, StringComparison.InvariantCultureIgnoreCase) && 
                 gameInstance.CanMovePiece() && 
                 !gameInstance.MovePieceModeOn &&
                 !gameInstance.RemovePieceModeOn &&
                 !gameInstance.MoveGridModeOn)
        {
            RemovePieceMode(gameInstance);
        }
        else if (input.Equals(ControllerHelper.MoveGridValue, StringComparison.InvariantCultureIgnoreCase) && 
                 gameInstance.CanMoveGrid() && 
                 !gameInstance.MovePieceModeOn &&
                 !gameInstance.MoveGridModeOn &&
                 !gameInstance.RemovePieceModeOn)
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
            return Message.InvalidCoordinatesMessage;
        }
        if (!int.TryParse(inputSplit[0], out var inputX))
        {
            return Message.InvalidXCoordinateMessage;
        }
        if (!int.TryParse(inputSplit[1], out var inputY))
        {
            return Message.InvalidYCoordinateMessage;
        }
        
        return TryAndMakeAMove(gameInstance, inputX, inputY);
    }

    private static string TryAndMakeAMove(TicTacTwoBrain gameInstance, int inputX, int inputY)
    {
        try
        {
            if (gameInstance.RemovePieceModeOn)
            {
                if (!gameInstance.RemovePiece(inputX, inputY))
                {
                    return Message.GetInvalidCoordinatesError(inputX, inputY);
                }
                gameInstance.DeActivateRemovePieceMode();
                gameInstance.ActivateMovePieceMode();
                return ControllerHelper.ReturnValue;
            }
            if (!gameInstance.HasGamePiece(gameInstance.NextMoveBy))
            {
                return Message.NotEnoughPiecesMessage;
            }
            if (!gameInstance.MakeAMove(inputX, inputY))
            {
                if (gameInstance.RemovedPieceCoordinateClash(inputX, inputY))
                {
                    return Message.RemovedPieceCoordinateClashMessage;
                }
                return Message.SpaceOccupiedMessage;
            }
            gameInstance.DeActivateMovePieceMode();
        }
        catch (Exception)
        {
            return Message.CoordinatesOutOfBoundsMessage;
        }

        return "";
    }

    private static void RemovePieceMode(TicTacTwoBrain gameInstance)
    {
        gameInstance.ActivateRemovePieceMode();
        var errorMessage = "";
        do
        {
            Visualizer.DrawBoard(gameInstance);
            Visualizer.WriteRemovePieceInstructions(gameInstance, errorMessage);
            var input = HandleInput(gameInstance, Console.ReadLine()!);
            if (input == ControllerHelper.ReturnValue) break;
            errorMessage = input;
            
        } while (true);
    }
    
    private static void MoveGridMode(TicTacTwoBrain gameInstance)
    {
        gameInstance.ActivateMoveGridMode();
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
                case ConsoleKey.R:
                    gameInstance.RestoreGridState();
                    return;
                case ConsoleKey.Enter:
                    if (gameInstance.GridWasMoved())
                    {
                        gameInstance.DeActivateMoveGridMode();
                        return;
                    }
                    errorMessage = Message.GridWasNotMovedMessage;
                    break;
            }
            
        } while (true);
    }

    private static string GetStringInputWithCancelOption()
    {
        do
        {
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                continue;
            }
            return input.Equals(ControllerHelper.CancelValue, StringComparison.InvariantCultureIgnoreCase) ? 
                ControllerHelper.CancelValue : input;
        } while (true);
    }
}
