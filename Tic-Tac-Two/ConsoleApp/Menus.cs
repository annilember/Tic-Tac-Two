using MenuSystem;

namespace ConsoleApp;

public static class Menus
{
    private static readonly Menu ConfigOptionsMenu = new Menu(
        EMenuLevel.Deep,
        "TIC-TAC-TWO - Config options", [
            new MenuItem()
            {
                Shortcut = "N",
                Title = "Create new config",
                MenuItemAction = OptionsController.CreateNewConfig
            },

            new MenuItem()
            {
                Shortcut = "C",
                Title = "Change config",
                MenuItemAction = OptionsController.ChangeExistingConfiguration
            },
            
            new MenuItem()
            {
                Shortcut = "D",
                Title = "Delete config",
                MenuItemAction = OptionsController.DeleteExistingConfiguration
            }
        ]);

    private static readonly Menu GameOptionsMenu = new Menu(
        EMenuLevel.Deep,
        "TIC-TAC-TWO - Game options", [
            new MenuItem()
            {
                Shortcut = "D",
                Title = "Delete saved game",
                MenuItemAction = OptionsController.DeleteSavedGame
            },
            
            new MenuItem()
            {
                Shortcut = "N",
                Title = "Rename saved game",
                MenuItemAction = OptionsController.RenameSavedGame
            }
        ]);

    private static readonly Menu OptionsMenu = new Menu(
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
                Shortcut = "G",
                Title = "Game options",
                MenuItemAction = GameOptionsMenu.Run
            }
        ]);
    
    public static Menu MainMenu = new Menu(
        EMenuLevel.Main,
        "TIC-TAC-TWO", [
            
            new MenuItem()
            {
                Shortcut = "N",
                Title = "New Game",
                MenuItemAction = GameController.StartNewGame
            },

            new MenuItem()
            {
                Shortcut = "L",
                Title = "Load Game",
                MenuItemAction = GameController.LoadSavedGame
            },
            
            new MenuItem()
            {
                Shortcut = "O",
                Title = "Options",
                MenuItemAction = OptionsMenu.Run
            },
            
            new MenuItem()
            {
                Shortcut = "G",
                Title = "Game Rules",
                MenuItemAction = OptionsController.DisplayGameRules
            }
        ]);
}