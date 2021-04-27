using System.Collections.Generic;
using System.Linq;
using insecure_bank_net.Bean;
using insecure_bank_net.Dao;
using insecure_bank_net.Facade;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace insecure_bank_net.Pages.User
{
    public class Dashboard : PageModel
    {
        private readonly IAuthenticationFacade authenticationFacade;
        private readonly IAccountDao accountDao;
        private readonly ICashAccountDao cashAccountDao;
        private readonly ICreditAccountDao creditAccountDao;

        public Dashboard(IAuthenticationFacade authenticationFacade, IAccountDao accountDao,
            ICashAccountDao cashAccountDao, ICreditAccountDao creditAccountDao)
        {
            this.authenticationFacade = authenticationFacade;
            this.accountDao = accountDao;
            this.cashAccountDao = cashAccountDao;
            this.creditAccountDao = creditAccountDao;
        }

        public Bean.Account Account { get; set; }

        public List<CashAccount> CashAccounts { get; set; }

        public List<CreditAccount> CreditAccounts { get; set; }

        public void OnGet()
        {
            var principal = authenticationFacade.GetPrincipal();
            var account = accountDao.FindUsersByUsername(principal.Identity!.Name).First();
            var cashAccounts = cashAccountDao.FindCashAccountsByUsername(account.Username);
            var creditAccounts = creditAccountDao.FindCreditAccountsByUsername(account.Username);
            Account = account;
            CashAccounts = cashAccounts;
            CreditAccounts = creditAccounts;
        }
    }
}