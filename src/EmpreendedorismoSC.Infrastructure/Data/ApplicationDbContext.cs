using EmpreendedorismoSC.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmpreendedorismoSC.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Empreendimento> Empreendimentos => Set<Empreendimento>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Empreendimento>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.NomeEmpreendimento)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.NomeEmpreendedor)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Municipio)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.SegmentoAtuacao)
                .IsRequired();

            entity.Property(e => e.Contato)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Ativo)
                .IsRequired();

            entity.Property(e => e.DataCriacao)
                .IsRequired();
        });
    }
}
