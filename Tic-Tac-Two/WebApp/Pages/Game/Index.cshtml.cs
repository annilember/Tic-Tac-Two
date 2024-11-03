using Domain;
using GameBrain;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Game;

public class Index : PageModel
{
    public TicTacTwoBrain TicTacTwoBrain { get; set; } = default!;
    
    public void OnGet()
    {
        GameConfiguration gameConfiguration = new GameConfiguration()
        {
            Name = "My Classical Config"
        };
        
        TicTacTwoBrain = new TicTacTwoBrain(gameConfiguration, "Maali", "Juuli");
        TicTacTwoBrain.MakeAMove(2, 2);
        TicTacTwoBrain.MakeAMove(4, 4);
        TicTacTwoBrain.MakeAMove(0, 2);
    }
}