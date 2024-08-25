using InventoryApp.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Data
{
    public class InventoryAppDbContext : DbContext
    {
        public InventoryAppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<SalesOrder> SalesOrders { get; set; }
    }
}
