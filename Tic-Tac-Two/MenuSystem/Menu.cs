using ConsoleUI;

namespace MenuSystem;

public class Menu
{
    private string MenuHeader { get; set; }
    private static string _menuDivider = "======================";
    private List<MenuItem> MenuItems { get; set; }
    private MenuItem _menuItemExit = new MenuItem()
    {
        Shortcut = "E",
        Title = "Exit",
        MenuItemAction = null
    };
    private MenuItem _menuItemReturn = new MenuItem()
    {
        Shortcut = "R",
        Title = "Return",
        MenuItemAction = null
    };
    private MenuItem _menuItemReturnMain = new MenuItem()
    {
        Shortcut = "M",
        Title = "Return to Main Menu",
        MenuItemAction = null
    };
    
    private EMenuLevel _menuLevel { get; set; }

    public void SetMenuItemAction(string shortCut, Func<string> action)
    {
        var menuItem = MenuItems.Single(m => m.Shortcut == shortCut);
        menuItem.MenuItemAction = action;
    }
    
    public Menu(EMenuLevel menuLevel, string menuHeader, List<MenuItem> menuItems)
    {
        if (string.IsNullOrWhiteSpace(menuHeader))
        {
            throw new ApplicationException("Menu header cannot be empty.");
        }
        
        MenuHeader = menuHeader;

        if (menuItems == null || menuItems.Count == 0)
        {
            throw new ApplicationException("Menu items cannot be empty.");
        }
        
        MenuItems = menuItems;
        
        _menuLevel = menuLevel;
        
        if (_menuLevel != EMenuLevel.Main)
        {
            MenuItems.Add(_menuItemReturn);
        }
        if (_menuLevel == EMenuLevel.Deep)
        {
            MenuItems.Add(_menuItemReturnMain);
        }
        MenuItems.Add(_menuItemExit);
        
        //TODO! Validate menuItems for shortcut conflict!
    }

    public string Run()
    {
        //TODO! Implement main menu loop!

        var message = "";
        do
        {
            Console.Clear();
            if (!string.IsNullOrWhiteSpace(message))
            {
                WriteMessage(message);
            }
            var menuItem = DisplayMenuGetUserChoice();
            message = "";
            var menuReturnValue = "";
        
            if (menuItem.MenuItemAction != null)
            {
                menuReturnValue = menuItem.MenuItemAction();
            }
            
            if (menuItem.Shortcut == "R")
            {
                return menuItem.Shortcut;
            }
            
            if (menuItem.Shortcut == "E" || menuReturnValue == "E")
            {
                return "E";
            }
            
            if ((menuItem.Shortcut == "M" || menuReturnValue == "M") && _menuLevel != EMenuLevel.Main)
            {
                return menuItem.Shortcut;
            }

            if (menuReturnValue == "R")
            {
                continue;
            }
            
            if (menuReturnValue.Length > 1)
            {
                message = menuReturnValue;
            }
            
            if (!string.IsNullOrWhiteSpace(menuReturnValue) && int.TryParse(menuReturnValue, out _))
            {
                return menuReturnValue;
            }
        } while (true);
    }

    private MenuItem DisplayMenuGetUserChoice()
    {
        var userInput = "";

        do
        {
            DrawMenu();
            
            userInput = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(userInput))
            {
                Console.WriteLine("Please choose an option.");
                Console.WriteLine();
            }
            else
            {
                userInput = userInput.ToUpper();
                
                foreach (var menuItem in MenuItems)
                {
                    if (menuItem.Shortcut.ToUpper() != userInput) continue;
                    return menuItem;
                }
                
                Console.WriteLine("Please choose a valid option.");
                Console.WriteLine();
            }
            
        } while (true);
    }

    private void DrawMenu()
    {
        Console.WriteLine(MenuHeader);
        Console.WriteLine(_menuDivider);
        
        foreach (var t in MenuItems)
        {
            Console.WriteLine(t);
        }
        
        Console.WriteLine();
        Console.Write(">");
    }

    private void WriteMessage(string message)
    {
        Console.ForegroundColor = VisualizerHelper.ErrorMessageColor;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}
