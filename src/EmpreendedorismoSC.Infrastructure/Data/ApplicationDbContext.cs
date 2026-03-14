using EmpreendedorismoSC.Domain.Entities;
using EmpreendedorismoSC.Domain.Enums;
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
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.Property(e => e.Contato)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Ativo)
                .IsRequired();

            entity.Property(e => e.DataCriacao)
                .IsRequired();

            // Índices para queries frequentes
            entity.HasIndex(e => e.Municipio);
            entity.HasIndex(e => e.SegmentoAtuacao);
            entity.HasIndex(e => e.Ativo);
        });

        // Seed data — empreendimentos de exemplo em SC
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Empreendimento>().HasData(
            new Empreendimento
            {
                Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
                NomeEmpreendimento = "Tech Floripa Hub",
                NomeEmpreendedor = "Marina Costa",
                Municipio = "Florianópolis",
                SegmentoAtuacao = SegmentoAtuacao.Tecnologia,
                Contato = "marina@techfloripahub.com.br",
                Ativo = true,
                DataCriacao = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc)
            },
            new Empreendimento
            {
                Id = Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f12345678901"),
                NomeEmpreendimento = "Comércio do Vale",
                NomeEmpreendedor = "Roberto Schneider",
                Municipio = "Blumenau",
                SegmentoAtuacao = SegmentoAtuacao.Comercio,
                Contato = "roberto@comerciodovale.com.br",
                Ativo = true,
                DataCriacao = new DateTime(2024, 2, 20, 14, 30, 0, DateTimeKind.Utc)
            },
            new Empreendimento
            {
                Id = Guid.Parse("c3d4e5f6-a7b8-9012-cdef-123456789012"),
                NomeEmpreendimento = "Indústria Joinville Têxtil",
                NomeEmpreendedor = "Ana Müller",
                Municipio = "Joinville",
                SegmentoAtuacao = SegmentoAtuacao.Industria,
                Contato = "ana@joinvilletextil.com.br",
                Ativo = true,
                DataCriacao = new DateTime(2024, 3, 10, 9, 0, 0, DateTimeKind.Utc)
            },
            new Empreendimento
            {
                Id = Guid.Parse("d4e5f6a7-b8c9-0123-defa-234567890123"),
                NomeEmpreendimento = "Consultoria Serra Catarinense",
                NomeEmpreendedor = "Pedro Souza",
                Municipio = "Lages",
                SegmentoAtuacao = SegmentoAtuacao.Servicos,
                Contato = "pedro@consultoriaserra.com.br",
                Ativo = false,
                DataCriacao = new DateTime(2024, 4, 5, 16, 0, 0, DateTimeKind.Utc)
            },
            new Empreendimento
            {
                Id = Guid.Parse("e5f6a7b8-c9d0-1234-efab-345678901234"),
                NomeEmpreendimento = "Agro Oeste SC",
                NomeEmpreendedor = "Carla Fernandes",
                Municipio = "Chapecó",
                SegmentoAtuacao = SegmentoAtuacao.Agronegocio,
                Contato = "carla@agrooestesc.com.br",
                Ativo = true,
                DataCriacao = new DateTime(2024, 5, 12, 8, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
