# Planejamento de Desenvolvimento — Good Hamburger

## Visão Geral

Sistema de pedidos para a lanchonete **Good Hamburger**, construído com **ASP.NET Core + .NET Aspire 10**, adotando **TDD** como prática central de desenvolvimento.

---

## Stack Tecnológica

| Camada | Tecnologia |
|---|---|
| Orquestração | .NET Aspire 10 (AppHost) |
| API | ASP.NET Core 10 (Minimal APIs) |
| Frontend | Blazor WebAssembly (diferencial) |
| Persistência | Entity Framework Core + SQLite (dev) / PostgreSQL (prod via Aspire) |
| Testes unitários | xUnit + FluentAssertions + NSubstitute |
| Testes de integração | Microsoft.AspNetCore.Mvc.Testing + Testcontainers |
| Documentação da API | Scalar (OpenAPI) |
| CI | GitHub Actions |

---

## Estrutura de Solução

```
GoodHamburger.sln
│
├── src/
│   ├── GoodHamburger.AppHost/          # .NET Aspire — orquestrador
│   ├── GoodHamburger.ServiceDefaults/  # .NET Aspire — defaults compartilhados (telemetria, health, etc.)
│   ├── GoodHamburger.Api/              # ASP.NET Core — Minimal API
│   │   ├── Endpoints/
│   │   │   ├── MenuEndpoints.cs
│   │   │   └── OrderEndpoints.cs
│   │   ├── Models/
│   │   │   ├── MenuItem.cs
│   │   │   ├── Order.cs
│   │   │   └── OrderItem.cs
│   │   ├── DTOs/
│   │   │   ├── CreateOrderRequest.cs
│   │   │   ├── UpdateOrderRequest.cs
│   │   │   └── OrderResponse.cs
│   │   ├── Services/
│   │   │   ├── IOrderService.cs
│   │   │   ├── OrderService.cs
│   │   │   ├── IMenuService.cs
│   │   │   └── MenuService.cs
│   │   ├── Domain/
│   │   │   ├── DiscountCalculator.cs
│   │   │   └── OrderValidator.cs
│   │   ├── Data/
│   │   │   ├── AppDbContext.cs
│   │   │   └── Migrations/
│   │   └── Program.cs
│   │
│   └── GoodHamburger.Web/              # Blazor WebAssembly (diferencial)
│       ├── Pages/
│       │   ├── Menu.razor
│       │   └── Orders.razor
│       └── Services/
│           └── ApiClient.cs
│
└── tests/
    ├── GoodHamburger.UnitTests/
    │   ├── Domain/
    │   │   ├── DiscountCalculatorTests.cs
    │   │   └── OrderValidatorTests.cs
    │   └── Services/
    │       └── OrderServiceTests.cs
    └── GoodHamburger.IntegrationTests/
        ├── Endpoints/
        │   ├── MenuEndpointsTests.cs
        │   └── OrderEndpointsTests.cs
        └── WebApplicationFactory.cs
```

---

## Abordagem TDD — Ciclo Red → Green → Refactor

Cada funcionalidade de domínio segue rigorosamente o ciclo:

1. **Red** — Escrever o teste que falha, expressando o comportamento esperado.
2. **Green** — Implementar o mínimo de código para o teste passar.
3. **Refactor** — Eliminar duplicação e melhorar legibilidade sem quebrar testes.

### Ordem de implementação guiada por testes

```
Fase 1 — Domínio (sem dependências externas)
  [RED]   DiscountCalculatorTests — regras de desconto
  [GREEN] DiscountCalculator.cs
  [RED]   OrderValidatorTests — item duplicado, pedido sem sanduíche
  [GREEN] OrderValidator.cs

Fase 2 — Serviços (com mocks de repositório)
  [RED]   OrderServiceTests — CRUD, cálculo integrado
  [GREEN] OrderService.cs

Fase 3 — Endpoints (testes de integração)
  [RED]   MenuEndpointsTests — GET /menu
  [RED]   OrderEndpointsTests — POST, GET, PUT, DELETE /orders
  [GREEN] MenuEndpoints.cs, OrderEndpoints.cs, Program.cs

Fase 4 — Persistência
  [GREEN] AppDbContext + EF Migrations

Fase 5 — Frontend Blazor (diferencial)
  [GREEN] Componentes razor consumindo a API
```

---

## Regras de Domínio

### Cardápio

| Categoria | Item | Preço |
|---|---|---|
| Sanduíche | X Burger | R$ 5,00 |
| Sanduíche | X Egg | R$ 4,50 |
| Sanduíche | X Bacon | R$ 7,00 |
| Acompanhamento | Batata Frita | R$ 2,00 |
| Acompanhamento | Refrigerante | R$ 2,50 |

### Regras de Desconto

| Combinação | Desconto |
|---|---|
| Sanduíche + Batata + Refrigerante | 20% |
| Sanduíche + Refrigerante | 15% |
| Sanduíche + Batata | 10% |
| Apenas sanduíche (sem acompanhamento) | 0% |

### Restrições de Validação

- Cada pedido pode conter **no máximo um** item de cada categoria.
- Um pedido **deve** conter ao menos um sanduíche.
- Itens duplicados devem ser rejeitados com HTTP 422 e mensagem descritiva.
- Pedido com ID inexistente retorna HTTP 404.

### Cálculo de Totais

```
Subtotal = soma dos preços dos itens sem desconto
Desconto = percentual aplicado sobre o subtotal (conforme combinação)
Total    = Subtotal - Desconto
```

---

## Endpoints da API

### Menu

| Método | Rota | Descrição |
|---|---|---|
| GET | `/menu` | Lista todos os itens do cardápio agrupados por categoria |

### Pedidos

| Método | Rota | Descrição |
|---|---|---|
| POST | `/orders` | Cria um novo pedido |
| GET | `/orders` | Lista todos os pedidos |
| GET | `/orders/{id}` | Consulta pedido por ID |
| PUT | `/orders/{id}` | Atualiza um pedido existente |
| DELETE | `/orders/{id}` | Remove um pedido |

### Respostas de Erro

| Cenário | HTTP Status | Exemplo de corpo |
|---|---|---|
| Item duplicado no pedido | 422 | `{ "error": "Duplicate item: Batata Frita" }` |
| Pedido sem sanduíche | 422 | `{ "error": "Order must contain exactly one sandwich" }` |
| Pedido não encontrado | 404 | `{ "error": "Order 42 not found" }` |

---

## Fases e Marcos

### Fase 1 — Domínio e Testes (Dias 1–2)
- [ ] Criar solução `.sln` e projetos (`Api`, `UnitTests`, `IntegrationTests`)
- [ ] Configurar `.NET Aspire 10` (`AppHost`, `ServiceDefaults`)
- [ ] Escrever testes de `DiscountCalculator` (Red)
- [ ] Implementar `DiscountCalculator` (Green)
- [ ] Escrever testes de `OrderValidator` (Red)
- [ ] Implementar `OrderValidator` (Green)
- [ ] Refactor da camada de domínio

### Fase 2 — Serviços e Repositório (Dias 2–3)
- [ ] Definir interfaces `IOrderService`, `IMenuService`
- [ ] Escrever testes de `OrderService` com mocks (Red)
- [ ] Implementar `OrderService` (Green)
- [ ] Implementar `AppDbContext` com EF Core + SQLite
- [ ] Criar migrations iniciais
- [ ] Seed do cardápio

### Fase 3 — Endpoints e Integração (Dias 3–4)
- [ ] Escrever testes de integração para `/menu` e `/orders` (Red)
- [ ] Implementar `MenuEndpoints` e `OrderEndpoints` (Green)
- [ ] Configurar `Program.cs` (DI, middleware, OpenAPI/Scalar)
- [ ] Registrar serviços no `AppHost` do Aspire
- [ ] Ajustar `ServiceDefaults` (health checks, telemetria)

### Fase 4 — Qualidade e Entregáveis (Dias 5–6)
- [ ] Revisar cobertura de testes (meta: ≥ 80% nas camadas de domínio e serviços)
- [ ] Validar todos os cenários de erro
- [ ] Configurar GitHub Actions (build + test)
- [ ] Escrever `README.md` com instruções de execução e decisões arquiteturais

### Fase 5 — Frontend Blazor (Dia 7, diferencial)
- [ ] Criar projeto `GoodHamburger.Web` (Blazor WebAssembly)
- [ ] Registrar no `AppHost` do Aspire
- [ ] Implementar página de cardápio (`/menu`)
- [ ] Implementar página de pedidos (`/orders`) com formulário de criação e listagem

---

## Decisões Arquiteturais

### Por que .NET Aspire 10?
Aspire oferece orquestração local de serviços (API + banco + frontend), dashboard de telemetria integrado e service discovery sem configuração manual — reduzindo atrito no setup e aproximando o ambiente de desenvolvimento do ambiente de produção.

### Por que Minimal APIs?
Para um escopo de CRUD focado, Minimal APIs eliminam a cerimônia de controllers sem perda de funcionalidade, mantendo o código coeso e próximo das rotas definidas nos endpoints.

### Por que SQLite (dev) + PostgreSQL (prod)?
SQLite não requer instalação, agilizando o onboarding. O Aspire provê PostgreSQL via container para o ambiente de produção/staging com uma linha de configuração.

### Por que TDD?
As regras de desconto e validação são o núcleo de valor do sistema. TDD garante que essas regras sejam especificadas com precisão antes da implementação, servindo como documentação viva e rede de segurança para refatorações.

---

## Configuração do Ambiente

### Pré-requisitos

- .NET 10 SDK
- Docker Desktop (para containers Aspire — PostgreSQL)
- Visual Studio 2022 17.12+ / Rider 2024.3+ / VS Code com extensão C#

### Execução local

```bash
# Restaurar dependências
dotnet restore

# Executar testes
dotnet test

# Rodar via Aspire (API + dashboard de telemetria)
dotnet run --project src/GoodHamburger.AppHost
```

O dashboard do Aspire estará disponível em `https://localhost:15888` com links diretos para a API e o Scalar (documentação interativa).
