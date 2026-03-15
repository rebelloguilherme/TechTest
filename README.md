# 🏢 Empreendedorismo SC — API REST

API REST para gerenciamento de empreendimentos em Santa Catarina, desenvolvida em **.NET 8** seguindo os princípios de **Clean Architecture**.

📺 **Vídeo Pitch:** Confira uma breve apresentação e demonstração das funcionalidades do projeto [neste link do YouTube](https://youtu.be/VSpE8kAGV18).

## 📋 Descrição

Sistema CRUD para cadastro e consulta de empreendimentos catarinenses, contemplando:

- Nome do empreendimento e do(a) empreendedor(a) responsável
- Município de Santa Catarina
- Segmento de atuação (Tecnologia, Comércio, Indústria, Serviços, Agronegócio)
- E-mail ou meio de contato
- Status (ativo/inativo)

### Funcionalidades

- **CRUD completo** — criar, listar, buscar por ID, atualizar e remover empreendimentos
- **Validação** — regras de negócio com FluentValidation (campos obrigatórios, tamanhos máximos, formato de contato)
- **Filtros** — por município, segmento, status e busca textual
- **Paginação** — controle de página e tamanho com metadados (`totalItems`, `totalPaginas`, `temProximaPagina`)
- **Respostas padronizadas** — wrapper `ApiResponse<T>` em todos os endpoints
- **Tratamento global de erros** — middleware que captura exceções e retorna respostas consistentes
- **Persistência com SQLite** — banco de dados local, criado automaticamente no primeiro uso
- **Seed data** — 5 empreendimentos de exemplo em municípios de SC
- **Logging estruturado** — Serilog com saída para console e arquivo (rolling diário)
- **Swagger/OpenAPI** — documentação interativa disponível em `/swagger`

---

## 🛠 Tecnologias Utilizadas

| Tecnologia | Versão | Finalidade |
|---|---|---|
| .NET | 8.0 | Framework principal |
| ASP.NET Core | 8.0 | API REST |
| Entity Framework Core | 8.0.11 | ORM e migrations |
| SQLite | — | Banco de dados (embutido, sem instalação) |
| FluentValidation | 11.11.0 | Validação de DTOs |
| Serilog | 8.0.3 | Logging estruturado |
| Swagger / Swashbuckle | — | Documentação da API |
| xUnit | 2.5.3 | Framework de testes |
| Moq | 4.20.72 | Mocking para testes unitários |
| FluentAssertions | 6.12.2 | Assertions expressivas |
| Microsoft.AspNetCore.Mvc.Testing | 8.0.11 | Testes de integração |

---

## 📁 Estrutura do Projeto

```
TechTest/
├── src/
│   ├── EmpreendedorismoSC.Domain/              # Camada de Domínio
│   │   ├── Entities/
│   │   │   └── Empreendimento.cs               # Entidade principal
│   │   ├── Enums/
│   │   │   └── SegmentoAtuacao.cs               # Enum dos segmentos
│   │   └── Interfaces/
│   │       └── IEmpreendimentoRepository.cs     # Contrato do repositório
│   │
│   ├── EmpreendedorismoSC.Application/          # Camada de Aplicação
│   │   ├── Common/
│   │   │   ├── ApiResponse.cs                   # Wrapper de resposta padronizada
│   │   │   └── PagedResult.cs                   # Resultado paginado genérico
│   │   ├── DTOs/
│   │   │   ├── CreateEmpreendimentoDto.cs       # DTO de criação
│   │   │   ├── UpdateEmpreendimentoDto.cs       # DTO de atualização
│   │   │   ├── EmpreendimentoDto.cs             # DTO de leitura
│   │   │   └── EmpreendimentoFilterDto.cs       # DTO de filtros + paginação
│   │   ├── Interfaces/
│   │   │   └── IEmpreendimentoService.cs        # Contrato do serviço
│   │   ├── Services/
│   │   │   └── EmpreendimentoService.cs         # Lógica de negócio
│   │   └── Validators/
│   │       ├── CreateEmpreendimentoValidator.cs  # Validação de criação
│   │       └── UpdateEmpreendimentoValidator.cs  # Validação de atualização
│   │
│   ├── EmpreendedorismoSC.Infrastructure/       # Camada de Infraestrutura
│   │   ├── Data/
│   │   │   └── ApplicationDbContext.cs          # DbContext + seed data
│   │   ├── Migrations/                          # EF Core Migrations
│   │   └── Repositories/
│   │       └── EmpreendimentoRepository.cs      # Implementação do repositório
│   │
│   └── EmpreendedorismoSC.WebApi/               # Camada de Apresentação
│       ├── Controllers/
│       │   └── EmpreendimentosController.cs     # Endpoints REST
│       ├── Middleware/
│       │   └── GlobalExceptionMiddleware.cs     # Tratamento global de erros
│       └── Program.cs                           # Configuração e startup
│
├── tests/
│   ├── EmpreendedorismoSC.UnitTests/            # Testes unitários (35)
│   │   ├── Domain/                              # Testes da entidade e enum
│   │   ├── Application/                         # Testes do service (com mock)
│   │   └── Infrastructure/                      # Testes do repository
│   └── EmpreendedorismoSC.IntegrationTests/     # Testes de integração (11)
│       └── EmpreendimentosApiTests.cs           # Testes do pipeline HTTP
│
├── EmpreendedorismoSC.sln
├── global.json
└── README.md
```

### Arquitetura

```
    WebApi (API)  →  Application (Lógica)  →  Domain (Entidades)
         ↓                                         ↑
    Infrastructure (EF Core, SQLite) ──────────────┘
```

- **Domain** — entidades, enums e interfaces (sem dependências externas)
- **Application** — DTOs, serviços, validações e contratos
- **Infrastructure** — implementação de repositórios, DbContext e migrations
- **WebApi** — controllers, middleware e configuração do pipeline

---

## 🚀 Como Executar

### Pré-requisitos

- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) instalado

> **Não é necessário instalar banco de dados.** O SQLite é embutido e o arquivo `empreendedorismo_sc.db` é criado automaticamente na primeira execução.

### Passos

```bash
# 1. Clonar o repositório
git clone <url-do-repositorio>
cd TechTest

# 2. Restaurar pacotes
dotnet restore

# 3. Executar a aplicação
dotnet run --project src/EmpreendedorismoSC.WebApi

# A API estará disponível em: http://localhost:5121
# Swagger UI: http://localhost:5121/swagger
```

Na primeira execução, o sistema:
1. Cria o banco de dados SQLite (`empreendedorismo_sc.db`)
2. Aplica as migrations (cria tabelas e índices)
3. Insere 5 empreendimentos de exemplo (seed data)

### Executar os testes

```bash
# Todos os testes (unitários + integração)
dotnet test

# Apenas unitários
dotnet test tests/EmpreendedorismoSC.UnitTests

# Apenas integração
dotnet test tests/EmpreendedorismoSC.IntegrationTests
```

---

## 📡 Endpoints da API

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/api/empreendimentos` | Listar (com filtros e paginação) |
| `GET` | `/api/empreendimentos/{id}` | Buscar por ID |
| `POST` | `/api/empreendimentos` | Criar empreendimento |
| `PUT` | `/api/empreendimentos/{id}` | Atualizar empreendimento |
| `DELETE` | `/api/empreendimentos/{id}` | Remover empreendimento |

### Parâmetros de filtro (GET `/api/empreendimentos`)

| Parâmetro | Tipo | Descrição |
|---|---|---|
| `municipio` | string | Filtro parcial por município |
| `segmentoAtuacao` | enum | Tecnologia, Comercio, Industria, Servicos, Agronegocio |
| `ativo` | bool | Filtrar por status |
| `busca` | string | Busca textual livre nos campos **Nome do Empreendimento** e **Nome do Empreendedor** (parcial, case-insensitive) |
| `pagina` | int | Número da página (padrão: 1) |
| `tamanhoPagina` | int | Itens por página (padrão: 10, máximo: 50) |

### Exemplo de resposta

```json
{
  "sucesso": true,
  "mensagem": "Empreendimentos listados com sucesso.",
  "dados": {
    "items": [
      {
        "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
        "nomeEmpreendimento": "Tech Floripa Hub",
        "nomeEmpreendedor": "Marina Costa",
        "municipio": "Florianópolis",
        "segmentoAtuacao": "Tecnologia",
        "contato": "marina@techfloripahub.com.br",
        "ativo": true,
        "dataCriacao": "2024-01-15T10:00:00",
        "dataAtualizacao": null
      }
    ],
    "totalItems": 5,
    "pagina": 1,
    "tamanhoPagina": 10,
    "totalPaginas": 1,
    "temProximaPagina": false
  },
  "erros": null
}
```

---

## 🧪 Testes

| Tipo | Projeto | Testes | Cobertura |
|---|---|---|---|
| Unitário | `EmpreendedorismoSC.UnitTests` | 35 | Domain, Service, Repository |
| Integração | `EmpreendedorismoSC.IntegrationTests` | 11 | Pipeline HTTP completo |
| **Total** | | **46** | |

---

## 📂 Logs

Os logs são gravados em:
- **Console** — saída padrão (Serilog)
- **Arquivo** — `src/EmpreendedorismoSC.WebApi/logs/log-YYYY-MM-DD.txt` (rolling diário)
