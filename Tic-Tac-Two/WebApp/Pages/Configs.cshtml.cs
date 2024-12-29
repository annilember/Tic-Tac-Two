using DAL;
using Domain;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages;

public class ConfigsModel : PageModel
{
    private readonly ILogger<ConfigsModel> _logger;
    
    private readonly AppDbContext _context;
        
    private readonly IConfigRepository _configRepository;
        
    private readonly IGameRepository _gameRepository;

    public ConfigsModel(ILogger<ConfigsModel> logger, AppDbContext context)
    {
        _logger = logger;
        var repoController = new RepoController(context);
        _configRepository = repoController.ConfigRepository;
        _gameRepository = repoController.GameRepository;
        _context = context;
    }

    public IList<GameConfiguration> GameConfiguration { get;set; } = default!;
    
    [BindProperty]
    public string ConfigName { get; set; } = default!;

    public async Task OnGetAsync()
    {
        _configRepository.CheckAndCreateInitialConfig();
        GameConfiguration = await _context.Configurations.ToListAsync();
    }
    
    public Task<IActionResult> OnPostNewConfigAsync()
    {
        return Task.FromResult<IActionResult>(RedirectToPage("./Config"));
    }
    
    public Task<IActionResult> OnPostChangeConfigAsync()
    {
        return Task.FromResult<IActionResult>(RedirectToPage(
            "./Config", 
            new { configName = ConfigName }
        ));
    }
    
    public Task<IActionResult> OnPostDeleteConfigAsync()
    {
        _configRepository.DeleteConfiguration(_configRepository.GetConfigurationByName(ConfigName));
        TempData["Message"] = Message.ConfigDeletedMessage;
        return Task.FromResult<IActionResult>(RedirectToPage());
    }
}