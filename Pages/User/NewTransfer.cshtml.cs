using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using insecure_bank_net.Bean;
using insecure_bank_net.Dao;
using insecure_bank_net.Facade;
using insecure_bank_net.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace insecure_bank_net.Pages.User
{
    public class NewTransfer : PageModel
    {
        private const string PENDING_TRANSFER = "PENDING_TRANSFER";

        private readonly IAuthenticationFacade authenticationFacade;
        private readonly IAccountDao accountDao;
        private readonly ICashAccountDao cashAccountDao;
        private readonly ITransferFacade transferFacade;

        public Bean.Account Account { get; set; }

        public List<SelectListItem> CashAccounts { get; set; }

        public string Type { get; set; }

        [BindProperty] public Transfer Transfer { get; set; }

        [BindProperty] public OperationConfirm Confirm { get; set; }

        public NewTransfer(IAuthenticationFacade authenticationFacade, ITransferFacade transferFacade, IAccountDao accountDao, ICashAccountDao cashAccountDao)
        {
            this.authenticationFacade = authenticationFacade;
            this.transferFacade = transferFacade;
            this.accountDao = accountDao;
            this.cashAccountDao = cashAccountDao;
        }

        public void OnGet()
        {
            var principal = authenticationFacade.GetPrincipal();
            var account = accountDao.FindUsersByUsername(principal.Identity!.Name).First();
            var cashAccounts = cashAccountDao.FindCashAccountsByUsername(account.Username);
            Account = account;
            CashAccounts = cashAccounts.Select(item => new SelectListItem
            {
                Text = $"{item.Number} ({item.Description})",
                Value = item.Number
            }).ToList();
            Transfer = new Transfer();
            Response.Cookies.Append("accountType", AccountType.PERSONAL);
        }

        public IActionResult OnPostTransfer()
        {
            if (Transfer.Amount <= 0)
            {
                ModelState.AddModelError("Input.Amount", "You must enter a positive amount");
            }

            if (ModelState.ErrorCount > 0)
            {
                OnGet();
                return Page();
            }

            var principal = authenticationFacade.GetPrincipal();
            Account = accountDao.FindUsersByUsername(principal.Identity!.Name).First();

            var accountType = Request.Cookies["accountType"] ?? AccountType.PERSONAL;
            return accountType.Equals(AccountType.PERSONAL)
                ? TransferCheck(Transfer)
                : TransferConfirmation(Transfer, accountType);
        }

        public IActionResult OnPostConfirm()
        {
            var json = Encoding.UTF8.GetString(HttpContext.Session.Get(PENDING_TRANSFER));
            HttpContext.Session.Remove(PENDING_TRANSFER);
            var transfer = JsonSerializer.Deserialize<Transfer>(json);
            
            if ("Confirm".Equals(this.Confirm.Action))
            {
                var accountType = Request.Cookies["accountType"] ?? AccountType.PERSONAL;
                return TransferConfirmation(transfer, accountType);
            }

            return RedirectToPage("/User/NewTransfer");
        }

        private IActionResult TransferCheck(Transfer transfer)
        {
            Transfer = transfer;
            Confirm = new OperationConfirm();
            HttpContext.Session.Set(PENDING_TRANSFER, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(Transfer)));
            return new ViewResult
            {
                ViewName = "TransferCheck",
                ViewData = ViewData
            };
        }

        private IActionResult TransferConfirmation(Transfer transfer, string accountType)
        {
            var principal = authenticationFacade.GetPrincipal();
            transfer.Username = principal.Identity!.Name;
            transfer.Date = DateTime.Now;

            var amount = transfer.Amount;
            transfer.Amount = InsecureBankUtils.Round(amount, 2);

            var feeAmount = transfer.Amount * transfer.Fee / 100.0;
            transfer.Fee = InsecureBankUtils.Round(feeAmount, 2);

            Type = accountType;
            transferFacade.CreateNewTransfer(transfer);

            Account = accountDao.FindUsersByUsername(transfer.Username).First();
            Transfer = transfer;
            
            return new ViewResult
            {
                ViewName = "TransferConfirmation",
                ViewData = ViewData
            };
        }
    }

    public static class AccountType
    {
        public const string PERSONAL = "Personal";

        public const string BUSINESS = "Business";
    }
}