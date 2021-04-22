using System;
using System.Security.Claims;
using System.Threading.Tasks;
using insecure_bank_net.Facade;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace insecure_bank_net.Pages.Account
{
    public class Login : PageModel
    {
        public class InputModel
        {
            public string Username { get; set; }

            public string Password { get; set; }
        }

        private readonly IAuthenticationFacade authenticationFacade;

        [BindProperty] public InputModel Input { get; set; }

        public Login(IAuthenticationFacade authenticationFacade)
        {
            this.authenticationFacade = authenticationFacade;
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var user = await authenticationFacade.SignInUser(Input.Username, Input.Password);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid username or password");
                    return Page();
                }

                if (!Url.IsLocalUrl(returnUrl))
                {
                    returnUrl = Url.Content("~/");
                }

                return LocalRedirect(returnUrl);
            }

            return Page();
        }
    }
}