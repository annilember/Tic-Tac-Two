using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class ConfigModel : PageModel
{
    private readonly ILogger<ConfigModel> _logger;
    
    private readonly AppDbContext _context;
        
    private readonly IConfigRepository _configRepository;
        
    private readonly IGameRepository _gameRepository;

    public ConfigModel(ILogger<ConfigModel> logger, AppDbContext context)
    {
        _logger = logger;
        var repoController = new RepoController(context);
        _configRepository = repoController.ConfigRepository;
        _gameRepository = repoController.GameRepository;
        _context = context;
    }
    
    [BindProperty(SupportsGet = true)]
    public string? ConfigName { get; set; }
    
    [BindProperty]
    public GameConfiguration GameConfiguration { get; set; } = default!;
    
    public IActionResult OnGet()
    {
        return Page();
    }
    
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Configurations.Add(GameConfiguration);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}


