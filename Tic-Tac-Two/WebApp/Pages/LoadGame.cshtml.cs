using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages;

public class LoadGameModel : PageModel
{
    private readonly ILogger<LoadGameModel> _logger;
    
    private readonly AppDbContext _context;
        
    private readonly IConfigRepository _configRepository;
        
    private readonly IGameRepository _gameRepository;

    public LoadGameModel(ILogger<LoadGameModel> logger, AppDbContext context)
    {
        _logger = logger;
        var repoController = new RepoController(context);
        _configRepository = repoController.ConfigRepository;
        _gameRepository = repoController.GameRepository;
        _context = context;
    }

    public IList<SavedGame> SavedGames { get;set; } = default!;

    public async Task OnGetAsync()
    {
        SavedGames = await _context.SavedGames
            .Where(savedGame => savedGame.ModeName == "Human vs Human")
            .Include(s => s.Configuration).ToListAsync();
    }
    
    [BindProperty]
    public string GameName { get; set; } = default!;
    
    [BindProperty]
    public string Password { get; set; } = default!;
    
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
        
        _logger.LogInformation($"POST FROM LOAD GAME - Game Name: {GameName}, Password: {Password}");
        
        return Task.FromResult<IActionResult>(RedirectToPage(
            "./Game", 
            new { gameName = GameName, password = Password }
        ));
    }
}