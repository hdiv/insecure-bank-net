using System.Security.Principal;
using System.Threading.Tasks;

namespace insecure_bank_net.Facade
{
    public interface IAuthenticationFacade
    {
        Task<IPrincipal> SignInUser(string login, string password);
        
        Task SignOutUser();

        IPrincipal GetPrincipal();
    }
}