using EcommerceApp.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<ProductoLoan> ProductosLoan { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 🧩 User
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<User>()
                .Property(u => u.Rol)
                .IsRequired();

            // 🧩 Producto
            modelBuilder.Entity<Producto>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Producto>()
                .Property(p => p.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Producto>()
                .Property(p => p.Precio)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Producto>()
                .HasOne(p => p.Empresa)
                .WithMany()
                .HasForeignKey(p => p.EmpresaId)
                .OnDelete(DeleteBehavior.Restrict);


            // ProductoLoan (Pedidos)
            modelBuilder.Entity<ProductoLoan>()
                .HasKey(pl => pl.Id);

            modelBuilder.Entity<ProductoLoan>()
                .HasOne(pl => pl.Producto)
                .WithMany()
                .HasForeignKey(pl => pl.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProductoLoan>()
                .HasOne(pl => pl.Cliente)
                .WithMany()
                .HasForeignKey(pl => pl.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);
            // ⚠️ evita el error de cascade multiple
        }
    }
}
