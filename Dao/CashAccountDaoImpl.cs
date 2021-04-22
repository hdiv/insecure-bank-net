using System.Collections.Generic;
using System.Linq;
using insecure_bank_net.Bean;
using insecure_bank_net.Data;
using Microsoft.EntityFrameworkCore;

namespace insecure_bank_net.Dao
{
    public class CashAccountDaoImpl : ICashAccountDao
    {
        private ApplicationDbContext dbContext;

        public CashAccountDaoImpl(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        
        public List<CashAccount> FindCashAccountsByUsername(string username) {

            var str = "select * from cashaccount  where username='" + username + "'";
            return dbContext.Cashaccount.FromSqlRaw(str).ToList();
        }

        public double GetFromAccountActualAmount(string fromAccount) {

            var sql = "SELECT * FROM cashaccount WHERE number = {0}";
            var account = dbContext.Cashaccount.FromSqlRaw(sql, fromAccount).Single();
            return account.Availablebalance;

        }

        public int UpdateCashAccount(string fromAccount, double amountTotal) {

            var sql = "UPDATE cashaccount SET availablebalance= {0} WHERE number = {1}";
            return dbContext.Database.ExecuteSqlRaw(sql, amountTotal, fromAccount);
        }

        public int GetIdFromNumber(string fromAccount)
        {
            var sql = "SELECT * FROM cashaccount WHERE number = {0}";
            var account = dbContext.Cashaccount.FromSqlRaw(sql, fromAccount).ToList().FirstOrDefault();
            return account?.Id ?? 0;
        }

    }
}