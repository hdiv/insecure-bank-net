using System.ComponentModel.DataAnnotations;

namespace insecure_bank_net.Bean
{
    public class Account
    {
        [Key]
        public string Username { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Password { get; set; }
    }
}
