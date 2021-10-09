using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using insecure_bank_net.Bean;
using insecure_bank_net.Dao;
using insecure_bank_net.Facade;
using insecure_bank_net.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace insecure_bank_net.Pages.User
{
    public class UserDetail : PageModel
    {
        private static readonly char SP = Path.DirectorySeparatorChar;
        private readonly IWebHostEnvironment environment;
        private readonly IAuthenticationFacade authenticationFacade;
        private readonly IAccountDao accountDao;
        private readonly ICreditAccountDao creditAccountDao;
        private readonly IStorageFacade storageFacade;

        public UserDetail(IWebHostEnvironment environment, IAuthenticationFacade authenticationFacade,
            IStorageFacade storageFacade,
            IAccountDao accountDao, ICreditAccountDao creditAccountDao)
        {
            this.environment = environment;
            this.authenticationFacade = authenticationFacade;
            this.storageFacade = storageFacade;
            this.accountDao = accountDao;
            this.creditAccountDao = creditAccountDao;
        }

        public Bean.Account Account { get; set; }

        public List<CreditAccount> CreditAccounts { get; set; }

        [BindProperty] public string Username { get; set; }

        [BindProperty] public string UsernameMalicious { get; set; }

        [BindProperty] public IFormFile Upload { get; set; }

        public void OnGet(string username)
        {
            var account = accountDao.FindUsersByUsername(username).First();
            var creditAccounts = creditAccountDao.FindCreditAccountsByUsername(account.Username);
            Account = account;
            Username = account.Username;
            UsernameMalicious = account.Username;
            CreditAccounts = creditAccounts;
        }

        public IActionResult OnGetCreditCardImage(string name)
        {
            var stream = new FileStream(environment.WebRootPath + $"{SP}img{SP}creditCards{SP}{name}", FileMode.Open);
            return File(stream, "image/png", name);
        }

        public IActionResult OnGetAvatar(string name)
        {
            var image = $"{SP}img{SP}avatars{SP}{name}";
            var file = storageFacade.Exists(image)
                ? storageFacade.Load(image)
                : storageFacade.Load($"{SP}img{SP}avatars{SP}avatar.png");
            return File(file, "image/png", name);
        }

        public IActionResult OnPostCertificate(string username)
        {
            var account = accountDao.FindUsersByUsername(username).First();
            var tmpFile = Path.GetTempFileName();
            var stream = new FileStream(tmpFile, FileMode.Create);
            var fos = new BinaryFormatter();
            fos.Serialize(stream, new FileUntrustedValid(account.Name));
            stream.Seek(0L, SeekOrigin.Begin);
            return File(stream, "application/octet-stream", $"Certificate_{account.Name}.jks");
        }

        public IActionResult OnPostMaliciousCertificate(string usernameMalicious)
        {
            var account = accountDao.FindUsersByUsername(usernameMalicious).First();
            var tmpFile = Path.GetTempFileName();
            var stream = new FileStream(tmpFile, FileMode.Create);
            var fos = new BinaryFormatter();
            fos.Serialize(stream, new FileUntrusted(account.Name));
            stream.Seek(0L, SeekOrigin.Begin);
            return File(stream, "application/octet-stream", $"MaliciousCertificate_{account.Name}.jks");
        }

        public IActionResult OnPostNewCertificate()
        {
            if (Upload != null && Upload.Length > 0)
            {
                var formatter = new BinaryFormatter();
                formatter.Deserialize(Upload.OpenReadStream());
                return Content("<p>File '" + Upload.FileName + "' uploaded successfully</p>", "text/html");
            }
            else
            {
                return Content("<p>No file uploaded", "text/html");
            }
        }

        public IActionResult OnPostUpdate()
        {
            var principal = authenticationFacade.GetPrincipal();
            var username = principal.Identity!.Name;
            if (Upload != null && Upload.Length > 0)
            {
                storageFacade.Save(Upload.OpenReadStream(), $"{SP}img{SP}avatars{SP}{username}.png");
            }

            return RedirectToPage("/User/UserDetail", new Dictionary<string, string> {{"username", username}});
        }
    }
}