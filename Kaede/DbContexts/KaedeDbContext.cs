﻿using Kaede.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaede.DbContexts
{
    public class KaedeDbContext : DbContext
    {
        #region Tables
        public DbSet<User> Users { get; set; }
        public DbSet<ShopItem> ShopItems { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        #endregion

        #region Constructor
        public KaedeDbContext(DbContextOptions options) : base(options) { }
        #endregion

        #region Methods
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.UsernameHash)
                .IsUnique();
            modelBuilder.Entity<ShopItem>()
                .HasIndex(i => i.Name)
                .IsUnique();
        }

        public override int SaveChanges()
        {
            ValidateEntities();
            User.OnDataSaving(ChangeTracker.Entries());
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ValidateEntities();
            User.OnDataSaving(ChangeTracker.Entries());
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void ValidateEntities()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity != null && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var context = new ValidationContext(entry.Entity);
                var results = new List<ValidationResult>();

                if (!Validator.TryValidateObject(entry.Entity, context, results, true))
                {
                    var errorMessage = string.Join(Environment.NewLine, results.Select(r => r.ErrorMessage));
                    throw new ValidationException($"Validation failed: {errorMessage}");
                }
            }
        }

        #endregion
    }
}
