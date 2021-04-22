using System.Linq;
using insecure_bank_net.Dao;
using insecure_bank_net.Facade;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace insecure_bank_net.Pages.User
{
    public class CreditActivity : PageModel
    {
        private readonly IAuthenticationFacade authenticationFacade;
        private readonly IAccountDao accountDao;

        public Bean.Account Account { get; set; }
            
        public string ActualCreditCardNumber { get; set; }

        [BindProperty]
        public string Number { get; set; }
        
        public CreditActivity(IAuthenticationFacade authenticationFacade, IAccountDao accountDao)
        {
            this.authenticationFacade = authenticationFacade;
            this.accountDao = accountDao;
        }
        
        public void OnGet(string number)
        {
            var principal = authenticationFacade.GetPrincipal();
            var account = accountDao.FindUsersByUsername(principal.Identity!.Name).First();
            Account = account;
            ActualCreditCardNumber = number;
        }
    }
}