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
    
    public SavedGame SavedGame { get; set; } = default!;
    
    public string GameName { get; set; } = default!;
    
    public string PlayerXName { get; set; } = default!;
    
    public string PlayerOName { get; set; } = default!;
    
    public string PlayerXPassword { get; set; } = default!;
    
    public string PlayerOPassword { get; set; } = default!;
    
    // [BindProperty]
    public string Password { get; set; } = default!;


    public IActionResult OnGet(string gameName)
    {
        SavedGame = _gameRepository.GetSavedGameByName(gameName);
        GameName = gameName;
        PlayerXName = SavedGame.PlayerXName;
        PlayerOName = SavedGame.PlayerOName;
        return Page();
    }
    
    public Task<IActionResult> OnPostAsync(string gameName, string password)
    {
        _logger.LogInformation($"POST FROM START GAME - Game Name: {gameName}, Password: {password}");
        GameName = gameName;
        Password = password;
        
        if (!ModelState.IsValid)
        {
            foreach (var error in ModelState)
            {
                _logger.LogError($"{error.Key}: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
            }
            return Task.FromResult<IActionResult>(Page());
        }
        
        return Task.FromResult<IActionResult>(RedirectToPage(
            "./Game", 
            new { gameName = GameName, password = Password }
        ));
    }
}