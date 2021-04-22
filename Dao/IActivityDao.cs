using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using insecure_bank_net.Bean;

namespace insecure_bank_net.Dao
{
    public interface IActivityDao
    {
        List<Transaction> FindTransactionsByCashAccountNumber(string number);

        int InsertNewActivity(DateTime date, string description, string number, double amount, double availablebalance);
    }
}