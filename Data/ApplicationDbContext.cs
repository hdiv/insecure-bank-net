﻿using insecure_bank_net.Bean;
using Microsoft.EntityFrameworkCore;

namespace insecure_bank_net.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet<CashAccount> Cashaccount { get; set; }
        public virtual DbSet<CreditAccount> Creditaccount { get; set; }
        public virtual DbSet<Transaction> Transaction { get; set; }
        public virtual DbSet<Transfer> Transfer { get; set; }
    }
}