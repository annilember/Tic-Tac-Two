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

var configRepository = new ConfigRepositoryDb(db);
var gameRepository = new GameRepositoryDb(db);

Menus.Init(configRepository, gameRepository);
Menus.MainMenu.Run();
