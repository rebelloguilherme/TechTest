using EmpreendedorismoSC.Domain.Entities;
using EmpreendedorismoSC.Domain.Enums;
using FluentAssertions;

namespace EmpreendedorismoSC.UnitTests.Domain;

public class EmpreendimentoTests
{
    [Fact]
    public void Empreendimento_DeveSerCriadoComValoresPadrao()
    {
        // Arrange & Act
        var empreendimento = new Empreendimento();

        // Assert
        empreendimento.Id.Should().Be(Guid.Empty);
        empreendimento.NomeEmpreendimento.Should().BeEmpty();
        empreendimento.NomeEmpreendedor.Should().BeEmpty();
        empreendimento.Municipio.Should().BeEmpty();
        empreendimento.Contato.Should().BeEmpty();
        empreendimento.Ativo.Should().BeTrue();
        empreendimento.DataAtualizacao.Should().BeNull();
    }

    [Fact]
    public void Empreendimento_DevePermitirAtribuirTodosOsCampos()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dataCriacao = DateTime.UtcNow;
        var dataAtualizacao = DateTime.UtcNow;

        // Act
        var empreendimento = new Empreendimento
        {
            Id = id,
            NomeEmpreendimento = "Tech SC Innovation",
            NomeEmpreendedor = "Maria Silva",
            Municipio = "Florianópolis",
            SegmentoAtuacao = SegmentoAtuacao.Tecnologia,
            Contato = "maria@techsc.com.br",
            Ativo = true,
            DataCriacao = dataCriacao,
            DataAtualizacao = dataAtualizacao
        };

        // Assert
        empreendimento.Id.Should().Be(id);
        empreendimento.NomeEmpreendimento.Should().Be("Tech SC Innovation");
        empreendimento.NomeEmpreendedor.Should().Be("Maria Silva");
        empreendimento.Municipio.Should().Be("Florianópolis");
        empreendimento.SegmentoAtuacao.Should().Be(SegmentoAtuacao.Tecnologia);
        empreendimento.Contato.Should().Be("maria@techsc.com.br");
        empreendimento.Ativo.Should().BeTrue();
        empreendimento.DataCriacao.Should().Be(dataCriacao);
        empreendimento.DataAtualizacao.Should().Be(dataAtualizacao);
    }

    [Theory]
    [InlineData(SegmentoAtuacao.Tecnologia)]
    [InlineData(SegmentoAtuacao.Comercio)]
    [InlineData(SegmentoAtuacao.Industria)]
    [InlineData(SegmentoAtuacao.Servicos)]
    [InlineData(SegmentoAtuacao.Agronegocio)]
    public void Empreendimento_DeveAceitarTodosSegmentos(SegmentoAtuacao segmento)
    {
        // Arrange & Act
        var empreendimento = new Empreendimento
        {
            SegmentoAtuacao = segmento
        };

        // Assert
        empreendimento.SegmentoAtuacao.Should().Be(segmento);
    }

    [Fact]
    public void Empreendimento_DevePermitirAlterarStatus()
    {
        // Arrange
        var empreendimento = new Empreendimento { Ativo = true };

        // Act
        empreendimento.Ativo = false;

        // Assert
        empreendimento.Ativo.Should().BeFalse();
    }

    [Fact]
    public void SegmentoAtuacao_DeveConterCincoValores()
    {
        // Arrange & Act
        var valores = Enum.GetValues<SegmentoAtuacao>();

        // Assert
        valores.Should().HaveCount(5);
        valores.Should().Contain(SegmentoAtuacao.Tecnologia);
        valores.Should().Contain(SegmentoAtuacao.Comercio);
        valores.Should().Contain(SegmentoAtuacao.Industria);
        valores.Should().Contain(SegmentoAtuacao.Servicos);
        valores.Should().Contain(SegmentoAtuacao.Agronegocio);
    }
}
