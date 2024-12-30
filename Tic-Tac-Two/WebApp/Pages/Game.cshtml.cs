using DAL;
using Domain;
using DTO;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class GameModel : PageModel
{
    private readonly ILogger<GameModel> _logger;

    private readonly IGameRepository _gameRepository;

    public GameModel(ILogger<GameModel> logger, AppDbContext context)
    {
        _logger = logger;
        var repoController = new RepoController(context);
        _gameRepository = repoController.GameRepository;
    }

    [BindProperty(SupportsGet = true)] public string GameName { get; set; } = default!;

    [BindProperty(SupportsGet = true)] public string Password { get; set; } = default!;

    public SavedGame SavedGame { get; set; } = default!;

    public TicTacTwoBrain GameInstance { get; set; } = default!;

    [BindProperty] public int XCoordinate { get; set; }

    [BindProperty] public int YCoordinate { get; set; }

    [BindProperty] public string ChosenMove { get; set; } = default!;
    
    [BindProperty] public string MoveGridDirection { get; set; } = default!;

    public IActionResult OnGet()
    {
        SavedGame = _gameRepository.GetSavedGameByName(GameName);
        GameInstance = new TicTacTwoBrain(SavedGame, SavedGame.Configuration!);
        
        if (!GameInstance.MoveGridModeOn && GameInstance.GameOver())
        {
            return RedirectToPage(
                "./GameOver",
                new { gameName = GameName, password = Password }
            );
        }

        return Page();
    }
    
    private enum GameAction
    {
        MakeAMove,
        RemovePiece,
        PlaceRemovedPiece,
        MoveTypeChosen,
        MoveGrid,
        SaveNewGridPosition,
        AiMove
    }

    public Task<IActionResult> OnPostAsync(string handler)
    {
        SavedGame = _gameRepository.GetSavedGameByName(GameName);
        GameInstance = new TicTacTwoBrain(SavedGame, SavedGame.Configuration!);
        var action = Enum.Parse<GameAction>(handler);
        var errorMessage = DoOnPostAction(action);
        
        _gameRepository.SaveGame(GameInstance);
        
        if (errorMessage != null)
        {
            TempData["ErrorMessage"] = errorMessage;
        }
        
        if (GameInstance.GameOver() && !GameInstance.MoveGridModeOn && !GameInstance.MovePieceModeOn)
        {
            return Task.FromResult<IActionResult>(RedirectToPage(
                "./GameOver",
                new { gameName = GameName, password = Password }
            ));
        }

        return Task.FromResult<IActionResult>(RedirectToPage(
            "./Game",
            new { gameName = GameName, password = Password }
        ));
        
    }

    private string? DoOnPostAction(GameAction action)
    {
        switch (action)
        {
            case GameAction.MakeAMove:
                GameInstance.MakeAMove(XCoordinate, YCoordinate);
                break;
            case GameAction.RemovePiece:
                GameInstance.RemovePiece(XCoordinate, YCoordinate);
                GameInstance.DeActivateRemovePieceMode();
                GameInstance.ActivateMovePieceMode();
                break;
            case GameAction.PlaceRemovedPiece:
                GameInstance.MakeAMove(XCoordinate, YCoordinate);
                GameInstance.DeActivateMovePieceMode();
                break;
            case GameAction.MoveTypeChosen:
                SetNewMoveTypeInGameState();
                break;
            case GameAction.MoveGrid:
                HandleMoveGrid();
                break;
            case GameAction.SaveNewGridPosition:
                if (!GameInstance.GridWasMoved())
                {
                    return Message.GridWasNotMovedMessage;
                }
                GameInstance.DeActivateMoveGridMode();
                break;
            case GameAction.AiMove:
                GameInstance.MakeAiMove();
                break;
        }
        return null;
    }
    
    private void HandleMoveGrid()
    {
        if (MoveGridDirection == EMoveGridDirection.Right.ToString())
        {
            GameInstance.MoveGrid(EMoveGridDirection.Right);
        }
        else if (MoveGridDirection == EMoveGridDirection.Left.ToString())
        {
            GameInstance.MoveGrid(EMoveGridDirection.Left);
        }
        else if (MoveGridDirection == EMoveGridDirection.Up.ToString())
        {
            GameInstance.MoveGrid(EMoveGridDirection.Up);
        }
        else if (MoveGridDirection == EMoveGridDirection.Down.ToString())
        {
            GameInstance.MoveGrid(EMoveGridDirection.Down);
        }
    }

    private void SetNewMoveTypeInGameState()
    {
        if (ChosenMove == EChosenMove.PlacePiece.ToString())
        {
            if (GameInstance.RemovePieceModeOn)
            {
                GameInstance.DeActivateRemovePieceMode();
            }

            if (GameInstance.MoveGridModeOn)
            {
                GameInstance.RestoreGridState();
            }
        }
        else if (ChosenMove == EChosenMove.RemovePiece.ToString())
        {
            if (GameInstance.MoveGridModeOn)
            {
                GameInstance.RestoreGridState();
            }

            GameInstance.ActivateRemovePieceMode();
        }
        else if (ChosenMove == EChosenMove.MoveGrid.ToString())
        {
            if (GameInstance.RemovePieceModeOn)
            {
                GameInstance.DeActivateRemovePieceMode();
            }

            GameInstance.ActivateMoveGridMode();
        }
    }

    public string StyleOccupiedSpot(int x, int y)
    {
        var style = "btn";
        if (GameInstance.RemovePieceModeOn && GameInstance.GameBoard[x][y] == GameInstance.GetNextMoveBy())
        {
            style += "-outline";
        }

        return style + StyleSpot(x, y);
    }

    public string StyleFreeSpot(int x, int y)
    {
        return "btn-outline" + StyleSpot(x, y);
    }

    private string StyleSpot(int x, int y)
    {
        var style = "";
        if (GameInstance.GameGrid[x][y])
        {
            style += "-warning";
        }
        else if (GameInstance.MoveGridModeOn && GameInstance.GridMovingArea[x][y])
        {
            style += "-info";
        }
        else
        {
            style += "-light";
        }

        return style;
    }

    public bool YourTurn()
    {
        if (AiTurn())
        {
            return false;
        }
        if (Password == SavedGame.PlayerXPassword && GameInstance.GetNextMoveBy() == EGamePiece.X)
        {
            return true;
        }
        if (Password == SavedGame.PlayerOPassword && GameInstance.GetNextMoveBy() == EGamePiece.O)
        {
            return true;
        }
        return false;
    }

    public string YourName()
    {
        if (Password == SavedGame.PlayerXPassword)
        {
            return SavedGame.PlayerXName;
        }
        if (Password == SavedGame.PlayerOPassword)
        {
            return SavedGame.PlayerOName;
        }
        return Message.UnknownPlayerName;
    }

    public bool AiTurn()
    {
        var gameMode = GameMode.GetMode(SavedGame.ModeName);
        var playerType = GameMode.GetPlayerType(gameMode, GameInstance.GetNextMoveBy());
        return playerType == EPlayerType.Ai;
    }
}
