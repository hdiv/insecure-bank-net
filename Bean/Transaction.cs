using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace insecure_bank_net.Bean
{
    public partial class Transaction
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Number { get; set; }
        public double? Amount { get; set; }
        public double? Availablebalance { get; set; }
    }
}
