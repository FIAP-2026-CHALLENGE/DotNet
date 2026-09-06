# DotNet.Api — API REST para Jornada Contínua de Cuidado Pet

> **Nome temporário do projeto:** `DotNet.Api`
> **Nome comercial:** a definir pelo grupo
> **Contexto:** Challenge FIAP 2026 — solução digital para apoiar a continuidade do cuidado veterinário, com foco em responsável, animal e eventos de cuidado.
> **Escopo desta entrega:** 3º Sprint da disciplina *Advanced Business Development with .NET* — Monitoramento, Observabilidade e Testes Automatizados.

---

## 1. Visão Geral

A `DotNet.Api` é uma API RESTful desenvolvida em **ASP.NET Core Web API** para representar o núcleo back-end de uma solução de acompanhamento contínuo da jornada de saúde de animais de estimação.

O objetivo da API é permitir o cadastro e gerenciamento de:

- Responsáveis;
- Animais vinculados a responsáveis;
- Eventos de cuidado vinculados aos animais.

A proposta é estruturar dados essenciais para que uma aplicação web/app possa consultar, registrar e acompanhar informações relacionadas ao cuidado preventivo, terapêutico e recorrente de cada animal.

Nesta versão, além do CRUD funcional, a API conta com uma arquitetura em camadas (Controller → Serviço → Repositório), Health Check de conectividade com o Oracle, logging estruturado com rastreabilidade por requisição (Correlation ID), instrumentação de observabilidade via OpenTelemetry (Tracing e Métricas), e uma suíte de testes automatizados (unitários e de integração), organizados em projetos separados por camada.

Nesta primeira versão, o MVP trabalha com **cães** e **gatos**, mantendo a arquitetura preparada para evolução futura para outras espécies.

---

## 2. Problema de Negócio

A jornada de saúde do pet costuma ser fragmentada. O responsável normalmente procura atendimento veterinário apenas em situações pontuais, como vacinação, sintomas, emergência ou retorno solicitado.

Essa fragmentação gera problemas como:

- Esquecimento de vacinas, retornos e check-ups;
- Baixa continuidade no tratamento;
- Falta de histórico centralizado do animal;
- Dificuldade para o responsável acompanhar cuidados recorrentes;
- Menor recorrência e fidelização para clínicas e profissionais veterinários.

A API proposta atua como base para uma plataforma que organiza a relação entre responsável, animal e eventos de cuidado.

---

## 3. Objetivo do MVP

O MVP da API tem como objetivo entregar um fluxo funcional mínimo:

1. Cadastrar um responsável;
2. Cadastrar um animal vinculado ao responsável;
3. Cadastrar eventos de cuidado vinculados ao animal;
4. Consultar a jornada de cuidado do animal;
5. Filtrar eventos por status, tipo e animal;
6. Persistir os dados em banco Oracle via Entity Framework Core;
7. Expor um endpoint de Health Check para monitoramento;
8. Registrar logs estruturados e rastreáveis por requisição;
9. Expor métricas e tracing via OpenTelemetry;
10. Garantir cobertura de testes automatizados nas camadas de Domínio, Serviço e Integração.

Fluxo principal:

```text
Responsavel → Animal → CareEvent
```

Exemplo:

```text
Responsavel: Mariana Oliveira
Animal: Thor
Evento: Check-up respiratório
Status: PENDING
Prioridade: HIGH
```

---

## 4. Tecnologias Utilizadas

| Tecnologia | Uso |
|---|---|
| C# / .NET 10 | Linguagem e runtime principal |
| ASP.NET Core Web API | Criação da API REST |
| Entity Framework Core | ORM para persistência |
| Oracle.EntityFrameworkCore | Provider Oracle para EF Core |
| Oracle Database | Banco de dados relacional |
| Swagger / Swashbuckle | Documentação e testes dos endpoints |
| User Secrets | Armazenamento local seguro da connection string |
| Serilog (Console + File) | Logging estruturado com níveis (Information/Warning) e Correlation ID |
| OpenTelemetry | Tracing (Spans manuais e automáticos) e Métricas customizadas |
| Health Checks (`Microsoft.Extensions.Diagnostics.HealthChecks`) | Endpoint `/health` com verificação real de conectividade Oracle |
| xUnit + Moq + FluentAssertions | Testes unitários de Domínio e de Serviço (padrão AAA) |
| Microsoft.AspNetCore.Mvc.Testing | Testes de integração via `WebApplicationFactory` |
| Microsoft.EntityFrameworkCore.InMemory | Banco em memória usado exclusivamente pelos testes de integração |
| Git / GitHub | Versionamento do projeto |

---

## 5. Arquitetura da Aplicação

```text
DotNet.Api/
├── Controllers/
│   ├── ResponsaveisController.cs
│   ├── AnimaisController.cs
│   └── CareEventsController.cs
│
├── Servicos/                          (regra de negócio / orquestração)
│   ├── IResponsavelServico.cs / ResponsavelServico.cs
│   ├── IAnimalServico.cs / AnimalServico.cs
│   └── ICareEventServico.cs / CareEventServico.cs
│
├── Repositorios/                      (acesso a dado puro, sem regra)
│   ├── IResponsavelRepositorio.cs / ResponsavelRepositorio.cs
│   ├── IAnimalRepositorio.cs / AnimalRepositorio.cs
│   └── ICareEventRepositorio.cs / CareEventRepositorio.cs
│
├── Excecoes/                          (exceções de domínio)
│   ├── RegraDeNegocioException.cs     (→ 400 Bad Request)
│   └── RecursoNaoEncontradoException.cs (→ 404 Not Found)
│
├── Aplicacao/Middlewares/
│   └── CorrelationIdMiddleware.cs
│
├── Infraestrutura/
│   ├── Health/OracleHealthCheck.cs
│   └── Observabilidade/AplicacaoMetricas.cs
│
├── Data/
│   └── AppDbContext.cs
│
├── Models/
│   ├── Responsavel.cs   (validação de domínio via Criar/Atualizar)
│   ├── Animal.cs
│   └── CareEvent.cs
│
├── Migrations/
│
├── Program.cs
├── appsettings.json
└── DotNet.Api.csproj

DotNet.Api.UnitTests/                  (testes de Domínio e de Serviço)
├── Dominio/
│   ├── ResponsavelTests.cs
│   ├── AnimalTests.cs
│   └── CareEventTests.cs
└── Servicos/
    ├── ResponsavelServicoTests.cs
    ├── AnimalServicoTests.cs
    └── CareEventServicoTests.cs

DotNet.Api.IntegrationTests/            (testes end-to-end via HTTP)
├── CustomWebApplicationFactory.cs
├── IntegrationTestsCollection.cs      (Collection Fixture)
├── HealthEndpointTests.cs
└── AnimaisEndpointsTests.cs
```

### Responsabilidade das pastas

| Pasta/Arquivo | Responsabilidade |
|---|---|
| `Controllers` | Recebe requisições HTTP, delega para os Serviços, registra logs (`ILogger`) e traduz exceções em códigos HTTP |
| `Servicos` | Orquestra regras de negócio entre entidades (ex: Animal só existe se Responsavel existir) |
| `Repositorios` | Acesso a dado puro via EF Core, sem nenhuma regra de negócio |
| `Excecoes` | Exceções de domínio usadas para sinalizar 400 (regra de negócio) e 404 (não encontrado) |
| `Aplicacao/Middlewares` | Middlewares customizados (Correlation ID) |
| `Infraestrutura/Health` | Health Check customizado de conectividade com o Oracle |
| `Infraestrutura/Observabilidade` | ActivitySource (Tracing) e Meter/Counter (Métricas) do OpenTelemetry |
| `Models` | Representa as entidades do domínio, com validação própria (`Criar`/`Atualizar`) |
| `Data/AppDbContext.cs` | Configura o EF Core e o mapeamento das tabelas Oracle |
| `Migrations` | Histórico de alterações do schema geradas pelo EF Core |
| `Program.cs` | Configuração principal: DI, Serilog, OpenTelemetry, Health Checks e pipeline |
| `appsettings.json` | Configurações públicas e placeholders |
| `DotNet.Api.UnitTests` | Testes de Domínio (entidades isoladas) e de Serviço (Moq mockando Repositórios) |
| `DotNet.Api.IntegrationTests` | Testes HTTP end-to-end via `WebApplicationFactory`, com banco EF Core InMemory |

---

## 6. Modelo de Domínio

Todas as entidades expõem métodos estáticos `Criar(...)` (fábrica com validação) e um método `Atualizar(...)` (revalida as mesmas regras). Isso garante que a entidade nunca exista em um estado inconsistente, independentemente de quem a esteja criando (Controller, Serviço ou um teste automatizado).

### 6.1. Responsavel

| Campo | Tipo | Descrição |
|---|---|---|
| `Id` | int | Identificador do responsável |
| `Name` | string | Nome do responsável (obrigatório) |
| `Email` | string | E-mail do responsável (obrigatório) |
| `Phone` | string | Telefone (obrigatório) |
| `Cpf` | string | CPF (obrigatório, único) |
| `CreatedAt` | DateTime | Data de cadastro |
| `IsActive` | bool | Indica se o registro está ativo |

### 6.2. Animal

| Campo | Tipo | Descrição |
|---|---|---|
| `Id` | int | Identificador do animal |
| `ResponsavelId` | int | ID do responsável pelo animal |
| `Name` | string | Nome do animal |
| `Nickname` | string | Apelido do animal |
| `Species` | string | Espécie do animal (`DOG` ou `CAT`) |
| `Breed` | string | Raça |
| `BirthDate` | DateTime | Data de nascimento |
| `Weight` | decimal | Peso (deve ser maior que zero) |
| `Sex` | string | Sexo |
| `Rga` | string | Registro Geral Animal (único quando informado) |
| `CreatedAt` | DateTime | Data de cadastro |
| `IsActive` | bool | Indica se o registro está ativo |

Espécies permitidas nesta versão:

```text
DOG
CAT
```

### 6.3. CareEvent

| Campo | Tipo | Descrição |
|---|---|---|
| `Id` | int | Identificador do evento |
| `PetId` | int | ID do animal relacionado |
| `Type` | string | Tipo do evento |
| `Title` | string | Título do evento |
| `Description` | string? | Descrição (opcional) |
| `ScheduledDate` | DateTime | Data prevista |
| `CompletedDate` | DateTime? | Data de conclusão |
| `Status` | string | Status do evento |
| `Priority` | string | Prioridade |
| `Notes` | string? | Observações (opcional) |
| `CreatedAt` | DateTime | Data de criação |
| `IsActive` | bool | Indica se o registro está ativo |

> **Nota técnica:** `Description` e `Notes` são opcionais (`string?`). Quando o valor recebido é vazio ou nulo, a entidade normaliza para `null` — nunca para string vazia (`""`). Isso evita um comportamento conhecido do Oracle, que trata `''` como sinônimo de `NULL` em colunas `VARCHAR2`, o que causava violação de constraint `NOT NULL` em inserções com texto vazio.

Tipos permitidos:

```text
VACCINE / DEWORMING / MEDICATION / CHECKUP
RETURN / EXAM / GROOMING / SURGERY / OTHER
```

Status permitidos:

```text
PENDING / COMPLETED / OVERDUE / CANCELED
```

Prioridades permitidas:

```text
LOW / MEDIUM / HIGH / CRITICAL
```

---

## 7. Banco de Dados

A aplicação utiliza **Oracle Database** com **Entity Framework Core**.

### 7.1. Tabelas

| Entidade | Tabela Oracle |
|---|---|
| `Responsavel` | `T_CP_RESPONSAVEIS` |
| `Animal` | `T_CP_ANIMAIS` |
| `CareEvent` | `T_CP_CARE_EVENTS` |

O EF Core também mantém a tabela interna:

```text
__EFMigrationsHistory
```

### 7.2. Relacionamentos

```text
T_CP_RESPONSAVEIS  1 ─── N  T_CP_ANIMAIS
T_CP_ANIMAIS       1 ─── N  T_CP_CARE_EVENTS
```

### 7.3. Migrations aplicadas

| Migration | Descrição |
|---|---|
| `20260516225751_InitialCreate` | Criação inicial das tabelas |
| `20260517175752_AddForeignKeys` | Adição das chaves estrangeiras |
| `20260524175656_RenameEntitiesToAnimalAndResponsavel` | Renomeação das entidades e tabelas |
| `20260906194515_AllowNullNotesAndDescriptionOnCareEvents` | Permite `NULL` nas colunas `NOTES` e `DESCRIPTION` de `T_CP_CARE_EVENTS` |

### 7.4. Configuração de Boolean no Oracle

Os campos booleanos foram mapeados como `NUMBER(1)`:

```text
true  → 1
false → 0
```

> **Nota técnica:** consultas LINQ que usam `.AnyAsync(...)` geram, neste provider Oracle, literais `True`/`False` na cláusula SQL — o que o Oracle não reconhece (`ORA-00904: identificador inválido`). Por isso, os métodos de verificação de existência (`ExisteAsync`, `CpfExisteAsync`, `RgaExisteAsync`) nos Repositórios usam `.CountAsync(...) > 0` no lugar de `.AnyAsync(...)`.

---

## 8. Configuração de Credenciais

O arquivo `appsettings.json` contém apenas um placeholder:

```json
{
  "ConnectionStrings": {
    "OracleConnection": "User Id=RMXXXXXX;Password=SUA_SENHA;Data Source=oracle.fiap.com.br:1521/orcl;"
  },
  "AllowedHosts": "*"
}
```

A connection string real deve ser configurada via **User Secrets**:

```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:OracleConnection" "User Id=SEU_RM;Password=SUA_SENHA_REAL;Data Source=oracle.fiap.com.br:1521/orcl;"
```

> **Atenção:** comandos `dotnet ef` (migrations) rodam em ambiente **Production** por padrão e não carregam os User Secrets. Sempre defina o ambiente antes de rodar:
> ```powershell
> $env:ASPNETCORE_ENVIRONMENT="Development"
> ```

---

## 9. Como Executar o Projeto

### 9.1. Pré-requisitos

- .NET SDK 10 instalado
- Oracle Database acessível
- Credenciais Oracle válidas
- Git
- Insomnia, Postman ou Swagger para testes

### 9.2. Clonar o repositório

```bash
git clone https://github.com/FIAP-2026-CHALLENGE/DotNet
cd DotNet/DotNet.Api
```

### 9.3. Restaurar dependências

```bash
dotnet restore
```

### 9.4. Configurar User Secrets

```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:OracleConnection" "User Id=SEU_RM;Password=SUA_SENHA_REAL;Data Source=oracle.fiap.com.br:1521/orcl;"
```

### 9.5. Aplicar migrations

```powershell
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet ef database update
```

### 9.6. Executar a API

```bash
dotnet run
```

A aplicação sobe em:

```text
http://localhost:5296
```

Swagger disponível em:

```text
http://localhost:5296/swagger
```

Health Check disponível em:

```text
http://localhost:5296/health
```

---

## 10. Endpoints da API

Base URL local: `http://localhost:5296`

### 10.1. Responsaveis

| Método | Rota | Descrição | Retornos |
|---|---|---|---|
| GET | `/api/responsaveis` | Lista todos os responsáveis | 200 |
| GET | `/api/responsaveis/{id}` | Busca responsável por ID | 200, 404 |
| GET | `/api/responsaveis/cpf/{cpf}` | Busca responsável por CPF | 200, 404 |
| POST | `/api/responsaveis` | Cria um responsável | 201, 400 |
| PUT | `/api/responsaveis/{id}` | Atualiza um responsável | 204, 400, 404 |
| DELETE | `/api/responsaveis/{id}` | Remove um responsável | 204, 404 |

#### Exemplo de POST `/api/responsaveis`

```json
{
  "name": "Mariana Oliveira",
  "email": "mariana.oliveira@email.com",
  "phone": "11977776666",
  "cpf": "45678912300"
}
```

### 10.2. Animais

| Método | Rota | Descrição | Retornos |
|---|---|---|---|
| GET | `/api/animais` | Lista todos os animais | 200 |
| GET | `/api/animais/{id}` | Busca animal por ID | 200, 404 |
| GET | `/api/animais/responsavel/{responsavelId}` | Lista animais de um responsável | 200, 404 |
| GET | `/api/animais/species/{species}` | Lista animais por espécie | 200 |
| GET | `/api/animais/breed/{breed}` | Lista animais por raça | 200 |
| GET | `/api/animais/rga/{rga}` | Busca animal por RGA | 200, 404 |
| POST | `/api/animais` | Cria um animal | 201, 400 |
| PUT | `/api/animais/{id}` | Atualiza um animal | 204, 400, 404 |
| DELETE | `/api/animais/{id}` | Remove um animal | 204, 404 |

#### Exemplo de POST `/api/animais`

```json
{
  "responsavelId": 1,
  "name": "Thor",
  "nickname": "Toto",
  "species": "DOG",
  "breed": "Pug",
  "birthDate": "2020-05-10T00:00:00",
  "weight": 8.5,
  "sex": "MALE",
  "rga": "RGA123456"
}
```

### 10.3. CareEvents

| Método | Rota | Descrição | Retornos |
|---|---|---|---|
| GET | `/api/care-events` | Lista todos os eventos | 200 |
| GET | `/api/care-events/{id}` | Busca evento por ID | 200, 404 |
| GET | `/api/care-events/animal/{animalId}` | Lista eventos de um animal | 200, 404 |
| GET | `/api/care-events/status/{status}` | Lista eventos por status | 200, 400 |
| GET | `/api/care-events/type/{type}` | Lista eventos por tipo | 200, 400 |
| GET | `/api/care-events/animal/{animalId}/status/{status}` | Lista eventos de um animal por status | 200, 400, 404 |
| GET | `/api/care-events/overdue` | Lista eventos atrasados | 200 |
| POST | `/api/care-events` | Cria evento de cuidado | 201, 400 |
| PUT | `/api/care-events/{id}` | Atualiza evento de cuidado | 204, 400, 404 |
| PATCH | `/api/care-events/{id}/complete` | Conclui evento de cuidado | 204, 400, 404 |
| DELETE | `/api/care-events/{id}` | Remove evento de cuidado | 204, 404 |

#### Exemplo de POST `/api/care-events`

```json
{
  "petId": 1,
  "type": "CHECKUP",
  "title": "Check-up respiratório",
  "description": "Avaliação preventiva por conta da raça Pug.",
  "scheduledDate": "2026-06-10T10:00:00",
  "completedDate": null,
  "status": "PENDING",
  "priority": "HIGH",
  "notes": "Pugs podem exigir atenção respiratória preventiva."
}
```

### 10.4. Diagnóstico

| Método | Rota | Descrição | Retornos |
|---|---|---|---|
| GET | `/health` | Verifica a saúde da aplicação (conectividade real com o Oracle) | 200, 503 |

---

## 11. Retornos HTTP Implementados

| Código | Uso |
|---|---|
| `200 OK` | Consulta realizada com sucesso |
| `201 Created` | Recurso criado com sucesso |
| `204 No Content` | Atualização, remoção ou conclusão realizada com sucesso |
| `400 Bad Request` | Dados inválidos ou regra de negócio violada |
| `404 Not Found` | Recurso não encontrado |
| `503 Service Unavailable` | Health Check indicando falha de conectividade com o Oracle |

---

## 12. Fluxo de Teste Recomendado

1. Verificar a saúde da aplicação via `GET /health`;
2. Criar um responsável via `POST /api/responsaveis`;
3. Listar responsáveis via `GET /api/responsaveis`;
4. Criar um animal usando o `id` do responsável via `POST /api/animais`;
5. Listar animais do responsável via `GET /api/animais/responsavel/{responsavelId}`;
6. Criar um evento de cuidado usando o `id` do animal via `POST /api/care-events`;
7. Listar eventos do animal via `GET /api/care-events/animal/{animalId}`;
8. Filtrar eventos por status via `GET /api/care-events/status/PENDING`;
9. Concluir um evento via `PATCH /api/care-events/{id}/complete`;
10. Consultar novamente o evento concluído;
11. Validar os registros no Oracle;
12. Conferir, no terminal onde a API está rodando, os logs estruturados (com Correlation ID e níveis Information/Warning) e os blocos de Tracing/Métricas do OpenTelemetry.

---

## 13. Consultas SQL para Validação

```sql
SELECT * FROM T_CP_RESPONSAVEIS;
SELECT * FROM T_CP_ANIMAIS;
SELECT * FROM T_CP_CARE_EVENTS;
```

Validação completa com JOIN:

```sql
SELECT
    r.ID AS RESPONSAVEL_ID,
    r.NAME AS RESPONSAVEL_NAME,
    a.ID AS ANIMAL_ID,
    a.NAME AS ANIMAL_NAME,
    a.SPECIES,
    a.BREED,
    ce.ID AS EVENT_ID,
    ce.TYPE,
    ce.TITLE,
    ce.STATUS,
    ce.PRIORITY,
    ce.SCHEDULED_DATE
FROM T_CP_RESPONSAVEIS r
INNER JOIN T_CP_ANIMAIS a
    ON a.RESPONSAVEL_ID = r.ID
INNER JOIN T_CP_CARE_EVENTS ce
    ON ce.PET_ID = a.ID
ORDER BY r.ID, a.ID, ce.ID;
```

---

## 14. Regras de Negócio Implementadas

### Responsaveis
- Nome, e-mail, telefone e CPF são obrigatórios;
- Não permite CPF duplicado.

### Animais
- Um animal só pode ser criado se o responsável existir;
- Nome, espécie, raça, sexo e peso válido (maior que zero) são obrigatórios;
- O MVP aceita apenas `DOG` e `CAT`;
- Não permite RGA duplicado quando informado.

### Eventos de Cuidado
- Um evento só pode ser criado se o animal existir;
- Tipo, título, status, prioridade e data prevista são obrigatórios; descrição e observações são opcionais;
- O tipo, status e prioridade devem estar dentro das listas permitidas;
- Eventos cancelados não podem ser concluídos;
- Ao concluir um evento, o status é alterado para `COMPLETED` e a data de conclusão é preenchida automaticamente, se ainda não informada.

---

## 15. Arquitetura em Camadas (Repository + Service)

A aplicação segue uma separação explícita de responsabilidades:

- **Controllers** — recebem a requisição HTTP, chamam o Serviço correspondente, registram logs da operação e traduzem o resultado (ou a exceção capturada) em um código de status HTTP. Não conhecem `AppDbContext` nem regra de negócio.
- **Serviços** (`Servicos/`) — orquestram as regras de negócio, muitas vezes consultando mais de um Repositório (ex: `AnimalServico` valida se o `ResponsavelId` existe antes de criar um Animal). Lançam `RegraDeNegocioException` ou `RecursoNaoEncontradoException` quando uma regra é violada.
- **Repositórios** (`Repositorios/`) — acesso a dado puro via EF Core, sem nenhuma regra de negócio embutida.
- **Models** — validam a si mesmas através de métodos estáticos `Criar(...)` e de instância `Atualizar(...)`, lançando `ArgumentException` quando os dados são inválidos. Isso garante que a entidade nunca exista em estado inconsistente, independentemente de quem a está criando.

Essa separação existe, sobretudo, para permitir testes de Serviço isolados via `Mock<IRepositorio>` (Moq), sem depender do Oracle real.

---

## 16. Health Checks & Diagnósticos

A aplicação expõe o endpoint nativo do ASP.NET Core, via `Microsoft.Extensions.Diagnostics.HealthChecks`, em:

```text
GET /health
```

O health check customizado (`OracleHealthCheck`, em `Infraestrutura/Health`) executa `Database.CanConnectAsync()` contra o Oracle real da FIAP a cada chamada — ou seja, o `/health` só retorna `Healthy` se a API conseguir abrir conexão de fato com o banco, não é um "ping" estático.

| Cenário | Resposta |
|---|---|
| Oracle acessível | `200 OK` — `Healthy` |
| Oracle inacessível / credencial inválida | `503 Service Unavailable` — `Unhealthy` |

**Como monitorar:** basta fazer `GET /health` periodicamente (de um load balancer, de um script de monitoramento, ou manualmente via Swagger/Insomnia/Postman). O endpoint não exige autenticação e responde rapidamente, sendo seguro para checagens automáticas frequentes.

---

## 17. Logging Estruturado & Correlation ID

O logging é feito com **Serilog**, configurado em `Program.cs`, com dois destinos (sinks):

- **Console** — para acompanhamento em tempo real durante o desenvolvimento e no vídeo de demonstração;
- **Arquivo rotativo diário** — `logs/app-{data}.log`, útil para auditoria e para anexar evidência.

### Níveis de log utilizados

- **Information**: emitido automaticamente pelo `UseSerilogRequestLogging()` para toda requisição, e manualmente pelos Controllers a cada operação de escrita bem-sucedida (`_logger.LogInformation(...)` em `Create`, `Update`, `Delete` e `Complete`).
- **Warning**: emitido manualmente pelos Controllers sempre que uma exceção de negócio é capturada (`ArgumentException` ou `RegraDeNegocioException`), sinalizando uma requisição rejeitada por dado inválido ou regra violada.
- **Error/Fatal**: reservado para exceções não tratadas que sobem além dos Controllers (ex: falha de conectividade inesperada com o Oracle), capturadas pelo bloco `try/catch/finally` do Serilog em `Program.cs`.

### Correlation ID

Cada requisição HTTP recebe (ou propaga, se já vier no header) um **Correlation ID**, injetado pelo `CorrelationIdMiddleware` (`Aplicacao/Middlewares`) no `LogContext` do Serilog. Isso significa que **todo log emitido durante aquela requisição** — inclusive o log automático de acesso e os logs manuais dos Controllers — carrega o mesmo Correlation ID, permitindo rastrear o fluxo completo de uma requisição específica no meio de milhares de linhas de log.

- Header de entrada/saída: `X-Correlation-ID`
- Se o cliente não enviar o header, a API gera um novo `Guid` automaticamente e devolve na resposta.

Exemplo de linha de log gerada:

```text
[14:32:07 INF] (3f9a2b91-...) Animal 12 (Thor) criado com sucesso para Responsavel 4.
[14:33:02 WRN] (7a1c9d44-...) Falha ao criar Animal: ResponsavelId does not exist.
```

---

## 18. Observabilidade com OpenTelemetry

Instrumentação manual centralizada em `Infraestrutura/Observabilidade/AplicacaoMetricas.cs`:

- **Tracing**: um `ActivitySource` nomeado `ClyvoVet.Api`, usado para abrir **Spans customizados** nos endpoints de escrita (`POST /api/responsaveis`, `POST /api/animais`, `POST /api/care-events`), com tags relevantes do domínio (ex: `animal.species`, `careEvent.petId`). Além disso, `AddAspNetCoreInstrumentation()` e `AddHttpClientInstrumentation()` geram Spans automáticos para toda requisição HTTP recebida/feita pela aplicação, incluindo tempo de resposta.
- **Métricas**: um `Counter<long>` (`clyvovet.cadastros.total`) incrementado a cada cadastro/evento processado, com as tags `recurso` (responsavel/animal/care_event) e `status` (sucesso/falha) — permitindo derivar taxa de erro por tipo de recurso.

Os dados de Tracing e Métricas são exportados para o **Console** (`AddConsoleExporter()`), visíveis diretamente no terminal onde a API está rodando.

---

## 19. Testes Automatizados

Os testes estão organizados em **dois projetos separados**, por camada:

| Projeto | Pastas internas | O que testa | Dependências |
|---|---|---|---|
| `DotNet.Api.UnitTests` | `Dominio/`, `Servicos/` | Regras de validação das entidades e regras de orquestração dos Serviços | `Mock<IRepositorio>` via Moq (nenhuma dependência externa nos testes de Domínio) |
| `DotNet.Api.IntegrationTests` | raiz do projeto | Fluxo HTTP completo (Controller → Serviço → Repositório) | `WebApplicationFactory<Program>` + EF Core InMemory |

Todos os testes seguem o padrão **AAA (Arrange, Act, Assert)** e a nomenclatura `MetodoTestado_Cenario_ResultadoEsperado` (ex: `CriarAsync_ComCpfDuplicado_DeveLancarRegraDeNegocioExceptionENaoDeveAdicionar`).

### Fixtures e Collection Fixtures

O projeto `DotNet.Api.IntegrationTests` utiliza uma **Collection Fixture** (`IntegrationTestsCollection`, com `[CollectionDefinition("Integration Tests")]` e `ICollectionFixture<CustomWebApplicationFactory>`), garantindo que a mesma instância da aplicação em memória (e do mesmo banco InMemory) seja compartilhada entre as classes `HealthEndpointTests` e `AnimaisEndpointsTests`, evitando subir a aplicação mais de uma vez por execução da suíte.

Os testes de integração **não tocam no Oracle real** — a `CustomWebApplicationFactory` substitui o `AppDbContext` por um provider EF Core InMemory isolado por execução, para que `dotnet test` rode de forma determinística, rápida e sem depender de rede ou de credenciais do Oracle da FIAP.

> **Nota sobre autenticação:** autenticação/autorização (JWT ou Identity) é um requisito da 4ª Sprint do Challenge, ainda não entregue. Por isso, os testes de integração desta entrega cobrem o fluxo HTTP completo — sucesso, validação de regra de negócio e recurso não encontrado — sem cenários de autenticação, consistente com o escopo da 3ª Sprint.

### Como executar os testes

```bash
cd DotNet.Api.UnitTests
dotnet test

cd ../DotNet.Api.IntegrationTests
dotnet test
```

Para ver o detalhamento de cada teste no console:

```bash
dotnet test --logger "console;verbosity=detailed"
```

---

## 20. Limitações Conhecidas

- O nome `DotNet.Api` é temporário;
- O MVP trabalha apenas com cães e gatos;
- Autenticação/autorização (JWT/Identity) ainda não implementada — requisito da 4ª Sprint do Challenge;
- Ainda não há DTOs separados para entrada e saída (os Controllers recebem/retornam as próprias entidades de domínio);
- Sem front-end integrado nesta entrega.

---

## 21. Comandos Úteis

```bash
# Build
dotnet build

# Run
dotnet run

# Criar migration (defina o ambiente antes, no PowerShell)
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet ef migrations add NomeDaMigration

# Aplicar migration
dotnet ef database update

# Listar migrations
dotnet ef migrations list

# Listar User Secrets
dotnet user-secrets list

# Rodar os testes unitários (Domínio + Serviço)
cd ../DotNet.Api.UnitTests
dotnet test

# Rodar os testes de integração
cd ../DotNet.Api.IntegrationTests
dotnet test
```

---

## 22. Status Atual

| Item | Status |
|---|---|
| API ASP.NET Core | ✅ Concluído |
| Swagger | ✅ Concluído |
| CRUD de Responsavel | ✅ Concluído |
| CRUD de Animal | ✅ Concluído |
| CRUD de CareEvent | ✅ Concluído |
| Rotas parametrizadas | ✅ Concluído |
| Retornos HTTP principais | ✅ Concluído |
| Oracle + EF Core | ✅ Concluído |
| Migrations | ✅ Concluído |
| Arquitetura Repository + Service | ✅ Concluído |
| Health Check | ✅ Concluído |
| Logging estruturado (Information/Warning) + Correlation ID | ✅ Concluído |
| OpenTelemetry (Tracing + Métricas) | ✅ Concluído |
| Testes de Domínio (`DotNet.Api.UnitTests`) | ✅ Concluído |
| Testes de Serviço com Moq (`DotNet.Api.UnitTests`) | ✅ Concluído |
| Testes de Integração com Collection Fixture (`DotNet.Api.IntegrationTests`) | ✅ Concluído |
| README técnico | ✅ Concluído |