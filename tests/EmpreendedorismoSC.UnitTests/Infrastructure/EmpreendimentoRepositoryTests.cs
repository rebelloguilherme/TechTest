using EmpreendedorismoSC.Domain.Entities;
using EmpreendedorismoSC.Domain.Enums;
using EmpreendedorismoSC.Infrastructure.Data;
using EmpreendedorismoSC.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace EmpreendedorismoSC.UnitTests.Infrastructure;

public class EmpreendimentoRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly EmpreendimentoRepository _repository;

    public EmpreendimentoRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new EmpreendimentoRepository(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    // ===== AddAsync =====

    [Fact]
    public async Task AddAsync_DeveAdicionarEmpreendimento()
    {
        // Arrange
        var empreendimento = CriarEmpreendimento("Tech SC", "Florianópolis", SegmentoAtuacao.Tecnologia);

        // Act
        var result = await _repository.AddAsync(empreendimento);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBe(Guid.Empty);

        var saved = await _context.Empreendimentos.FindAsync(result.Id);
        saved.Should().NotBeNull();
        saved!.NomeEmpreendimento.Should().Be("Tech SC");
    }

    // ===== GetAllAsync =====

    [Fact]
    public async Task GetAllAsync_SemFiltros_DeveRetornarTodos()
    {
        // Arrange
        await SeedData();

        // Act
        var (items, totalCount) = await _repository.GetAllAsync();

        // Assert
        totalCount.Should().Be(3);
        items.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetAllAsync_FiltroPorMunicipio_DeveRetornarFiltrados()
    {
        // Arrange
        await SeedData();

        // Act
        var (items, totalCount) = await _repository.GetAllAsync(municipio: "Florianópolis");

        // Assert
        totalCount.Should().Be(1);
        items.Should().HaveCount(1);
        items.First().Municipio.Should().Be("Florianópolis");
    }

    [Fact]
    public async Task GetAllAsync_FiltroPorSegmento_DeveRetornarFiltrados()
    {
        // Arrange
        await SeedData();

        // Act
        var (items, totalCount) = await _repository.GetAllAsync(segmentoAtuacao: SegmentoAtuacao.Tecnologia);

        // Assert
        totalCount.Should().Be(1);
        items.First().SegmentoAtuacao.Should().Be(SegmentoAtuacao.Tecnologia);
    }

    [Fact]
    public async Task GetAllAsync_FiltroPorStatus_DeveRetornarFiltrados()
    {
        // Arrange
        await SeedData();

        // Act
        var (items, totalCount) = await _repository.GetAllAsync(ativo: true);

        // Assert
        totalCount.Should().Be(2);
        items.Should().AllSatisfy(e => e.Ativo.Should().BeTrue());
    }

    [Fact]
    public async Task GetAllAsync_BuscaTextual_DeveRetornarCorrespondentes()
    {
        // Arrange
        await SeedData();

        // Act
        var (items, totalCount) = await _repository.GetAllAsync(busca: "Tech");

        // Assert
        totalCount.Should().Be(1);
        items.First().NomeEmpreendimento.Should().Contain("Tech");
    }

    [Fact]
    public async Task GetAllAsync_Paginacao_DeveRetornarPaginaCorreta()
    {
        // Arrange
        await SeedData();

        // Act
        var (items, totalCount) = await _repository.GetAllAsync(pagina: 1, tamanhoPagina: 2);

        // Assert
        totalCount.Should().Be(3);
        items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAllAsync_SegundaPagina_DeveRetornarRestante()
    {
        // Arrange
        await SeedData();

        // Act
        var (items, totalCount) = await _repository.GetAllAsync(pagina: 2, tamanhoPagina: 2);

        // Assert
        totalCount.Should().Be(3);
        items.Should().HaveCount(1);
    }

    // ===== GetByIdAsync =====

    [Fact]
    public async Task GetByIdAsync_QuandoExiste_DeveRetornar()
    {
        // Arrange
        var empreendimento = CriarEmpreendimento("Tech SC", "Florianópolis", SegmentoAtuacao.Tecnologia);
        await _repository.AddAsync(empreendimento);

        // Act
        var result = await _repository.GetByIdAsync(empreendimento.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(empreendimento.Id);
    }

    [Fact]
    public async Task GetByIdAsync_QuandoNaoExiste_DeveRetornarNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    // ===== UpdateAsync =====

    [Fact]
    public async Task UpdateAsync_DeveAtualizarDados()
    {
        // Arrange
        var empreendimento = CriarEmpreendimento("Antigo", "Florianópolis", SegmentoAtuacao.Tecnologia);
        await _repository.AddAsync(empreendimento);

        // Act
        empreendimento.NomeEmpreendimento = "Novo Nome";
        empreendimento.Municipio = "Joinville";
        var result = await _repository.UpdateAsync(empreendimento);

        // Assert
        result.NomeEmpreendimento.Should().Be("Novo Nome");
        result.Municipio.Should().Be("Joinville");

        var updated = await _context.Empreendimentos.FindAsync(empreendimento.Id);
        updated!.NomeEmpreendimento.Should().Be("Novo Nome");
    }

    // ===== DeleteAsync =====

    [Fact]
    public async Task DeleteAsync_QuandoExiste_DeveRemoverERetornarTrue()
    {
        // Arrange
        var empreendimento = CriarEmpreendimento("Tech SC", "Florianópolis", SegmentoAtuacao.Tecnologia);
        await _repository.AddAsync(empreendimento);

        // Act
        var result = await _repository.DeleteAsync(empreendimento.Id);

        // Assert
        result.Should().BeTrue();
        var deleted = await _context.Empreendimentos.FindAsync(empreendimento.Id);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_QuandoNaoExiste_DeveRetornarFalse()
    {
        // Act
        var result = await _repository.DeleteAsync(Guid.NewGuid());

        // Assert
        result.Should().BeFalse();
    }

    // ===== Helpers =====

    private async Task SeedData()
    {
        var empreendimentos = new List<Empreendimento>
        {
            CriarEmpreendimento("Tech SC Innovation", "Florianópolis", SegmentoAtuacao.Tecnologia, true),
            CriarEmpreendimento("Agro Joinville", "Joinville", SegmentoAtuacao.Agronegocio, true),
            CriarEmpreendimento("Comércio Inativo", "Blumenau", SegmentoAtuacao.Comercio, false)
        };

        _context.Empreendimentos.AddRange(empreendimentos);
        await _context.SaveChangesAsync();
    }

    private static Empreendimento CriarEmpreendimento(string nome, string municipio, SegmentoAtuacao segmento, bool ativo = true)
    {
        return new Empreendimento
        {
            Id = Guid.NewGuid(),
            NomeEmpreendimento = nome,
            NomeEmpreendedor = "Empreendedor Teste",
            Municipio = municipio,
            SegmentoAtuacao = segmento,
            Contato = "contato@teste.com",
            Ativo = ativo,
            DataCriacao = DateTime.UtcNow
        };
    }
}
