using System.Collections.Generic;
using System.Threading.Tasks;
using insecure_bank_net.Bean;

namespace insecure_bank_net.Dao
{
    public interface ICreditAccountDao
    {
        List<CreditAccount> FindCreditAccountsByUsername(string username);

        int UpdateCreditAccount(int cashAccountId, double round);
    }
}