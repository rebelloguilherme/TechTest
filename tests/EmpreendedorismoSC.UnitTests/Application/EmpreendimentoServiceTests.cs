using EmpreendedorismoSC.Application.DTOs;
using EmpreendedorismoSC.Application.Services;
using EmpreendedorismoSC.Application.Validators;
using EmpreendedorismoSC.Domain.Entities;
using EmpreendedorismoSC.Domain.Enums;
using EmpreendedorismoSC.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace EmpreendedorismoSC.UnitTests.Application;

public class EmpreendimentoServiceTests
{
    private readonly Mock<IEmpreendimentoRepository> _repositoryMock;
    private readonly EmpreendimentoService _service;

    public EmpreendimentoServiceTests()
    {
        _repositoryMock = new Mock<IEmpreendimentoRepository>();
        var createValidator = new CreateEmpreendimentoValidator();
        var updateValidator = new UpdateEmpreendimentoValidator();
        _service = new EmpreendimentoService(_repositoryMock.Object, createValidator, updateValidator);
    }

    // ===== GetAllAsync =====

    [Fact]
    public async Task GetAllAsync_DeveRetornarListaPaginada()
    {
        // Arrange
        var empreendimentos = new List<Empreendimento>
        {
            CriarEmpreendimentoValido("Tech SC", "Florianópolis", SegmentoAtuacao.Tecnologia),
            CriarEmpreendimentoValido("Agro SC", "Joinville", SegmentoAtuacao.Agronegocio)
        };

        _repositoryMock
            .Setup(r => r.GetAllAsync(null, null, null, null, 1, 10))
            .ReturnsAsync((empreendimentos.AsEnumerable(), 2));

        var filter = new EmpreendimentoFilterDto();

        // Act
        var result = await _service.GetAllAsync(filter);

        // Assert
        result.Items.Should().HaveCount(2);
        result.TotalItems.Should().Be(2);
        result.Pagina.Should().Be(1);
        result.TamanhoPagina.Should().Be(10);
    }

    [Fact]
    public async Task GetAllAsync_DevePassarFiltrosParaRepositorio()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetAllAsync("Florianópolis", SegmentoAtuacao.Tecnologia, true, null, 1, 10))
            .ReturnsAsync((Enumerable.Empty<Empreendimento>(), 0));

        var filter = new EmpreendimentoFilterDto
        {
            Municipio = "Florianópolis",
            SegmentoAtuacao = SegmentoAtuacao.Tecnologia,
            Ativo = true
        };

        // Act
        await _service.GetAllAsync(filter);

        // Assert
        _repositoryMock.Verify(r => r.GetAllAsync("Florianópolis", SegmentoAtuacao.Tecnologia, true, null, 1, 10), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_DeveLimitarTamanhoPaginaEm50()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetAllAsync(null, null, null, null, 1, 50))
            .ReturnsAsync((Enumerable.Empty<Empreendimento>(), 0));

        var filter = new EmpreendimentoFilterDto { TamanhoPagina = 100 };

        // Act
        var result = await _service.GetAllAsync(filter);

        // Assert
        result.TamanhoPagina.Should().Be(50);
        _repositoryMock.Verify(r => r.GetAllAsync(null, null, null, null, 1, 50), Times.Once);
    }

    // ===== GetByIdAsync =====

    [Fact]
    public async Task GetByIdAsync_QuandoExiste_DeveRetornarDto()
    {
        // Arrange
        var id = Guid.NewGuid();
        var empreendimento = CriarEmpreendimentoValido("Tech SC", "Florianópolis", SegmentoAtuacao.Tecnologia);
        empreendimento.Id = id;

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(empreendimento);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
        result.NomeEmpreendimento.Should().Be("Tech SC");
    }

    [Fact]
    public async Task GetByIdAsync_QuandoNaoExiste_DeveRetornarNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Empreendimento?)null);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        result.Should().BeNull();
    }

    // ===== CreateAsync =====

    [Fact]
    public async Task CreateAsync_ComDadosValidos_DeveCriarEmpreendimento()
    {
        // Arrange
        var dto = new CreateEmpreendimentoDto
        {
            NomeEmpreendimento = "Tech SC Innovation",
            NomeEmpreendedor = "Maria Silva",
            Municipio = "Florianópolis",
            SegmentoAtuacao = SegmentoAtuacao.Tecnologia,
            Contato = "maria@techsc.com.br",
            Ativo = true
        };

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Empreendimento>()))
            .ReturnsAsync((Empreendimento e) => e);

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBe(Guid.Empty);
        result.NomeEmpreendimento.Should().Be("Tech SC Innovation");
        result.NomeEmpreendedor.Should().Be("Maria Silva");
        result.Municipio.Should().Be("Florianópolis");
        result.SegmentoAtuacao.Should().Be(SegmentoAtuacao.Tecnologia);
        result.Contato.Should().Be("maria@techsc.com.br");
        result.Ativo.Should().BeTrue();
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Empreendimento>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ComNomeVazio_DeveLancarArgumentException()
    {
        // Arrange
        var dto = new CreateEmpreendimentoDto
        {
            NomeEmpreendimento = "",
            NomeEmpreendedor = "Maria Silva",
            Municipio = "Florianópolis",
            SegmentoAtuacao = SegmentoAtuacao.Tecnologia,
            Contato = "maria@techsc.com.br"
        };

        // Act
        var act = () => _service.CreateAsync(dto);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*nome do empreendimento*");
    }

    [Fact]
    public async Task CreateAsync_ComTodosCamposVazios_DeveLancarArgumentExceptionComMultiplosErros()
    {
        // Arrange
        var dto = new CreateEmpreendimentoDto
        {
            NomeEmpreendimento = "",
            NomeEmpreendedor = "",
            Municipio = "",
            Contato = ""
        };

        // Act
        var act = () => _service.CreateAsync(dto);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    // ===== UpdateAsync =====

    [Fact]
    public async Task UpdateAsync_ComDadosValidos_DeveAtualizar()
    {
        // Arrange
        var id = Guid.NewGuid();
        var existing = CriarEmpreendimentoValido("Antigo", "Florianópolis", SegmentoAtuacao.Tecnologia);
        existing.Id = id;

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Empreendimento>()))
            .ReturnsAsync((Empreendimento e) => e);

        var dto = new UpdateEmpreendimentoDto
        {
            NomeEmpreendimento = "Novo Nome",
            NomeEmpreendedor = "Novo Empreendedor",
            Municipio = "Joinville",
            SegmentoAtuacao = SegmentoAtuacao.Comercio,
            Contato = "novo@email.com",
            Ativo = false
        };

        // Act
        var result = await _service.UpdateAsync(id, dto);

        // Assert
        result.Should().NotBeNull();
        result!.NomeEmpreendimento.Should().Be("Novo Nome");
        result.Municipio.Should().Be("Joinville");
        result.SegmentoAtuacao.Should().Be(SegmentoAtuacao.Comercio);
        result.Ativo.Should().BeFalse();
        result.DataAtualizacao.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateAsync_QuandoNaoExiste_DeveRetornarNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Empreendimento?)null);

        var dto = new UpdateEmpreendimentoDto
        {
            NomeEmpreendimento = "Teste",
            NomeEmpreendedor = "Teste",
            Municipio = "Teste",
            SegmentoAtuacao = SegmentoAtuacao.Tecnologia,
            Contato = "teste@email.com"
        };

        // Act
        var result = await _service.UpdateAsync(id, dto);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_ComDadosInvalidos_DeveLancarArgumentException()
    {
        // Arrange
        var dto = new UpdateEmpreendimentoDto
        {
            NomeEmpreendimento = "",
            NomeEmpreendedor = "",
            Municipio = "",
            Contato = ""
        };

        // Act
        var act = () => _service.UpdateAsync(Guid.NewGuid(), dto);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    // ===== DeleteAsync =====

    [Fact]
    public async Task DeleteAsync_QuandoExiste_DeveRetornarTrue()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.DeleteAsync(id)).ReturnsAsync(true);

        // Act
        var result = await _service.DeleteAsync(id);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_QuandoNaoExiste_DeveRetornarFalse()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.DeleteAsync(id)).ReturnsAsync(false);

        // Act
        var result = await _service.DeleteAsync(id);

        // Assert
        result.Should().BeFalse();
    }

    // ===== Helpers =====

    private static Empreendimento CriarEmpreendimentoValido(string nome, string municipio, SegmentoAtuacao segmento)
    {
        return new Empreendimento
        {
            Id = Guid.NewGuid(),
            NomeEmpreendimento = nome,
            NomeEmpreendedor = "Empreendedor Teste",
            Municipio = municipio,
            SegmentoAtuacao = segmento,
            Contato = "contato@teste.com",
            Ativo = true,
            DataCriacao = DateTime.UtcNow
        };
    }
}
