using ConsoleApp;
using DAL;
using Microsoft.EntityFrameworkCore;

var connectionString = $"Data Source={FileHelper.BasePath}app.db";
var options = new DbContextOptionsBuilder<AppDbContext>()
    .UseSqlite(connectionString)
    .EnableDetailedErrors()
    .EnableSensitiveDataLogging()
    .Options;
using var db = new AppDbContext(options);

var repoController = new RepoController(db);
var configRepository = repoController.ConfigRepository;
var gameRepository = repoController.GameRepository;

OptionsController.Init(configRepository, gameRepository);
GameController.Init(configRepository, gameRepository);
Menus.MainMenu.Run();
