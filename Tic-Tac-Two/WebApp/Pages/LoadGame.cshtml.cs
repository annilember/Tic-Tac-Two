using System.ComponentModel.DataAnnotations;
using DAL;
using Domain;
using GameBrain;
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

    [BindProperty(SupportsGet = true)]
    public IList<SavedGame> HumanVsHumanGames { get;set; } = default!;
    
    [BindProperty(SupportsGet = true)]
    public IList<SavedGame> HumanVsAiGames { get;set; } = default!;
    
    [BindProperty(SupportsGet = true)]
    public IList<SavedGame> AiVsAiGames { get;set; } = default!;

    public async Task OnGetAsync()
    {
        HumanVsHumanGames = await _context.SavedGames
            .Where(savedGame => savedGame.ModeName == GameMode.GetModeName(EGameMode.HumanVsHuman.ToString()))
            .Include(s => s.Configuration).ToListAsync();
        
        HumanVsAiGames = await _context.SavedGames
            .Where(savedGame => savedGame.ModeName == GameMode.GetModeName(EGameMode.HumanVsAi.ToString()) || 
                                savedGame.ModeName == GameMode.GetModeName(EGameMode.AiVsHuman.ToString()))
            .Include(s => s.Configuration).ToListAsync();
  
        AiVsAiGames = await _context.SavedGames
            .Where(savedGame => savedGame.ModeName == GameMode.GetModeName(EGameMode.AiVsAi.ToString()))
            .Include(s => s.Configuration).ToListAsync();
    }
    
    [BindProperty(SupportsGet = true)]
    public string GameName { get; set; } = default!;
    
    [BindProperty]
    public string Password { get; set; } = default!;
    
    public Task<IActionResult> OnPostJoinGameAsync()
    {
        return Task.FromResult<IActionResult>(RedirectToPage(
            "./Game", 
            new { gameName = GameName, password = Password }
        ));
    }
    
    [BindProperty]
    [Required(ErrorMessage = "New game name is required")]
    public string NewGameName { get; set; } = default!;
    
    public Task<IActionResult> OnPostRenameGameAsync()
    {
        if (_gameRepository.GameExists(NewGameName))
        {
            TempData["ErrorMessage"] = Message.GameNameAlreadyInUseMessage;
            TempData["NewGameName"] = NewGameName;
            return Task.FromResult<IActionResult>(RedirectToPage(
                "./LoadGame", 
                new { gameName = GameName }
            ));
        }

        var savedGame = _gameRepository.GetSavedGameByName(GameName);
        _gameRepository.RenameGame(savedGame, NewGameName);
        TempData["Message"] = Message.GameRenamedMessage;
        return Task.FromResult<IActionResult>(RedirectToPage());
    }
    
    public Task<IActionResult> OnPostDeleteGameAsync()
    {
        _gameRepository.DeleteGame(GameName);
        TempData["Message"] = Message.GameDeletedMessage;
        return Task.FromResult<IActionResult>(RedirectToPage());
    }
    
    [BindProperty]
    public string GameModeName { get; set; } = default!;
    public Task<IActionResult> OnPostNewGameAsync()
    {
        return Task.FromResult<IActionResult>(RedirectToPage(
            "./NewGame", 
            new { gameModeName = GameModeName }
        ));
    }
}
