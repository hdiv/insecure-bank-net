using System.Collections.Generic;
using System.Threading.Tasks;
using insecure_bank_net.Bean;

namespace insecure_bank_net.Dao
{
    public interface ICashAccountDao
    {
        List<CashAccount> FindCashAccountsByUsername(string username);

        double GetFromAccountActualAmount(string fromAccount);

        int UpdateCashAccount(string fromAccount, double amountTotal);

        int GetIdFromNumber(string fromAccount);
    }
}