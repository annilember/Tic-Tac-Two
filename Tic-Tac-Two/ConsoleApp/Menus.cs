using MenuSystem;

namespace ConsoleApp;

public static class Menus
{
    public static readonly Menu ConfigOptionsMenu = new Menu(
        EMenuLevel.Deep,
        "TIC-TAC-TWO - Config options", [
            new MenuItem()
            {
                Shortcut = "A",
                Title = "Create new config",
                MenuItemAction = OptionsController.CreateNewConfig
            },

            new MenuItem()
            {
                Shortcut = "B",
                Title = "Change config",
                MenuItemAction = OptionsController.ChangeExistingConfiguration
            },
            
            new MenuItem()
            {
                Shortcut = "C",
                Title = "Delete config",
                MenuItemAction = OptionsController.DeleteExistingConfiguration
            }
        ]);
    
    public static readonly Menu OptionsMenu = new Menu(
        EMenuLevel.Secondary,
        "TIC-TAC-TWO Options", [
            new MenuItem()
            {
                Shortcut = "C",
                Title = "Config options",
                MenuItemAction = ConfigOptionsMenu.Run
            },

            new MenuItem()
            {
                Shortcut = "X",
                Title = "X Starts",
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

    public static string DummyMethod()
    {
        Console.Write("Press any key to exit...");
        Console.ReadKey();
        return "Dummy";
    }
}