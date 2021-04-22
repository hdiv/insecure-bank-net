using System.Collections.Generic;
using System.Threading.Tasks;
using insecure_bank_net.Bean;

namespace insecure_bank_net.Dao
{
    public interface IAccountDao
    {
        List<Account> FindUsersByUsernameAndPassword(string username, string password);

        List<Account> FindUsersByUsername(string username);

        List<Account> FindAllUsers();
    }
}