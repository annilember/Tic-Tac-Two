using MenuSystem;

namespace ConsoleApp;

public static class Menus
{
    public static readonly Menu OptionsMenu = new Menu(
        EMenuLevel.Secondary,
        "TIC-TAC-TWO Options", [
            new MenuItem()
            {
                Shortcut = "X",
                Title = "X Starts",
                MenuItemAction = DummyMethod
            },

            new MenuItem()
            {
                Shortcut = "O",
                Title = "O Starts",
                MenuItemAction = DummyMethod
            }
        ]);
    
    public static Menu MainMenu = new Menu(
        EMenuLevel.Main,
        "TIC-TAC-TWO", [
            new MenuItem()
            {
                Shortcut = "O",
                Title = "Options",
                MenuItemAction = OptionsMenu.Run
            },

            new MenuItem()
            {
                Shortcut = "N",
                Title = "New Game",
                MenuItemAction = GameController.MainLoop
            }
        ]);

    private static string DummyMethod()
    {
        Console.Write("Press any key to exit...");
        Console.ReadKey();
        return "Dummy";
    }
}