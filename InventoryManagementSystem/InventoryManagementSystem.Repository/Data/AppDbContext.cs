using InventoryManagementSystem.Repository.Models;
using InventoryManagementSytem.Common.Enums;
using Microsoft.EntityFrameworkCore;
using Npgsql.NameTranslation;

namespace InventoryManagementSystem.Repository.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<RoleDb> Roles { get; set; }
        public DbSet<UserDb> Users { get; set; }
        public DbSet<GenericStatusDb> Status { get; set; }
        public DbSet<UserKycDb> UserKycs { get; set; }
        public DbSet<WarehouseDb> Warehouses { get; set; }
        public DbSet<SupplierDb> Suppliers { get; set; }
        public DbSet<StoneDb> Stones { get; set; }
        public DbSet<SaleOrderDb> SaleOrders { get; set; }
        public DbSet<SaleOrderItemDb> SaleOrderItems { get; set; }
        public DbSet<PurchaseOrderDb> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderItemDb> PurchaseOrderItems { get; set; }
        public DbSet<PurityDb> Purities { get; set; }
        public DbSet<MetalDb> Metals { get; set; }
        public DbSet<JewelleryItemDb> JewelleryItems { get; set; }
        public DbSet<ItemStoneDb> ItemStones { get; set; }
        public DbSet<ItemStockDb> ItemStocks { get; set; }
        public DbSet<PaymentDb> Payments { get; set; }
        public DbSet<InventoryTransactionDb> InventoryTransactions { get; set; }
        public DbSet<MetalRateHistoryDb> MetalRateHistories { get; set; }
        public DbSet<StoneRateHistoryDb> StoneRateHistories { get; set; }
        public DbSet<ExchangeOrderDb> ExchangeOrders { get; set; }
        public DbSet<ExchangeItemDb> ExchangeItems { get; set; }
        public DbSet<InvoiceDb> Invoices { get; set; }
        public DbSet<InvoiceItemDb> InvoiceItems { get; set; }
        public DbSet<InvoicePaymentDb> InvoicePayments { get; set; }
        public DbSet<TcsTransactionDb> TcsTransactions { get; set; }
        public DbSet<CategoryDb> Categories { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure the enum type in the public schema with snake_case name translation
            modelBuilder.HasPostgresEnum<MakingChargeType>(
                "public",
                "making_charge_type",
                new NpgsqlSnakeCaseNameTranslator()
            );

            // Configure GenericStatusDb relationships
            modelBuilder.Entity<GenericStatusDb>()
                .HasOne(g => g.CreatedByUser)
                .WithMany()
                .HasForeignKey(g => g.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GenericStatusDb>()
                .HasOne(g => g.UpdatedByUser)
                .WithMany()
                .HasForeignKey(g => g.UpdatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure UserDb relationships
            modelBuilder.Entity<UserDb>()
                .HasOne(u => u.CreatedByUser)
                .WithMany()
                .HasForeignKey(u => u.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserDb>()
                .HasOne(u => u.UpdatedByUser)
                .WithMany()
                .HasForeignKey(u => u.UpdatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure RoleDb relationships
            modelBuilder.Entity<RoleDb>()
                .HasOne(r => r.Status)
                .WithMany()
                .HasForeignKey(r => r.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }

    }
}
