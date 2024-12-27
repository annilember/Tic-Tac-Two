using DAL;
using Domain;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages;

public class GameModel : PageModel
{
    private readonly ILogger<GameModel> _logger;
    
    private readonly AppDbContext _context;
        
    private readonly IConfigRepository _configRepository;
        
    private readonly IGameRepository _gameRepository;

    public GameModel(ILogger<GameModel> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
        var repoController = new RepoController(context);
        _configRepository = repoController.ConfigRepository;
        _gameRepository = repoController.GameRepository;
    }
    
    [BindProperty(SupportsGet = true)]
    public string GameName { get; set; } = default!;
    
    [BindProperty(SupportsGet = true)]
    public string Password { get; set; } = default!;
    
    public SavedGame SavedGame { get; set; } = default!;
    
    public TicTacTwoBrain GameInstance { get; set; } = default!;
    
    public IActionResult OnGet()
    {
        SavedGame = _gameRepository.GetSavedGameByName(GameName);
        GameInstance = new TicTacTwoBrain(SavedGame, SavedGame.Configuration!);
        return Page();
    }
    
    [BindProperty]
    public int XCoordinate { get; set; }
    
    [BindProperty]
    public int YCoordinate { get; set; }
    
    public Task<IActionResult> OnPostMakeAMoveAsync()
    {
        SavedGame = _gameRepository.GetSavedGameByName(GameName);
        GameInstance = new TicTacTwoBrain(SavedGame, SavedGame.Configuration!);
        GameInstance.MakeAMove(XCoordinate, YCoordinate);
        SavedGame.State = GameInstance.GetGameStateJson();
        
        //TODO: check if saves correctly.
        _gameRepository.SaveGame(SavedGame);
        
        return Task.FromResult<IActionResult>(RedirectToPage(
            "./Game", 
            new { gameName = GameName, password = Password }
        ));
    }
    
    public Task<IActionResult> OnPostRemovePieceAsync()
    {
        SavedGame = _gameRepository.GetSavedGameByName(GameName);
        GameInstance = new TicTacTwoBrain(SavedGame, SavedGame.Configuration!);
        GameInstance.RemovePiece(XCoordinate, YCoordinate);
        GameInstance.DeActivateRemovePieceMode();
        GameInstance.ActivateMovePieceMode();
        SavedGame.State = GameInstance.GetGameStateJson();
        
        //TODO: check if saves correctly.
        _gameRepository.SaveGame(SavedGame);
        
        return Task.FromResult<IActionResult>(RedirectToPage(
            "./Game", 
            new { gameName = GameName, password = Password }
        ));
    }
    
    public Task<IActionResult> OnPostPlaceRemovedPieceAsync()
    {
        SavedGame = _gameRepository.GetSavedGameByName(GameName);
        GameInstance = new TicTacTwoBrain(SavedGame, SavedGame.Configuration!);
        GameInstance.MakeAMove(XCoordinate, YCoordinate);
        GameInstance.DeActivateMovePieceMode();
        SavedGame.State = GameInstance.GetGameStateJson();
        
        //TODO: check if saves correctly.
        _gameRepository.SaveGame(SavedGame);
        
        return Task.FromResult<IActionResult>(RedirectToPage(
            "./Game", 
            new { gameName = GameName, password = Password }
        ));
    }
    
    [BindProperty]
    public string ChosenMove { get; set; } = default!;
    
    public Task<IActionResult> OnPostMoveTypeChosenAsync()
    {
        SavedGame = _gameRepository.GetSavedGameByName(GameName);
        GameInstance = new TicTacTwoBrain(SavedGame, SavedGame.Configuration!);
        
        _logger.LogInformation("LOGGER!!! Chosen Move: " + ChosenMove);
        
        SavedGame.State = SetNewMoveTypeInGameState();
        
        //TODO: check if saves correctly.
        _gameRepository.SaveGame(SavedGame);
        
        return Task.FromResult<IActionResult>(RedirectToPage(
            "./Game", 
            new { gameName = GameName, password = Password }
        ));
    }
    
    [BindProperty]
    public string MoveGridDirection { get; set; } = default!;
    
    public Task<IActionResult> OnPostMoveGridAsync()
    {
        SavedGame = _gameRepository.GetSavedGameByName(GameName);
        GameInstance = new TicTacTwoBrain(SavedGame, SavedGame.Configuration!);

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
        
        SavedGame.State = GameInstance.GetGameStateJson();
        
        //TODO: check if saves correctly.
        _gameRepository.SaveGame(SavedGame);
        
        return Task.FromResult<IActionResult>(RedirectToPage(
            "./Game", 
            new { gameName = GameName, password = Password }
        ));
    }
    
    public Task<IActionResult> OnPostSaveNewGridPositionAsync()
    {
        SavedGame = _gameRepository.GetSavedGameByName(GameName);
        GameInstance = new TicTacTwoBrain(SavedGame, SavedGame.Configuration!);

        if (!GameInstance.GridWasMoved())
        {
            TempData["ErrorMessage"] = Message.GridWasNotMovedMessage;
        }
        else
        {
            GameInstance.DeActivateMoveGridMode();
            SavedGame.State = GameInstance.GetGameStateJson();
        
            //TODO: check if saves correctly.
            _gameRepository.SaveGame(SavedGame);
        }
        
        return Task.FromResult<IActionResult>(RedirectToPage(
            "./Game", 
            new { gameName = GameName, password = Password }
        ));
    }

    private string SetNewMoveTypeInGameState()
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
        return GameInstance.GetGameStateJson();
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
}
