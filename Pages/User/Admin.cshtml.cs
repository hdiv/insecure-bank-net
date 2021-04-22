using System.Collections.Generic;
using System.Linq;
using insecure_bank_net.Dao;
using insecure_bank_net.Facade;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace insecure_bank_net.Pages.User
{
    public class Admin : PageModel
    {
        private readonly IAuthenticationFacade authenticationFacade;
        private readonly IAccountDao accountDao;

        public Bean.Account Account { get; set; }
        
        public List<Bean.Account> Accounts { get; set; }
        
        public Admin(IAuthenticationFacade authenticationFacade, IAccountDao accountDao)
        {
            this.authenticationFacade = authenticationFacade;
            this.accountDao = accountDao;
        }

        public void OnGet()
        {
            var principal = authenticationFacade.GetPrincipal();
            Account = accountDao.FindUsersByUsername(principal.Identity!.Name).First();
            Accounts = accountDao.FindAllUsers();
        }
    }
}