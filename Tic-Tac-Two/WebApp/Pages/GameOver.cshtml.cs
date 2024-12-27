using DAL;
using Domain;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class GameOverModel : PageModel
{
    private readonly ILogger<GameOverModel> _logger;
    
    private readonly AppDbContext _context;
        
    private readonly IConfigRepository _configRepository;
        
    private readonly IGameRepository _gameRepository;

    public GameOverModel(ILogger<GameOverModel> logger, AppDbContext context)
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
    
    public Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            foreach (var error in ModelState)
            {
                _logger.LogError($"{error.Key}: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
            }
            return Task.FromResult<IActionResult>(Page());
        }
        
        SavedGame = _gameRepository.GetSavedGameByName(GameName);
        GameInstance = new TicTacTwoBrain(SavedGame, SavedGame.Configuration!);
        GameInstance.ResetGame();
        SavedGame.State = GameInstance.GetGameStateJson();
        
        //TODO: check if saves correctly.
        _gameRepository.SaveGame(SavedGame);
        
        return Task.FromResult<IActionResult>(RedirectToPage(
            "./Game", 
            new { gameName = GameName, password = Password }
        ));
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
}
