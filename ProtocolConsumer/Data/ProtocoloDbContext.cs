using Microsoft.EntityFrameworkCore;
using ProtocolConsumer.Models;

namespace ProtocolConsumer.Data
{
    public class ProtocoloDbContext : DbContext
    {
        public DbSet<Protocolo> Protocolos { get; set; }

        public ProtocoloDbContext(DbContextOptions<ProtocoloDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Protocolo>()
                .HasIndex(p => p.NumeroProtocolo)
                .HasDatabaseName("idx_numero_protocolo")
                .IsUnique();

            modelBuilder.Entity<Protocolo>()
                .HasIndex(p => new { p.Cpf, p.NumeroVia })
                .HasDatabaseName("idx_cpf_numero_via")
                .IsUnique();

            modelBuilder.Entity<Protocolo>()
                .HasIndex(p => new { p.Rg, p.NumeroVia })
                .HasDatabaseName("idx_rg_numero_via")
                .IsUnique();
        }
    }
}