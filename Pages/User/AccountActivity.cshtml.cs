using System.Collections.Generic;
using System.Linq;
using insecure_bank_net.Bean;
using insecure_bank_net.Dao;
using insecure_bank_net.Facade;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace insecure_bank_net.Pages.User
{
    public class AccountActivity : PageModel
    {
        private readonly IAuthenticationFacade authenticationFacade;
        private readonly IAccountDao accountDao;
        private readonly ICashAccountDao cashAccountDao;
        private readonly IActivityDao activityDao;

        public Bean.Account Account { get; set; }

        public List<Transaction> FirstCashAccountTransfers { get; set; }

        public List<SelectListItem> CashAccounts { get; set; }

        public string ActualCashAccountNumber { get; set; }

        [BindProperty] public string Number { get; set; }

        public AccountActivity(IAuthenticationFacade authenticationFacade, IAccountDao accountDao,
            ICashAccountDao cashAccountDao, IActivityDao activityDao)
        {
            this.authenticationFacade = authenticationFacade;
            this.accountDao = accountDao;
            this.cashAccountDao = cashAccountDao;
            this.activityDao = activityDao;
        }

        public void OnGet(string number)
        {
            ShowAccount(number);
        }

        public void OnPost()
        {
            ShowAccount(Number);
        }

        private void ShowAccount(string number)
        {
            var principal = authenticationFacade.GetPrincipal();
            var account = accountDao.FindUsersByUsername( principal.Identity!.Name).First();
            var cashAccounts = cashAccountDao.FindCashAccountsByUsername(account.Username);

            var cashAccount = number ?? cashAccounts.First().Number;
            var firstCashAccountTransfers = activityDao.FindTransactionsByCashAccountNumber(cashAccount);
            Account = account;
            FirstCashAccountTransfers = Enumerable.Reverse(firstCashAccountTransfers).ToList();
            ActualCashAccountNumber = number;
            CashAccounts = cashAccounts.Select(item => new SelectListItem
            {
                Text = $"{item.Number} ({item.Description})",
                Value = item.Number,
                Selected = item.Number == number
            }).ToList();
        }
    }
}