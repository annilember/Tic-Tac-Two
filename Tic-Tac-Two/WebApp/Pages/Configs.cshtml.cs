using DAL;
using Domain;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class ConfigsModel : PageModel
{
    private readonly ILogger<ConfigsModel> _logger;
        
    private readonly IConfigRepository _configRepository;

    public ConfigsModel(ILogger<ConfigsModel> logger, AppDbContext context)
    {
        _logger = logger;
        var repoController = new RepoController(context);
        _configRepository = repoController.ConfigRepository;
    }

    public IList<GameConfiguration> GameConfigurations { get;set; } = default!;
    
    [BindProperty]
    public string ConfigName { get; set; } = default!;

    public Task OnGetAsync()
    {
        GameConfigurations = _configRepository.GetConfigurations();
        return Task.CompletedTask;
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