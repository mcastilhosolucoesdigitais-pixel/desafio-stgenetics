# Good Hamburger 🍔

Sistema de pedidos para a lanchonete **Good Hamburger**, desenvolvido como desafio técnico para a STGenetics.

## Funcionalidades

- **Cardápio**: endpoint e interface visual com os 5 itens disponíveis
- **Pedidos (CRUD)**: criar, listar, consultar, atualizar e remover pedidos
- **Desconto automático**: calculado conforme a combinação de itens
- **Validações**: itens duplicados, sanduíche obrigatório, IDs inválidos
- **Frontend Blazor**: interface web consumindo a API
- **Observabilidade**: health checks, métricas e tracing via OpenTelemetry

## Regras de negócio

| Combinação | Desconto |
|---|---|
| Sanduíche + Batata Frita + Refrigerante | 20% |
| Sanduíche + Refrigerante | 15% |
| Sanduíche + Batata Frita | 10% |
| Apenas sanduíche | 0% |

> Cada pedido aceita no máximo **um item por categoria**. Itens duplicados retornam HTTP 422.

## Stack

| Camada | Tecnologia |
|---|---|
| Orquestração | .NET Aspire 9.3.1 |
| API | ASP.NET Core 10 — Minimal APIs |
| Frontend | Blazor WebAssembly (.NET 10) |
| Observabilidade | OpenTelemetry + Aspire Dashboard |
| Testes unitários | xUnit + FluentAssertions + NSubstitute |
| Testes de integração | xUnit + Microsoft.AspNetCore.Mvc.Testing |
| Cobertura | coverlet |
| CI | GitHub Actions |

## Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Docker (opcional — necessário apenas para recursos em container no Aspire)

## Execução

### Opção 1 — Via .NET Aspire (recomendado)

Sobe API + Blazor + Dashboard de telemetria em um único comando:

```bash
dotnet run --project src/GoodHamburger.AppHost
```

Serviços disponíveis após a inicialização:

| Serviço | URL |
|---|---|
| Aspire Dashboard | https://localhost:18888 |
| API (via dashboard) | link disponível no dashboard |
| Blazor Web (via dashboard) | link disponível no dashboard |

### Opção 2 — API isolada

```bash
dotnet run --project src/GoodHamburger.Api
```

API disponível em `http://localhost:5001`. Documentação OpenAPI em `http://localhost:5001/openapi/v1.json`.

### Opção 3 — Frontend isolado

```bash
dotnet run --project src/GoodHamburger.Web
```

Configure a URL da API via `src/GoodHamburger.Web/wwwroot/appsettings.json`:

```json
{ "ApiBaseUrl": "http://localhost:5001/" }
```

## Testes

```bash
# Executar todos os testes
dotnet test

# Com relatório de cobertura (domínio)
dotnet test tests/GoodHamburger.UnitTests \
  /p:CollectCoverage=true \
  /p:CoverletOutputFormat=cobertura \
  "/p:Include=[GoodHamburger.Api]GoodHamburger.Api.Domain.*"
```

Resultado atual: **60/60 testes passando** (28 unitários + 32 integração, net10.0).

Cobertura da camada de domínio: **87% line / 100% branch**.

## Endpoints da API

### Cardápio

```
GET /menu
```

### Pedidos

```
POST   /orders
GET    /orders
GET    /orders/{id}
PUT    /orders/{id}
DELETE /orders/{id}
```

### Saúde

```
GET /health   → status geral
GET /alive    → liveness probe
```

### Exemplo — criar pedido

```bash
curl -X POST http://localhost:5001/orders \
  -H "Content-Type: application/json" \
  -d '{ "itemIds": [1, 4, 5] }'
```

Resposta (`201 Created`):

```json
{
  "id": 1,
  "items": [
    { "id": 1, "name": "X Burger",     "category": "Sandwich", "price": 5.00 },
    { "id": 4, "name": "Batata Frita", "category": "Side",     "price": 2.00 },
    { "id": 5, "name": "Refrigerante", "category": "Side",     "price": 2.50 }
  ],
  "subtotal": 9.50,
  "discountPercent": 0.20,
  "discount": 1.90,
  "total": 7.60
}
```

### Exemplo — erro de validação

```bash
curl -X POST http://localhost:5001/orders \
  -H "Content-Type: application/json" \
  -d '{ "itemIds": [4, 5] }'
```

Resposta (`422 Unprocessable Entity`):

```json
{
  "errors": ["Order must contain exactly one sandwich."]
}
```

## Estrutura do projeto

```
GoodHamburger.sln
│
├── src/
│   ├── GoodHamburger.AppHost/          # Orquestrador .NET Aspire
│   ├── GoodHamburger.ServiceDefaults/  # OpenTelemetry, health checks, service discovery
│   ├── GoodHamburger.Api/              # ASP.NET Core Minimal API
│   │   ├── Domain/                     # DiscountCalculator, OrderValidator, entidades
│   │   ├── Services/                   # MenuService, OrderService (in-memory)
│   │   ├── Endpoints/                  # MenuEndpoints, OrderEndpoints
│   │   └── DTOs/                       # Contratos de request/response
│   └── GoodHamburger.Web/              # Blazor WebAssembly
│       ├── Pages/                      # Menu.razor, Orders.razor
│       ├── Services/                   # ApiClient
│       └── Models/                     # MenuItemModel, OrderModel
│
├── tests/
│   ├── GoodHamburger.UnitTests/        # DiscountCalculator, OrderValidator, MenuService
│   └── GoodHamburger.IntegrationTests/ # Todos os endpoints HTTP
│
└── docs/
    ├── planejamento.md
    └── backlogs.md
```

## Decisões arquiteturais

### Minimal APIs
Adequadas ao escopo do desafio: eliminam a cerimônia de controllers mantendo o código coeso e próximo das rotas. Cada endpoint tem responsabilidade única e clara.

### Persistência in-memory
Os pedidos são armazenados em memória (`List<Order>` com `Singleton`). A decisão foi deliberada: o desafio não exige persistência entre reinicializações, e uma camada de banco de dados adicionaria complexidade sem agregar valor ao objetivo avaliado. O `IOrderService` isola essa decisão — trocar por EF Core + PostgreSQL exige apenas uma nova implementação da interface.

### TDD como guia de design
As regras de desconto e validação foram especificadas como testes antes da implementação. Isso garantiu que `DiscountCalculator` e `OrderValidator` fossem classes puras e sem dependências externas, testáveis de forma isolada e com 100% de cobertura de branches.

### .NET Aspire para orquestração local
O Aspire elimina a configuração manual de URLs entre serviços e provê um dashboard de telemetria integrado. No .NET 10, o Aspire é distribuído via NuGet (sem workload), com o SDK referenciado diretamente no projeto AppHost.

### Blazor WebAssembly
Escolhido por ser um frontend genuinamente client-side que consome a API via HTTP — o modelo mais próximo de uma SPA real. A comunicação com a API é centralizada no `ApiClient`, facilitando manutenção e futuras mudanças de contrato.

## O que ficou de fora

| Item | Motivo |
|---|---|
| Persistência em banco de dados | Fora do escopo do desafio; arquitetura permite adição sem breaking changes |
| Autenticação / Autorização | Não requerido pelo enunciado |
| Paginação na listagem de pedidos | Sem requisito explícito; trivial de adicionar com `IOrderService` atual |
| Testes E2E (Playwright) | Priorizaram-se testes de integração que cobrem os mesmos fluxos com menor overhead |
| Deploy em nuvem | Não requerido; Aspire facilita a adição de recursos Azure/AWS |
