// See https://aka.ms/new-console-template for more information

using DAL;
using Domain;
using Microsoft.EntityFrameworkCore;

var connectionString = $"Data Source={FileHelper.BasePath}app.db";
    
Console.WriteLine("Hello, DB/EF demo!");
Console.WriteLine("Db is in: " + connectionString);

var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
    .UseSqlite(connectionString)
    .EnableDetailedErrors()
    .EnableSensitiveDataLogging()
    .Options;

using var context = new AppDbContext(contextOptions);

// if there's something to migrate, then it will, otherwise nothing happens.
// this is like the migration update from command line
context.Database.Migrate();

Console.WriteLine($"Games in db: {context.Savegames.Count()}");
Console.WriteLine($"Configs in db: {context.Configurations.Count()}");

// Include does sql join
foreach (var conf in context.Configurations
             .Include(c => c.SaveGames)
             .Where(c => c.Id == 2))
{
    Console.WriteLine(conf);
}

// // This is how we inserted initial data.
//
// var config = new Configuration()
// {
//     Name = "Standard",
//     BoardWidth = 3,
//     BoardHeight = 3
// };
//
// // now we add stuff to dbSet, like to a list
// // context.Configurations.Add(config);
//
// var saveGame = new SaveGame()
// {
//     CreatedAtDateTime = "eile",
//     State = "{}",
//     Configuration = config
// };
//
// // you can see that the config was found from inside the savedgame
// // and the config was also added to db
// context.Savegames.Add(saveGame);
//
// context.SaveChanges();
//
//
// saveGameCount = context.Savegames.Count();
// Console.WriteLine($"Games in db: {saveGameCount}");
// Console.WriteLine($"Configs in db: {context.Configurations.Count()}");
