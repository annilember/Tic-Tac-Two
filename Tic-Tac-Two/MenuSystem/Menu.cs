using System.Globalization;
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
        var message = "";
        
        do
        {
            Console.Clear();
            if (!string.IsNullOrWhiteSpace(message))
            {
                Visualizer.WriteErrorMessage(message);
                Console.WriteLine();
            }
            
            var menuItem = DisplayMenuGetUserChoice(message);
            message = "";
            var menuReturnValue = "";
        
            if (menuItem.MenuItemAction != null)
            {
                menuReturnValue = menuItem.MenuItemAction();
            }
            
            if (menuItem.Shortcut == _menuItemReturn.Shortcut)
            {
                return menuItem.Shortcut;
            }
            
            if (menuItem.Shortcut == _menuItemExit.Shortcut || menuReturnValue == _menuItemExit.Shortcut)
            {
                return _menuItemExit.Shortcut;
            }
            
            if ((menuItem.Shortcut == _menuItemReturnMain.Shortcut || 
                 menuReturnValue == _menuItemReturnMain.Shortcut) && 
                _menuLevel != EMenuLevel.Main)
            {
                return _menuItemReturnMain.Shortcut;
            }

            if (menuReturnValue == _menuItemReturn.Shortcut)
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

    private MenuItem DisplayMenuGetUserChoice(string message)
    {
        var errorMessage = message;

        do
        {
            DrawMenu(errorMessage);
            
            var userInput = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(userInput))
            {
                errorMessage = "Please choose an option.";
            }
            else
            {
                userInput = userInput.ToUpper();
                
                foreach (var menuItem in MenuItems)
                {
                    if (menuItem.Shortcut.ToUpper() != userInput) continue;
                    return menuItem;
                }
                
                errorMessage = "Please choose a valid option.";
                Console.WriteLine();
            }
            
        } while (true);
    }

    private void DrawMenu(string errorMessage)
    {
        Console.Clear();
        Console.WriteLine(MenuHeader);
        Console.WriteLine(_menuDivider);
        
        foreach (var t in MenuItems)
        {
            Console.WriteLine(t);
        }

        if (!string.IsNullOrWhiteSpace(errorMessage))
        {
            Visualizer.WriteErrorMessage(errorMessage);
        }
        Console.WriteLine();
        Console.Write(">");
    }
}
