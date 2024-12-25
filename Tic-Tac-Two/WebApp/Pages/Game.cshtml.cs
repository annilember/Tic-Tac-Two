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
    
    public Task<IActionResult> OnPostAsync()
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
}
