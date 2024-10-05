// An example of bad design, fine for UI prototyping.

using GameBrain;
using MenuSystem;

var gameInstance = new TicTacTwoBrain();

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
            MenuItemAction = NewGame
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

string NewGame()
{
    ConsoleUI.Visualizer.DrawBoard(gameInstance);
    
    Console.Write("Give me coordinates <x,y>:");
    var input = Console.ReadLine()!;
    var inputSplit = input.Split(',');
    var inputX = int.Parse(inputSplit[0]);
    var inputY = int.Parse(inputSplit[1]);
    gameInstance.MakeAMove(inputX, inputY);
    
    // loop
    // draw the board again
    // ask input again
    // is game over?
    // add validation!
    
    return "";
}