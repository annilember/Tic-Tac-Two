using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class StartGameModel : PageModel
{
    private readonly ILogger<StartGameModel> _logger;
        
    private readonly IGameRepository _gameRepository;

    public StartGameModel(ILogger<StartGameModel> logger, AppDbContext context)
    {
        _logger = logger;
        var repoController = new RepoController(context);
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
        
        return Task.FromResult<IActionResult>(RedirectToPage(
            "./Game", 
            new { gameName = GameName, password = savedGame.PlayerOPassword }
        ));
    }
}