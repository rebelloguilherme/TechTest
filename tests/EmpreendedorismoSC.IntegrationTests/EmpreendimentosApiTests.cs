using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using EmpreendedorismoSC.Application.Common;
using EmpreendedorismoSC.Application.DTOs;
using EmpreendedorismoSC.Domain.Enums;
using FluentAssertions;

namespace EmpreendedorismoSC.IntegrationTests;

public class EmpreendimentosApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public EmpreendimentosApiTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() }
        };
    }

    // ===== POST =====

    [Fact]
    public async Task Post_ComDadosValidos_DeveRetornar201ComApiResponse()
    {
        // Arrange
        var dto = CriarDtoValido();

        // Act
        var response = await PostAsync("/api/empreendimentos", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await DeserializarResponse<EmpreendimentoDto>(response);
        body.Sucesso.Should().BeTrue();
        body.Mensagem.Should().Contain("criado");
        body.Dados.Should().NotBeNull();
        body.Dados!.Id.Should().NotBe(Guid.Empty);
        body.Dados.NomeEmpreendimento.Should().Be("Tech SC Innovation");
        body.Dados.SegmentoAtuacao.Should().Be(SegmentoAtuacao.Tecnologia);
    }

    [Fact]
    public async Task Post_ComCamposVazios_DeveRetornar400ComErrosDeValidacao()
    {
        // Arrange
        var dto = new CreateEmpreendimentoDto
        {
            NomeEmpreendimento = "",
            NomeEmpreendedor = "",
            Municipio = "",
            SegmentoAtuacao = SegmentoAtuacao.Tecnologia,
            Contato = ""
        };

        // Act
        var response = await PostAsync("/api/empreendimentos", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var body = await DeserializarResponse<object>(response);
        body.Sucesso.Should().BeFalse();
    }

    // ===== GET by ID =====

    [Fact]
    public async Task GetById_QuandoExiste_DeveRetornar200ComApiResponse()
    {
        // Arrange
        var created = await CriarEmpreendimentoViaApi();

        // Act
        var response = await _client.GetAsync($"/api/empreendimentos/{created.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await DeserializarResponse<EmpreendimentoDto>(response);
        body.Sucesso.Should().BeTrue();
        body.Dados.Should().NotBeNull();
        body.Dados!.Id.Should().Be(created.Id);
    }

    [Fact]
    public async Task GetById_QuandoNaoExiste_DeveRetornar404()
    {
        // Act
        var response = await _client.GetAsync($"/api/empreendimentos/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var body = await DeserializarResponse<object>(response);
        body.Sucesso.Should().BeFalse();
        body.Mensagem.Should().Contain("não encontrado");
    }

    // ===== GET all com paginação e filtros =====

    [Fact]
    public async Task GetAll_DeveRetornar200ComPaginacao()
    {
        // Arrange
        await CriarEmpreendimentoViaApi("Emp Pag 1", "Cidade1", SegmentoAtuacao.Tecnologia);

        // Act
        var response = await _client.GetAsync("/api/empreendimentos");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        root.GetProperty("sucesso").GetBoolean().Should().BeTrue();
        root.GetProperty("dados").GetProperty("totalItems").GetInt32().Should().BeGreaterOrEqualTo(1);
        root.GetProperty("dados").GetProperty("pagina").GetInt32().Should().Be(1);
    }

    [Fact]
    public async Task GetAll_ComFiltroPorMunicipio_DeveRetornarFiltrados()
    {
        // Arrange
        await CriarEmpreendimentoViaApi("Empresa Unica Fpolis", "FlorianopolisUnica", SegmentoAtuacao.Tecnologia);

        // Act
        var response = await _client.GetAsync("/api/empreendimentos?municipio=FlorianopolisUnica");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var items = doc.RootElement.GetProperty("dados").GetProperty("items");

        items.GetArrayLength().Should().BeGreaterOrEqualTo(1);
        items.EnumerateArray().Should().AllSatisfy(item =>
            item.GetProperty("municipio").GetString().Should().Contain("FlorianopolisUnica"));
    }

    [Fact]
    public async Task GetAll_ComPaginacao_DeveLimitarResultados()
    {
        // Arrange
        await CriarEmpreendimentoViaApi("Emp Pag A", "CidadeA", SegmentoAtuacao.Tecnologia);
        await CriarEmpreendimentoViaApi("Emp Pag B", "CidadeB", SegmentoAtuacao.Comercio);

        // Act
        var response = await _client.GetAsync("/api/empreendimentos?pagina=1&tamanhoPagina=1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var dados = doc.RootElement.GetProperty("dados");

        dados.GetProperty("tamanhoPagina").GetInt32().Should().Be(1);
        dados.GetProperty("items").GetArrayLength().Should().Be(1);
    }

    // ===== PUT =====

    [Fact]
    public async Task Put_ComDadosValidos_DeveRetornar200()
    {
        // Arrange
        var created = await CriarEmpreendimentoViaApi();
        var updateDto = new UpdateEmpreendimentoDto
        {
            NomeEmpreendimento = "Nome Atualizado",
            NomeEmpreendedor = "Empreendedor Atualizado",
            Municipio = "Joinville",
            SegmentoAtuacao = SegmentoAtuacao.Comercio,
            Contato = "novo@email.com",
            Ativo = false
        };

        // Act
        var response = await PutAsync($"/api/empreendimentos/{created.Id}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await DeserializarResponse<EmpreendimentoDto>(response);
        body.Sucesso.Should().BeTrue();
        body.Dados!.NomeEmpreendimento.Should().Be("Nome Atualizado");
        body.Dados.Municipio.Should().Be("Joinville");
        body.Dados.Ativo.Should().BeFalse();
    }

    [Fact]
    public async Task Put_IdInexistente_DeveRetornar404()
    {
        // Arrange
        var updateDto = new UpdateEmpreendimentoDto
        {
            NomeEmpreendimento = "Teste",
            NomeEmpreendedor = "Teste",
            Municipio = "Teste",
            SegmentoAtuacao = SegmentoAtuacao.Tecnologia,
            Contato = "teste@email.com"
        };

        // Act
        var response = await PutAsync($"/api/empreendimentos/{Guid.NewGuid()}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ===== DELETE =====

    [Fact]
    public async Task Delete_QuandoExiste_DeveRetornar200()
    {
        // Arrange
        var created = await CriarEmpreendimentoViaApi("ParaDeletar", "CidadeDel", SegmentoAtuacao.Servicos);

        // Act
        var response = await _client.DeleteAsync($"/api/empreendimentos/{created.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await DeserializarResponse<object>(response);
        body.Sucesso.Should().BeTrue();
        body.Mensagem.Should().Contain("removido");

        // Verificar que realmente foi removido
        var getResponse = await _client.GetAsync($"/api/empreendimentos/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_IdInexistente_DeveRetornar404()
    {
        // Act
        var response = await _client.DeleteAsync($"/api/empreendimentos/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ===== Helpers =====

    private static CreateEmpreendimentoDto CriarDtoValido(
        string nome = "Tech SC Innovation",
        string municipio = "Florianopolis",
        SegmentoAtuacao segmento = SegmentoAtuacao.Tecnologia)
    {
        return new CreateEmpreendimentoDto
        {
            NomeEmpreendimento = nome,
            NomeEmpreendedor = "Maria Silva",
            Municipio = municipio,
            SegmentoAtuacao = segmento,
            Contato = "maria@techsc.com.br",
            Ativo = true
        };
    }

    private async Task<EmpreendimentoDto> CriarEmpreendimentoViaApi(
        string nome = "Tech SC Innovation",
        string municipio = "Florianopolis",
        SegmentoAtuacao segmento = SegmentoAtuacao.Tecnologia)
    {
        var dto = CriarDtoValido(nome, municipio, segmento);
        var response = await PostAsync("/api/empreendimentos", dto);
        response.StatusCode.Should().Be(HttpStatusCode.Created,
            $"Falha ao criar empreendimento: {await response.Content.ReadAsStringAsync()}");

        var body = await DeserializarResponse<EmpreendimentoDto>(response);
        return body.Dados!;
    }

    private async Task<HttpResponseMessage> PostAsync<T>(string url, T payload)
    {
        var json = JsonSerializer.Serialize(payload, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        return await _client.PostAsync(url, content);
    }

    private async Task<HttpResponseMessage> PutAsync<T>(string url, T payload)
    {
        var json = JsonSerializer.Serialize(payload, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        return await _client.PutAsync(url, content);
    }

    private async Task<ApiResponse<T>> DeserializarResponse<T>(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ApiResponse<T>>(json, _jsonOptions)!;
    }
}
