using System;
using System.Collections.Generic;
using System.Linq;
using insecure_bank_net.Bean;
using insecure_bank_net.Data;
using Microsoft.EntityFrameworkCore;

namespace insecure_bank_net.Dao
{
    public class ActivityDaoImpl : IActivityDao
    {
        private ApplicationDbContext dbContext;

        public ActivityDaoImpl(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public List<Transaction> FindTransactionsByCashAccountNumber(string number)
        {
            var sql = "SELECT * FROM [transaction] WHERE number = '" + number + "'";
            return dbContext.Transaction.FromSqlRaw(sql).ToList();
        }

        public int InsertNewActivity(DateTime date, string description, string number, double amount, double availablebalance)
        {
            var sql = "INSERT INTO [transaction] (date, description, number, amount, availablebalance) VALUES ({0}, {1}, {2}, {3}, {4})";
            return dbContext.Database.ExecuteSqlRaw(sql, date, description, number, amount, availablebalance);
        }
    }
}