using AssesmentOnlineAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AssesmentOnlineAPI.Data
{
    public class TransferAccountDBContext: DbContext
    {
        public TransferAccountDBContext(DbContextOptions<TransferAccountDBContext> options): base(options) { }

        public DbSet<User> Users { get; set; }        
        public DbSet<TransactionLog> TransactionLogs { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>()
                .HasIndex(x => x.Username)
                .IsUnique();
        } 
    }
}
