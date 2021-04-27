using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace insecure_bank_net.Bean
{
    public partial class Transfer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string FromAccount { get; set; }
        public string ToAccount { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public double Fee { get; set; } = 20.0;
        public string Username { get; set; }
        public DateTime Date { get; set; }
    }
}
