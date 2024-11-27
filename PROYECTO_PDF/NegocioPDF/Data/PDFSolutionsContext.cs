using Microsoft.EntityFrameworkCore;
using NegocioPDF.Models;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace NegocioPDF.Data
{
    public class PDFSolutionsContext : DbContext
    {
        public PDFSolutionsContext(DbContextOptions<PDFSolutionsContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<OperacionPDF> OperacionesPDF { get; set; }
        public DbSet<DetalleSuscripcion> DetallesSuscripcion { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración de Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuarios");
        entity.HasKey(e => e.Id);
                entity.Property(e => e.Correo).IsRequired();
                entity.Property(e => e.Password).IsRequired();
            });

            // Configuración de OperacionPDF
            modelBuilder.Entity<OperacionPDF>(entity =>
            {
                 entity.ToTable("operaciones_pdf");
        entity.HasKey(e => e.Id);
                entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");
                entity.Property(e => e.TipoOperacion).HasColumnName("TipoOperacion");
                entity.Property(e => e.FechaOperacion).HasColumnName("fecha_operacion");
                
                entity.HasOne<Usuario>()
                    .WithMany()
                    .HasForeignKey(e => e.UsuarioId);
            });

            // Configuración de DetalleSuscripcion
           modelBuilder.Entity<DetalleSuscripcion>(entity =>
    {
        entity.ToTable("detalles_suscripciones");
        entity.Property(e => e.precio)
            .HasPrecision(10, 2); // 10 dígitos en total, 2 decimales
        entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");
        entity.Property(e => e.tipo_suscripcion).HasColumnName("tipo_suscripcion");
        entity.Property(e => e.fecha_inicio).HasColumnName("fecha_inicio");
        entity.Property(e => e.fecha_final).HasColumnName("fecha_final");
        entity.Property(e => e.operaciones_realizadas).HasColumnName("operaciones_realizadas");
        
        entity.HasOne(d => d.Usuario)
            .WithMany()
            .HasForeignKey(d => d.UsuarioId);
            });
        }
     

        
    }
}