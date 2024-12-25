using Domain;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages;

public class ConfigsModel : PageModel
{
    private readonly DAL.AppDbContext _context;

    public ConfigsModel(DAL.AppDbContext context)
    {
        _context = context;
    }

    public IList<GameConfiguration> GameConfiguration { get;set; } = default!;

    public async Task OnGetAsync()
    {
        GameConfiguration = await _context.Configurations.ToListAsync();
    }
}