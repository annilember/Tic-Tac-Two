// An example of bad design, fine for UI prototyping.
using MenuSystem;

var deepMenu = new Menu(
    EMenuLevel.Deep,
    "TIC-TAC-TOE Deep stuff...", [
        new MenuItem()
        {
            Shortcut = "Y",
            Title = "Why is the question.",
            MenuItemAction = DummyMethod
        }
    ]);
var optionsMenu = new Menu(
    EMenuLevel.Secondary,
    "TIC-TAC-TOE Options", [
        new MenuItem()
        {
            Shortcut = "X",
            Title = "X Starts",
            MenuItemAction = deepMenu.Run
        },

        new MenuItem()
        {
            Shortcut = "O",
            Title = "O Starts",
            MenuItemAction = DummyMethod
        }
    ]);

var mainMenu = new Menu(
    EMenuLevel.Main,
    "TIC-TAC-TOE", [
        new MenuItem()
        {
            Shortcut = "O",
            Title = "Options",
            MenuItemAction = optionsMenu.Run
        },

        new MenuItem()
        {
            Shortcut = "N",
            Title = "New Game",
            MenuItemAction = DummyMethod
        }
    ]);

mainMenu.Run();

return;
// ============================================

string DummyMethod()
{
    Console.Write("Press any key to exit...");
    Console.ReadKey();
    return "Dummy";
}