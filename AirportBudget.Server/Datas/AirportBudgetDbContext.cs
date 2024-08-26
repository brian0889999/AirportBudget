using AirportBudget.Server.Enums;
using AirportBudget.Server.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection.Emit;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;


namespace AirportBudget.Server.Datas
{
    public class AirportBudgetDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AirportBudgetDbContext> _logger;
        public AirportBudgetDbContext(DbContextOptions<AirportBudgetDbContext> options, IHttpContextAccessor httpContextAccessor, ILogger<AirportBudgetDbContext> logger)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }
        public DbSet<User> User { get; set; }
        public DbSet<RolePermission> RolePermission { get; set; }
        public DbSet<Group> Group { get; set; }
        public DbSet<Budget> Budget { get; set; }
        //public DbSet<Subject6> Subject6 { get; set; }
        //public DbSet<Subject7> Subject7 { get; set; }
        //public DbSet<Subject8> Subject8 { get; set; }
        public DbSet<BudgetAmount> BudgetAmount { get; set; }
        public DbSet<EntityLog> EntityLog { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<EntityLog>()
            .HasIndex(b => b.EntityLogId)
            .HasDatabaseName("IX_EntityLogId");  //加索引可以加快這類查詢的速度

            //modelBuilder.Entity<EntityLog>()
            //  .HasOne(log => log.BudgetAmount)
            //  .WithMany(ba => ba.BudgetAmountLogs)
            //  .HasForeignKey(log => log.BudgetAmountId)
            //  .OnDelete(DeleteBehavior.Restrict); // 避免外鍵刪除問題
        }

        
    }
}