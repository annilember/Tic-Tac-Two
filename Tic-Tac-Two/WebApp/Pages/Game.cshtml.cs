using DAL;
using Domain;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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

    public SavedGame SavedGame { get; set; } = default!;
    
    public string GameName { get; set; } = default!;
    public string Password { get; set; } = default!;
    
    public TicTacTwoBrain GameInstance { get; set; } = default!;
    
    public IActionResult OnGet(string gameName, string password)
    {
        GameName = gameName;
        Password = password;
        SavedGame = _gameRepository.GetSavedGameByName(gameName);
        _logger.LogInformation($"GET FROM GAME - Game Name: {gameName}, Password: {password}, Game State: {SavedGame.State}");
        
        // TODO: make sure this config finding works and isn't null.
        GameInstance = new TicTacTwoBrain(SavedGame, SavedGame.Configuration!);
        return Page();
    }
    
    public Task<IActionResult> OnPostAsync()
    {
        return Task.FromResult<IActionResult>(RedirectToPage(
            "./Game", 
            new { gameName = GameName, password = Password }
        ));
    }
}