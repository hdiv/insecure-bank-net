namespace insecure_bank_net.Bean
{
    public partial class CreditAccount
    {
        public long Id { get; set; }
        public string Number { get; set; }
        public string Username { get; set; }
        public string Description { get; set; }
        public double? Availablebalance { get; set; }
        public long? Cashaccountid { get; set; }
    }
}
