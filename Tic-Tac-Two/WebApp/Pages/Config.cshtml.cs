using System.Text.Json;
using DAL;
using Domain;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Build.Framework;

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

    [BindProperty(SupportsGet = true)] public string? ConfigName { get; set; }

    [Required] [BindProperty] public GameConfiguration GameConfiguration { get; set; } = default!;

    public IActionResult OnGet()
    {
        GameConfiguration = new GameConfiguration();
        if (ConfigName != null) GameConfiguration = _configRepository.GetConfigurationByName(ConfigName);
        return Page();
    }

    public Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            foreach (var error in ModelState)
            {
                _logger.LogError(
                    $"{error.Key}: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
            }
            return Task.FromResult<IActionResult>(Page());
        }
        
        var errorMessages = GetErrorMessages(GameConfiguration, ConfigName);
        if (errorMessages.Length > 0)
        {
            TempData["ErrorMessages"] = JsonSerializer.Serialize(errorMessages);
            
            // siin peab andmed kaasa andma kuidagi nii, et ei nulliks ära
            return Task.FromResult<IActionResult>(RedirectToPage(
                "./Config", 
                new { configName = ConfigName }
                ));
        }

        if (ConfigName == null)
        {
            _configRepository.AddNewConfiguration(GameConfiguration);
            TempData["Message"] = Message.NewConfigCreatedMessage;
        }
        else
        {
            _configRepository.SaveConfigurationChanges(GameConfiguration, ConfigName);
            TempData["Message"] = Message.ConfigUpdatedMessage;
        }
        
        return Task.FromResult<IActionResult>(RedirectToPage("./Configs"));
    }
    
    private string[] GetErrorMessages(GameConfiguration config, string? configName)
    {
        var properties = GameConfigurationHelper.GetConfigPropertyInfo(config);
        var errorMessages = new List<string>();
        
        if (_configRepository.ConfigurationExists(GameConfiguration.Name) && GameConfiguration.Name != configName)
        {
            errorMessages.Add(Message.ConfigNameAlreadyInUseMessage);
        }
        
        foreach (var property in properties)
        {
            if (property.Name.Equals("Id", StringComparison.InvariantCultureIgnoreCase) ||
                property.Name.Equals("Name", StringComparison.InvariantCultureIgnoreCase))
            {
                continue;
            }
            
            var value = property.GetValue(config);
            if (value != null && int.TryParse(value.ToString(), out var intValue))
            {
                var error = GameConfigurationHelper.CheckBoundsAndGetErrorMessage(config, property.Name, intValue);
                if (!string.IsNullOrEmpty(error))
                {
                    errorMessages.Add(error);
                }
            }
            else if (value != null)
            {
                errorMessages.Add(Message.InvalidInputMessage);
            }
        }
        
        return errorMessages.ToArray();
    }
}