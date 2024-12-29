using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DAL;
using Domain;
using DTO;
using GameBrain;
using Microsoft.Build.Framework;

namespace WebApp.Pages
{
    public class NewGameModel : PageModel
    {
        private readonly ILogger<NewGameModel> _logger;

        private readonly IConfigRepository _configRepository;

        private readonly IGameRepository _gameRepository;

        public NewGameModel(AppDbContext context, ILogger<NewGameModel> logger)
        {
            var repoController = new RepoController(context);
            _configRepository = repoController.ConfigRepository;
            _gameRepository = repoController.GameRepository;
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            ViewData["ConfigurationName"] = new SelectList(_configRepository.GetConfigurationNames());
            ViewData["GameMode"] = new SelectList(GameMode.GetGameModeNames());
            if (!string.IsNullOrEmpty(GameModeName))
            {
                ModeName = GameModeName;
            }
            if (TempData["FormValues"] is Dictionary<string, string> formValues)
            {
                GameName = formValues.GetValueOrDefault("GameName", "");
                ConfigurationName = formValues.GetValueOrDefault("ConfigurationName", "");
                ModeName = formValues.GetValueOrDefault("ModeName", "");
                PlayerXName = formValues.GetValueOrDefault("PlayerXName", "");
                PlayerOName = formValues.GetValueOrDefault("PlayerOName", "");
                PlayerXPassword = formValues.GetValueOrDefault("PlayerXPassword", "");
                PlayerOPassword = formValues.GetValueOrDefault("PlayerOPassword", "");
            }
            return Page();
        }

        [BindProperty(SupportsGet = true)] public string? GameModeName { get; set; } = default!;

        [Required] [BindProperty] public string GameName { get; set; } = default!;

        [Required] [BindProperty] public string ConfigurationName { get; set; } = default!;

        [Required] [BindProperty] public string ModeName { get; set; } = default!;

        [Required] [BindProperty] public string PlayerXName { get; set; } = default!;

        [Required] [BindProperty] public string PlayerOName { get; set; } = default!;

        [Required] [BindProperty] public string PlayerXPassword { get; set; } = default!;

        [Required] [BindProperty] public string PlayerOPassword { get; set; } = default!;

        public Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("GameModeName");
            
            if (!ModelState.IsValid)
            {
                ViewData["ConfigurationName"] = new SelectList(_configRepository.GetConfigurationNames());
                ViewData["GameMode"] = new SelectList(GameMode.GetGameModeNames());
                foreach (var error in ModelState)
                {
                    _logger.LogError(
                        $"{error.Key}: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                }

                return Task.FromResult<IActionResult>(Page());
            }

            if (_gameRepository.GameExists(GameName))
            {
                TempData["FormValues"] = new Dictionary<string, string>
                {
                    { "GameName", GameName },
                    { "ConfigurationName", ConfigurationName },
                    { "ModeName", ModeName },
                    { "PlayerXName", PlayerXName },
                    { "PlayerOName", PlayerOName },
                    { "PlayerXPassword", PlayerXPassword },
                    { "PlayerOPassword", PlayerOPassword }
                };
                TempData["ErrorMessage"] = Message.GameNameAlreadyInUseMessage;
                return Task.FromResult<IActionResult>(RedirectToPage());
            }

            var savedGame = CreateSavedGame();
            _gameRepository.CreateGame(savedGame);

            return Task.FromResult<IActionResult>(RedirectToPage(
                "./StartGame",
                new { gameName = savedGame.Name }
            ));
        }

        public SavedGame CreateSavedGame()
        {
            var config = _configRepository.GetConfigurationByName(ConfigurationName);
            return new SavedGame
            {
                Name = GameName,
                ModeName = ModeName,
                PlayerXName = PlayerXName,
                PlayerOName = PlayerOName,
                PlayerXPassword = PlayerXPassword,
                PlayerOPassword = PlayerOPassword,
                CreatedAtDateTime = DateTime.Now.ToString("O"),
                State = new GameState(
                    config,
                    GameMode.GetMode(ModeName),
                    PlayerXName,
                    PlayerOName
                ).ToString(),
                Configuration = config
            };
        }
    }
}