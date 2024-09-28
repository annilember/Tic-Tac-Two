// An example of bad design, fine for UI prototyping.
using MenuSystem;

var mainMenu = new Menu("TIC-TAC-TOE", [
    new MenuItem()
    {
        Shortcut = "O",
        Title = "Options"
    },

    new MenuItem()
    {
        Shortcut = "N",
        Title = "New Game"
    }
]);

mainMenu.Run();

return;
// ============================================

static void MenuStart()
{
    Console.Clear();
    Console.WriteLine("TIC-TAC-TOE");
    Console.WriteLine("------------------");
}

static void MenuMain()
{
    MenuStart();
    Console.WriteLine("O) Options");
    Console.WriteLine("N) New Game");
    Console.WriteLine("L) Load Game");
    Console.WriteLine("E) Exit");
    MenuEnd();
}

static void MenuOptions()
{
    MenuStart();
    Console.WriteLine("Choose symbol for player one (X)");
    Console.WriteLine("Choose symbol for player two (O)");
    MenuEnd();
}

static void MenuEnd()
{
    Console.WriteLine();
    Console.Write(">");
}