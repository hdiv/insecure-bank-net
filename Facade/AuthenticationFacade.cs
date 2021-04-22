using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using insecure_bank_net.Bean;
using insecure_bank_net.Dao;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace insecure_bank_net.Facade
{
    public class AuthenticationFacade : IAuthenticationFacade
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IAccountDao accountDao;

        public AuthenticationFacade(IAccountDao accountDao, IHttpContextAccessor httpContextAccessor)
        {
            this.accountDao = accountDao;
            this.httpContextAccessor = httpContextAccessor;
        }

        public IPrincipal GetPrincipal()
        {
            var username = httpContextAccessor.HttpContext.User.Identity.Name;
            var user = accountDao.FindUsersByUsername(username).FirstOrDefault();
            if (user == null)
            {
                throw new Exception("User is not logged in");
            }
            return BuildPrincipal(user);
        }

        public async Task<IPrincipal> SignInUser(string login, string password)
        {
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                return null;
            }

            var accounts = accountDao.FindUsersByUsernameAndPassword(login, password);
            var account = accounts.FirstOrDefault();
            if (account == null)
            {
                return null;
            }

            var principal = BuildPrincipal(account);

            await httpContextAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(principal),
                new AuthenticationProperties
                {
                    AllowRefresh = true,
                    IssuedUtc = DateTimeOffset.Now,
                    ExpiresUtc = DateTimeOffset.Now.AddHours(1),
                    IsPersistent = true
                });
            return principal;
        }

        public async Task SignOutUser()
        {
            await httpContextAccessor.HttpContext.SignOutAsync();
        }

        private IPrincipal BuildPrincipal(Account account)
        {
            var identity = new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.Name, account.Username)
            }, CookieAuthenticationDefaults.AuthenticationScheme);
            return new GenericPrincipal(identity, new[] {account.Name == "john" ? "ROLE_ADMIN" : "ROLE_USER"});
        }
    }
}