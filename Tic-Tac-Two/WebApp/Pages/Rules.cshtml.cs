using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class RulesModel : PageModel
{
    private readonly AppDbContext _context;

    public RulesModel(AppDbContext context)
    {
        _context = context;
    }

    public void OnGet()
    {

    }
}