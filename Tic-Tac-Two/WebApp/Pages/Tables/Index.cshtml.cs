using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Domain;

namespace WebApp.Pages.Tables
{
    public class IndexModel : PageModel
    {
        private readonly WebApp.Data.AppDbContext _context;

        public IndexModel(WebApp.Data.AppDbContext context)
        {
            _context = context;
        }

        public IList<Table> Table { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Table = await _context.Tables
                .Include(t => t.Restaurant).ToListAsync();
        }
    }
}
