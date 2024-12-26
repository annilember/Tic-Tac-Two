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
    
    [BindProperty]
    public string ChosenMove { get; set; } = default!;
    
    public Task<IActionResult> OnPostMoveTypeChosenAsync()
    {
        SavedGame = _gameRepository.GetSavedGameByName(GameName);
        GameInstance = new TicTacTwoBrain(SavedGame, SavedGame.Configuration!);
        
        _logger.LogInformation("LOGGER!!! Chosen Move: " + ChosenMove);
        
        SavedGame.State = SetNewMoveTypeInGameState(ChosenMove, GameInstance);
        
        //TODO: check if saves correctly.
        _gameRepository.SaveGame(SavedGame);
        
        return Task.FromResult<IActionResult>(RedirectToPage(
            "./Game", 
            new { gameName = GameName, password = Password }
        ));
    }

    private string SetNewMoveTypeInGameState(string chosenMove, TicTacTwoBrain gameInstance)
    {
        if (chosenMove == EChosenMove.PlacePiece.ToString())
        {
            if (gameInstance.RemovePieceModeOn)
            {
                gameInstance.DeActivateRemovePieceMode();
            }
            if (gameInstance.MoveGridModeOn)
            {
                gameInstance.RestoreGridState();
            }
        }
        else if (chosenMove == EChosenMove.MovePiece.ToString())
        {
            if (gameInstance.MoveGridModeOn)
            {
                gameInstance.RestoreGridState();
            }
            gameInstance.ActivateRemovePieceMode();
        }
        else if (chosenMove == EChosenMove.MoveGrid.ToString())
        {
            if (gameInstance.RemovePieceModeOn)
            {
                gameInstance.DeActivateRemovePieceMode();
            }
            gameInstance.ActivateMoveGridMode();
        }
        return gameInstance.GetGameStateJson();
    }

    public string GetButtonStyle(int x, int y)
    {
        var style = "btn";
        if (GameInstance.RemovePieceModeOn && GameInstance.GameBoard[x][y] == GameInstance.GetNextMoveBy())
        {
            style += "-outline";
        }
        style += GameInstance.GameGrid[x][y] ? "-warning" : "-light";
        return style;
    }
}
