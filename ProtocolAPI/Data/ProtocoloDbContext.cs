using Microsoft.EntityFrameworkCore;
using ProtocolAPI.Models;

namespace ProtocolAPI.Data
{
    public class ProtocoloDbContext : DbContext
    {
        public ProtocoloDbContext(DbContextOptions<ProtocoloDbContext> options) : base(options) { }

        public DbSet<Protocolo> Protocolos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Protocolo>()
                .HasIndex(p => p.NumeroProtocolo)
                .IsUnique();

            modelBuilder.Entity<Protocolo>()
                .HasIndex(p => new { p.Cpf, p.NumeroVia })
                .IsUnique();
        }
    }
}