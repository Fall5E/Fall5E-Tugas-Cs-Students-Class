using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TenThousandHours.Pages;

public class IndexModel(SignInManager<IdentityUser> signInManager) : PageModel
{
    private readonly SignInManager<IdentityUser> _signInManager = signInManager;

    public IActionResult OnGet()
    {
        if (_signInManager.IsSignedIn(User))
        {
            return RedirectToPage("/Today");
        }

        return Page();
    }
}
