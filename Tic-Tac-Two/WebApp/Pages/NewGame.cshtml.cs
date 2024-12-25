using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DAL;
using Domain;

namespace WebApp.Pages
{
    public class NewGameModel : PageModel
    {
        private readonly ILogger<NewGameModel> _logger;
        
        private readonly AppDbContext _context;
        
        private readonly IConfigRepository _configRepository;
        
        private readonly IGameRepository _gameRepository;

        public NewGameModel(AppDbContext context, ILogger<NewGameModel> logger)
        {
            var repoController = new RepoController(context);
            _configRepository = repoController.ConfigRepository;
            _gameRepository = repoController.GameRepository;
            _context = context;
            _logger = logger;
        }

        public IActionResult OnGet()
        {
        ViewData["ConfigurationName"] = new SelectList(_configRepository.GetConfigurationNames());
        ViewData["GameMode"] = new SelectList(GameMode.GetGameModeNames());
            return Page();
        }
        
        
        [BindProperty]
        public string GameName { get; set; } = default!;
        
        [BindProperty]
        public string ConfigurationName { get; set; } = default!;
        
        [BindProperty]
        public string ModeName { get; set; } = default!;
        
        [BindProperty]
        public string PlayerXName { get; set; } = default!;
        
        [BindProperty]
        public string PlayerOName { get; set; } = default!;
        
        [BindProperty]
        public string PlayerXPassword { get; set; } = default!;
        
        [BindProperty]
        public string PlayerOPassword { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
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
