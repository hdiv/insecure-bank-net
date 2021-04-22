using insecure_bank_net.Bean;
using insecure_bank_net.Dao;
using insecure_bank_net.Data;
using insecure_bank_net.Util;

namespace insecure_bank_net.Facade
{
    public class TransferFacadeImpl : ITransferFacade
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ICashAccountDao cashAccountDao;
        private readonly ICreditAccountDao creditAccountDao;
        private readonly IActivityDao activityDao;

        public TransferFacadeImpl(ApplicationDbContext dbContext, ICashAccountDao cashAccountDao,
            ICreditAccountDao creditAccountDao, IActivityDao activityDao)
        {
            this.dbContext = dbContext;
            this.cashAccountDao = cashAccountDao;
            this.creditAccountDao = creditAccountDao;
            this.activityDao = activityDao;
        }

        public void CreateNewTransfer(Transfer transfer)
        {
            using var transaction = dbContext.Database.BeginTransaction();
            try
            {
                InsertTransfer(transfer);
                UpdateFromAccounts(transfer);
                UpdateToAccounts(transfer);
                dbContext.SaveChanges();
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        private void InsertTransfer(Transfer transfer)
        {
            new TransferDaoImpl(dbContext).InsertTransfer(transfer);
        }

        private void UpdateFromAccounts(Transfer transfer)
        {
            var actualAmount = cashAccountDao.GetFromAccountActualAmount(transfer.FromAccount);
            var amountTotal = actualAmount - (transfer.Amount + transfer.Fee);

            cashAccountDao.UpdateCashAccount(transfer.FromAccount, InsecureBankUtils.Round(amountTotal, 2));

            var amount = actualAmount - transfer.Amount;
            var amountWithFees = amount - transfer.Fee;

            var cashAccountId = cashAccountDao.GetIdFromNumber(transfer.FromAccount);

            creditAccountDao.UpdateCreditAccount(cashAccountId, InsecureBankUtils.Round(amountTotal, 2));

            var desc = transfer.Description.Length > 12
                ? transfer.Description.Substring(0, 12)
                : transfer.Description;
            activityDao.InsertNewActivity(transfer.Date, "TRANSFER: " + desc, transfer.FromAccount,
                -InsecureBankUtils.Round(transfer.Amount, 2), InsecureBankUtils.Round(amount, 2));
            activityDao.InsertNewActivity(transfer.Date, "TRANSFER FEE", transfer.FromAccount,
                -InsecureBankUtils.Round(transfer.Fee, 2), InsecureBankUtils.Round(amountWithFees, 2));
        }

        private void UpdateToAccounts(Transfer transfer)
        {
            var toCashAccountId = cashAccountDao.GetIdFromNumber(transfer.ToAccount);
            if (toCashAccountId <= 0)
            {
                return;
            }

            var actualAmount = cashAccountDao.GetFromAccountActualAmount(transfer.ToAccount);
            var amountTotal = actualAmount + transfer.Amount;
            creditAccountDao.UpdateCreditAccount(toCashAccountId, InsecureBankUtils.Round(amountTotal, 2));

            var desc = transfer.Description;
            activityDao.InsertNewActivity(transfer.Date, "TRANSFER: " + desc, transfer.ToAccount,
                InsecureBankUtils.Round(transfer.Amount, 2), InsecureBankUtils.Round(amountTotal, 2));
        }
    }
}