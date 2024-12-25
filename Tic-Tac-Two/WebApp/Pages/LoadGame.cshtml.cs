using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages;

public class LoadGameModel : PageModel
{
    private readonly AppDbContext _context;
        
    private readonly IConfigRepository _configRepository;
        
    private readonly IGameRepository _gameRepository;

    public LoadGameModel(AppDbContext context)
    {
        var repoController = new RepoController(context);
        _configRepository = repoController.ConfigRepository;
        _gameRepository = repoController.GameRepository;
        _context = context;
    }

    public IList<SavedGame> SavedGame { get;set; } = default!;

    public async Task OnGetAsync()
    {
        SavedGame = await _context.SavedGames
            .Include(s => s.Configuration).ToListAsync();
    }
}