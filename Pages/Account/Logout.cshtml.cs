using System.Threading.Tasks;
using insecure_bank_net.Facade;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace insecure_bank_net.Pages.Account
{
    public class Logout : PageModel
    {
        private readonly IAuthenticationFacade authenticationFacade;

        public Logout(IAuthenticationFacade authenticationFacade)
        {
            this.authenticationFacade = authenticationFacade;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await authenticationFacade.SignOutUser();
            return Redirect("/");
        }
    }
}