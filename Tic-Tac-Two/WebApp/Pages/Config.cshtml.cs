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
        
        if (TempData["FormValues"] is Dictionary<string, string> formValues)
        {
            GameConfiguration.Name = formValues.GetValueOrDefault("Name", "");
            
            int ParseValue(string key) => 
                int.TryParse(formValues.GetValueOrDefault(key), out var result) ? result : 0;
            
            GameConfiguration.BoardSizeWidth = ParseValue("BoardSizeWidth");
            GameConfiguration.BoardSizeHeight = ParseValue("BoardSizeHeight");
            GameConfiguration.GridSizeWidth = ParseValue("GridSizeWidth");
            GameConfiguration.GridSizeHeight = ParseValue("GridSizeHeight");
            GameConfiguration.GridStartPosX = ParseValue("GridStartPosX");
            GameConfiguration.GridStartPosY = ParseValue("GridStartPosY");
            GameConfiguration.NumberOfPieces = ParseValue("NumberOfPieces");
            GameConfiguration.WinCondition = ParseValue("WinCondition");
            GameConfiguration.MaxGameRounds = ParseValue("MaxGameRounds");
            GameConfiguration.MoveGridAfterNMoves = ParseValue("MoveGridAfterNMoves");
            GameConfiguration.MovePieceAfterNMoves = ParseValue("MovePieceAfterNMoves");
        }
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
            TempData["FormValues"] = new Dictionary<string, string>
            {
                { "Name", GameConfiguration.Name },
                { "BoardSizeWidth", GameConfiguration.BoardSizeWidth.ToString() },
                { "BoardSizeHeight", GameConfiguration.BoardSizeHeight.ToString() },
                { "GridSizeWidth", GameConfiguration.GridSizeWidth.ToString() },
                { "GridSizeHeight", GameConfiguration.GridSizeHeight.ToString() },
                { "GridStartPosX", GameConfiguration.GridStartPosX.ToString() },
                { "GridStartPosY", GameConfiguration.GridStartPosY.ToString() },
                { "NumberOfPieces", GameConfiguration.NumberOfPieces.ToString() },
                { "WinCondition", GameConfiguration.WinCondition.ToString() },
                { "MaxGameRounds", GameConfiguration.MaxGameRounds.ToString() },
                { "MoveGridAfterNMoves", GameConfiguration.MoveGridAfterNMoves.ToString() },
                { "MovePieceAfterNMoves", GameConfiguration.MovePieceAfterNMoves.ToString() }
            };

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