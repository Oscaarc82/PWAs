using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PWAs.Models;
using PWAs.Models.Conexion;
using PWAs.Models.Geolocalizacion;
using PWAs.Models.Reportes;
using PWAs.Models.Usuarios;

namespace PWAs.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> tUsuarios { get; set; }
        public DbSet<TokenTemp> tTokensTemp { get; set; }
        public DbSet<Rol> tRoles { get; set; }
        public DbSet<Producto> tProducto { get; set; }
        public DbSet<Compra> tCompra { get; set; }
        public DbSet<CompraDetalles> tCompraDetalles { get; set; }
        public DbSet<LocationRecord> tLocationRecords { get; set; }
        public DbSet<Sesiones> tSesiones { get; set; }
        public DbSet<Proveedor> tProveedores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Compra>(entity =>
            {
                entity.HasMany(c => c.Detalles)
                      .WithOne()
                      .HasForeignKey("iCompra");

                entity.HasKey(c => c.IId);
                entity.Property(c => c.IId).ValueGeneratedOnAdd();
            });

            // Configuración para Proveedor
            modelBuilder.Entity<Proveedor>(entity =>
            {
                entity.HasKey(p => p.Iid);
                entity.Property(p => p.Iid).ValueGeneratedOnAdd();

                entity.Property(p => p.SNombre)
                    .IsRequired()
                    .HasMaxLength(90);

                entity.Property(p => p.STelefono)
                    .IsRequired()
                    .HasMaxLength(15);

                entity.Property(p => p.SContacto)
                    .IsRequired()
                    .HasMaxLength(60);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}