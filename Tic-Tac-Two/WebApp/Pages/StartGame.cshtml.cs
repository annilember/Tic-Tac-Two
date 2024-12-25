using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class StartGameModel : PageModel
{
    private readonly ILogger<StartGameModel> _logger;
    
    private readonly AppDbContext _context;
        
    private readonly IConfigRepository _configRepository;
        
    private readonly IGameRepository _gameRepository;

    public StartGameModel(ILogger<StartGameModel> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
        var repoController = new RepoController(context);
        _configRepository = repoController.ConfigRepository;
        _gameRepository = repoController.GameRepository;
    }
    
    [BindProperty]
    public string GameName { get; set; } = default!;
    
    public string PlayerXName { get; set; } = default!;
    
    public string PlayerOName { get; set; } = default!;

    public IActionResult OnGet(string gameName)
    {
        var savedGame = _gameRepository.GetSavedGameByName(gameName);
        GameName = gameName;
        PlayerXName = savedGame.PlayerXName;
        PlayerOName = savedGame.PlayerOName;
        return Page();
    }
    
    public Task<IActionResult> OnPostPlayerXAsync()
    {
        if (!ModelState.IsValid)
        {
            foreach (var error in ModelState)
            {
                _logger.LogError($"{error.Key}: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
            }
            return Task.FromResult<IActionResult>(Page());
        }
        
        var savedGame = _gameRepository.GetSavedGameByName(GameName);
        _logger.LogInformation($"POST FROM START GAME - Game Name: {GameName}, Password: {savedGame.PlayerXPassword}");
        
        return Task.FromResult<IActionResult>(RedirectToPage(
            "./Game", 
            new { gameName = GameName, password = savedGame.PlayerXPassword }
        ));
    }
    
    public Task<IActionResult> OnPostPlayerOAsync()
    {
        if (!ModelState.IsValid)
        {
            foreach (var error in ModelState)
            {
                _logger.LogError($"{error.Key}: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
            }
            return Task.FromResult<IActionResult>(Page());
        }
        
        var savedGame = _gameRepository.GetSavedGameByName(GameName);
        _logger.LogInformation($"POST FROM START GAME - Game Name: {GameName}, Password: {savedGame.PlayerOPassword}");
        
        return Task.FromResult<IActionResult>(RedirectToPage(
            "./Game", 
            new { gameName = GameName, password = savedGame.PlayerOPassword }
        ));
    }
}