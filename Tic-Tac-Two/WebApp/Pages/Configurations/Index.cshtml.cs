using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Domain;

namespace WebApp.Pages.Configurations
{
    public class IndexModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public IndexModel(DAL.AppDbContext context)
        {
            _context = context;
        }

        public IList<GameConfiguration> GameConfiguration { get;set; } = default!;

        public async Task OnGetAsync()
        {
            GameConfiguration = await _context.Configurations.ToListAsync();
        }
    }
}
