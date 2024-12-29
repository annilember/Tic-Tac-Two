using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger, AppDbContext context)
    {
        _logger = logger;
    }
    
    public Task<IActionResult> OnPostNewGame()
    {
        return Task.FromResult<IActionResult>(RedirectToPage("./NewGame"));
    }
    
    public Task<IActionResult> OnPostLoadGame()
    {
        return Task.FromResult<IActionResult>(RedirectToPage("./LoadGame"));
    }
}