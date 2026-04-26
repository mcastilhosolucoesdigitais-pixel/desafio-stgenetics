# Backlogs — Good Hamburger

## Definição de Pronto (Definition of Done)

Um item é considerado **pronto** quando:

- [ ] O código está implementado e compilando sem erros ou warnings.
- [ ] Os testes unitários e/ou de integração cobrindo o comportamento foram escritos **antes** da implementação (TDD).
- [ ] Todos os testes do projeto passam (`dotnet test` com zero falhas).
- [ ] O código passou por revisão (self-review ou pair) e não possui code smells óbvios.
- [ ] A funcionalidade está integrada à branch `develop` sem conflitos.
- [ ] Os endpoints da API foram validados manualmente via Scalar/Swagger ou cliente HTTP.
- [ ] O `README.md` foi atualizado caso a funcionalidade impacte a execução ou configuração do projeto.

---

## Épicos e User Stories

---

### ÉPICO 1 — Cardápio

#### US-01 — Consultar cardápio
> **Como** cliente da lanchonete,  
> **Quero** consultar o cardápio disponível,  
> **Para que** eu possa escolher os itens do meu pedido.

**Critérios de Aceite:**

- [ ] `GET /menu` retorna HTTP 200 com a lista de itens agrupados por categoria (Sanduíches e Acompanhamentos).
- [ ] Cada item contém: `id`, `name`, `category`, `price`.
- [ ] Os preços exibidos são: X Burger R$ 5,00 | X Egg R$ 4,50 | X Bacon R$ 7,00 | Batata Frita R$ 2,00 | Refrigerante R$ 2,50.
- [ ] O endpoint não requer autenticação.
- [ ] A resposta é serializada em JSON com chaves em camelCase.

**Testes a escrever (TDD):**
- `GET /menu` retorna status 200.
- `GET /menu` retorna exatamente 5 itens.
- Cada item da resposta possui os campos obrigatórios.
- Itens de sanduíche pertencem à categoria `"Sandwich"`.
- Itens de acompanhamento pertencem à categoria `"Side"`.

---

### ÉPICO 2 — Pedidos (CRUD)

#### US-02 — Criar pedido
> **Como** atendente da lanchonete,  
> **Quero** criar um novo pedido com os itens escolhidos pelo cliente,  
> **Para que** o pedido seja registrado com subtotal, desconto e total calculados automaticamente.

**Critérios de Aceite:**

- [ ] `POST /orders` aceita um JSON com a lista de IDs dos itens selecionados.
- [ ] A resposta HTTP é 201 Created com o pedido criado (incluindo `id`, `items`, `subtotal`, `discount`, `total`).
- [ ] O pedido pode conter: 1 sanduíche (obrigatório), 0 ou 1 batata frita, 0 ou 1 refrigerante.
- [ ] Ao incluir sanduíche + batata + refrigerante, o desconto aplicado é 20%.
- [ ] Ao incluir sanduíche + refrigerante (sem batata), o desconto aplicado é 15%.
- [ ] Ao incluir sanduíche + batata (sem refrigerante), o desconto aplicado é 10%.
- [ ] Ao incluir apenas sanduíche (sem acompanhamentos), o desconto é 0%.
- [ ] `subtotal` = soma dos preços sem desconto.
- [ ] `discount` = valor monetário descontado (não o percentual).
- [ ] `total` = `subtotal` - `discount`.
- [ ] Retorna HTTP 422 com mensagem de erro se o pedido não contiver nenhum sanduíche.
- [ ] Retorna HTTP 422 com mensagem de erro se o pedido contiver mais de um item da mesma categoria.
- [ ] Retorna HTTP 422 com mensagem de erro se algum `itemId` informado não existir no cardápio.

**Testes a escrever (TDD):**
- Cálculo de desconto 20% para combinação completa.
- Cálculo de desconto 15% para sanduíche + refrigerante.
- Cálculo de desconto 10% para sanduíche + batata.
- Cálculo de 0% de desconto para apenas sanduíche.
- Subtotal, discount e total calculados corretamente para cada combinação.
- Pedido sem sanduíche retorna erro 422.
- Pedido com dois sanduíches retorna erro 422.
- Pedido com duas batatas retorna erro 422.
- Pedido com dois refrigerantes retorna erro 422.
- Pedido com item de ID inexistente retorna erro 422.
- Pedido válido retorna 201 com o corpo correto.

---

#### US-03 — Listar pedidos
> **Como** gerente da lanchonete,  
> **Quero** visualizar todos os pedidos registrados,  
> **Para que** eu possa acompanhar o movimento do estabelecimento.

**Critérios de Aceite:**

- [ ] `GET /orders` retorna HTTP 200 com array de pedidos.
- [ ] Cada pedido exibe `id`, `items`, `subtotal`, `discount` e `total`.
- [ ] Quando não houver pedidos, retorna 200 com array vazio `[]`.
- [ ] Os pedidos são retornados em ordem de criação (mais antigo primeiro).

**Testes a escrever (TDD):**
- `GET /orders` retorna 200.
- `GET /orders` retorna array vazio quando não há pedidos.
- `GET /orders` retorna os pedidos criados anteriormente com os campos corretos.

---

#### US-04 — Consultar pedido por ID
> **Como** atendente,  
> **Quero** consultar os detalhes de um pedido específico pelo seu identificador,  
> **Para que** eu possa verificar os itens e valores de um pedido individual.

**Critérios de Aceite:**

- [ ] `GET /orders/{id}` retorna HTTP 200 com os dados do pedido quando o ID existe.
- [ ] Retorna HTTP 404 com mensagem de erro clara quando o pedido não é encontrado.
- [ ] A resposta contém `id`, `items`, `subtotal`, `discount` e `total`.

**Testes a escrever (TDD):**
- `GET /orders/{id}` existente retorna 200 com dados corretos.
- `GET /orders/{id}` inexistente retorna 404.
- Corpo do 404 contém campo `error` com mensagem descritiva.

---

#### US-05 — Atualizar pedido
> **Como** atendente,  
> **Quero** atualizar os itens de um pedido existente,  
> **Para que** eu possa corrigir escolhas feitas pelo cliente antes do preparo.

**Critérios de Aceite:**

- [ ] `PUT /orders/{id}` aceita um JSON com a nova lista de IDs de itens.
- [ ] Retorna HTTP 200 com o pedido atualizado (subtotal, desconto e total recalculados).
- [ ] Retorna HTTP 404 quando o pedido não existe.
- [ ] Aplica as mesmas regras de validação e desconto do POST.
- [ ] Retorna HTTP 422 para qualquer violação das regras de negócio.

**Testes a escrever (TDD):**
- Atualização válida retorna 200 com dados recalculados.
- Atualização de pedido inexistente retorna 404.
- Atualização com itens duplicados retorna 422.
- Atualização sem sanduíche retorna 422.
- Desconto é recalculado corretamente após atualização.

---

#### US-06 — Remover pedido
> **Como** atendente,  
> **Quero** remover um pedido do sistema,  
> **Para que** pedidos cancelados não poluam o registro.

**Critérios de Aceite:**

- [ ] `DELETE /orders/{id}` retorna HTTP 204 No Content quando o pedido é removido com sucesso.
- [ ] Retorna HTTP 404 com mensagem clara quando o pedido não existe.
- [ ] Após a remoção, `GET /orders/{id}` retorna 404.

**Testes a escrever (TDD):**
- `DELETE /orders/{id}` existente retorna 204.
- `DELETE /orders/{id}` inexistente retorna 404.
- Após deleção, GET do mesmo ID retorna 404.

---

### ÉPICO 3 — Qualidade e Infraestrutura

#### US-07 — Configuração do projeto com .NET Aspire 10
> **Como** desenvolvedor,  
> **Quero** que o projeto seja orquestrado pelo .NET Aspire 10,  
> **Para que** o ambiente local seja simples de iniciar e próximo do ambiente de produção.

**Critérios de Aceite:**

- [ ] O projeto possui `AppHost` e `ServiceDefaults` do Aspire configurados.
- [ ] `dotnet run --project src/GoodHamburger.AppHost` inicia API, banco e dashboard sem configuração adicional.
- [ ] O dashboard do Aspire exibe os serviços registrados com health check verde.
- [ ] A API está configurada com OpenAPI/Scalar acessível via dashboard.

---

#### US-08 — Testes automatizados (TDD)
> **Como** desenvolvedor,  
> **Quero** cobertura de testes nas regras de negócio e nos endpoints,  
> **Para que** regressões sejam detectadas automaticamente.

**Critérios de Aceite:**

- [ ] Projeto `GoodHamburger.UnitTests` cobre `DiscountCalculator` e `OrderValidator` com 100% dos cenários de domínio.
- [ ] Projeto `GoodHamburger.IntegrationTests` cobre todos os endpoints com os cenários de aceite acima.
- [ ] `dotnet test` passa com zero falhas a partir de um clone limpo do repositório.
- [ ] Cobertura de código ≥ 80% nas camadas de domínio e serviços.

---

#### US-09 — CI com GitHub Actions
> **Como** desenvolvedor,  
> **Quero** um pipeline de CI que execute os testes a cada push,  
> **Para que** a branch main nunca contenha código com testes quebrados.

**Critérios de Aceite:**

- [ ] Arquivo `.github/workflows/ci.yml` presente no repositório.
- [ ] O pipeline executa `dotnet restore`, `dotnet build` e `dotnet test` em cada push e pull request.
- [ ] O pipeline falha e bloqueia o merge se algum teste falhar.
- [ ] O pipeline roda em ubuntu-latest com .NET 10.

---

### ÉPICO 4 — Frontend Blazor (Diferencial)

#### US-10 — Visualizar cardápio no Blazor
> **Como** cliente,  
> **Quero** acessar o cardápio pelo navegador,  
> **Para que** eu veja os itens e preços disponíveis de forma visual.

**Critérios de Aceite:**

- [ ] Página `/menu` exibe todos os itens agrupados por categoria.
- [ ] Cada card de item exibe nome e preço formatado em R$.
- [ ] A página consome `GET /menu` da API via `HttpClient`.
- [ ] Estado de carregamento é exibido enquanto a API responde.

---

#### US-11 — Gerenciar pedidos no Blazor
> **Como** atendente,  
> **Quero** criar e visualizar pedidos pela interface web,  
> **Para que** eu não precise usar ferramentas de linha de comando.

**Critérios de Aceite:**

- [ ] Página `/orders` lista todos os pedidos existentes com subtotal, desconto e total.
- [ ] Formulário de criação de pedido permite selecionar até 1 sanduíche, 1 batata e 1 refrigerante via checkboxes/selects.
- [ ] Ao submeter, o pedido é criado via `POST /orders` e a lista é atualizada.
- [ ] Erros de validação retornados pela API são exibidos ao usuário de forma amigável.
- [ ] Botão de exclusão remove o pedido via `DELETE /orders/{id}` com confirmação.

---

## Priorização (MoSCoW)

| ID | User Story | Prioridade |
|---|---|---|
| US-01 | Consultar cardápio | Must Have |
| US-02 | Criar pedido | Must Have |
| US-03 | Listar pedidos | Must Have |
| US-04 | Consultar pedido por ID | Must Have |
| US-05 | Atualizar pedido | Must Have |
| US-06 | Remover pedido | Must Have |
| US-07 | Configuração Aspire 10 | Must Have |
| US-08 | Testes automatizados (TDD) | Must Have |
| US-09 | CI com GitHub Actions | Should Have |
| US-10 | Cardápio no Blazor | Could Have |
| US-11 | Pedidos no Blazor | Could Have |
