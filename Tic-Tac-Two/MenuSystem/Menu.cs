using ConsoleUI;
using GameBrain;

namespace MenuSystem;

public class Menu
{
    private string MenuHeader { get; }
    private List<MenuItem> MenuItems { get; }
    private readonly MenuItem _menuItemExit = new()
    {
        Shortcut = "E",
        Title = "Exit",
        MenuItemAction = null
    };
    private readonly MenuItem _menuItemReturn = new()
    {
        Shortcut = "R",
        Title = "Return",
        MenuItemAction = null
    };
    private readonly MenuItem _menuItemReturnMain = new()
    {
        Shortcut = "M",
        Title = "Return to Main Menu",
        MenuItemAction = null
    };
    
    private EMenuLevel MenuLevel { get; }
    
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
        
        MenuLevel = menuLevel;
        
        if (MenuLevel != EMenuLevel.Main)
        {
            MenuItems.Add(_menuItemReturn);
        }
        if (MenuLevel == EMenuLevel.Deep)
        {
            MenuItems.Add(_menuItemReturnMain);
        }
        MenuItems.Add(_menuItemExit);
    }

    public string Run()
    {
        return Run("");
    }

    public string Run(string message)
    {
        
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
                MenuLevel != EMenuLevel.Main)
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
                errorMessage = Message.MenuChooseOptionMessage;
            }
            else
            {
                userInput = userInput.ToUpper();
                
                foreach (var menuItem in MenuItems)
                {
                    if (menuItem.Shortcut.ToUpper() != userInput) continue;
                    return menuItem;
                }

                errorMessage = Message.MenuChooseValidOptionMessage;
                Console.WriteLine();
            }
            
        } while (true);
    }

    private void DrawMenu(string errorMessage)
    {
        Console.Clear();
        Console.WriteLine(MenuHeader);
        Console.WriteLine(VisualizerHelper.Divider);
        
        foreach (var t in MenuItems)
        {
            Console.WriteLine(t);
        }

        if (!string.IsNullOrWhiteSpace(errorMessage))
        {
            Visualizer.WriteErrorMessage(errorMessage);
        }
        Console.WriteLine();
        Console.Write(VisualizerHelper.MenuInputArrow);
    }
}
