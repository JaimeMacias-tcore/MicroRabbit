﻿using Microsoft.EntityFrameworkCore;
using MicroRabbit.Transfer.Domain.Models;

namespace MicroRabbit.Transfer.Data.Context
{
    public class TransferDbContext : DbContext
    {
        public TransferDbContext(DbContextOptions<TransferDbContext> options)
            : base(options)
        {

        }

        public DbSet<TransferLog> Transfers { get; set; }
    }
}
