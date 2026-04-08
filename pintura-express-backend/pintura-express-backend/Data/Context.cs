using Microsoft.EntityFrameworkCore;
using pintura_express_backend.Models;

namespace pintura_express_backend.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }

        public DbSet<Producto> Productos => Set<Producto>();
        public DbSet<Orden> Ordenes => Set<Orden>();
        public DbSet<OrdenItem> OrdenItems => Set<OrdenItem>();
    }
}
