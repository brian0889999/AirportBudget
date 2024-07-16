using AirportBudget.Server.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection.Emit;


namespace AirportBudget.Server.Datas
{
    public class AirportBudgetDbContext : DbContext
    {
        public AirportBudgetDbContext(DbContextOptions<AirportBudgetDbContext> options) : base(options)
        {
        }
        public DbSet<User> User {  get; set; }
        public DbSet<RolePermission> RolePermission { get; set; }
        public DbSet<Group> Group { get; set; }
        public DbSet<Budget> Budget { get; set; }
        public DbSet<Subject6> Subject6 { get; set; }
        public DbSet<Subject7> Subject7 { get; set; }
        public DbSet<Subject8> Subject8 { get; set; }
        public DbSet<BudgetAmount> BudgetAmount { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        optionsBuilder.UseSqlServer("Money;MultipleActiveResultSets=True;");
        //    }
        //}
    }
}