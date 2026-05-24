# DotNet.Api — API REST para Jornada Contínua de Cuidado Pet

> **Nome temporário do projeto:** `DotNet.Api`  
> **Nome comercial:** a definir pelo grupo  
> **Contexto:** Challenge FIAP 2026 — solução digital para apoiar a continuidade do cuidado veterinário, com foco em responsável, animal e eventos de cuidado.

---

## 1. Visão Geral

A `DotNet.Api` é uma API RESTful desenvolvida em **ASP.NET Core Web API** para representar o núcleo back-end de uma solução de acompanhamento contínuo da jornada de saúde de animais de estimação.

O objetivo da API é permitir o cadastro e gerenciamento de:

- Responsáveis;
- Animais vinculados a responsáveis;
- Eventos de cuidado vinculados aos animais.

A proposta é estruturar dados essenciais para que uma aplicação web/app possa consultar, registrar e acompanhar informações relacionadas ao cuidado preventivo, terapêutico e recorrente de cada animal.

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
6. Persistir os dados em banco Oracle via Entity Framework Core.

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
| C# | Linguagem principal |
| ASP.NET Core Web API | Criação da API REST |
| Entity Framework Core | ORM para persistência |
| Oracle.EntityFrameworkCore | Provider Oracle para EF Core |
| Oracle Database | Banco de dados relacional |
| Swagger / Swashbuckle | Documentação e testes dos endpoints |
| User Secrets | Armazenamento local seguro da connection string |
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
├── Data/
│   └── AppDbContext.cs
│
├── Models/
│   ├── Responsavel.cs
│   ├── Animal.cs
│   └── CareEvent.cs
│
├── Migrations/
│
├── Program.cs
├── appsettings.json
└── DotNet.Api.csproj
```

### Responsabilidade das pastas

| Pasta/Arquivo | Responsabilidade |
|---|---|
| `Controllers` | Recebe requisições HTTP e retorna respostas REST |
| `Models` | Representa as entidades do domínio |
| `Data/AppDbContext.cs` | Configura o EF Core e o mapeamento das tabelas Oracle |
| `Migrations` | Histórico de alterações do schema geradas pelo EF Core |
| `Program.cs` | Configuração principal da aplicação |
| `appsettings.json` | Configurações públicas e placeholders |

---

## 6. Modelo de Domínio

### 6.1. Responsavel

Representa o responsável pelo animal.

| Campo | Tipo | Descrição |
|---|---|---|
| `Id` | int | Identificador do responsável |
| `Name` | string | Nome do responsável |
| `Email` | string | E-mail do responsável |
| `Phone` | string | Telefone |
| `Cpf` | string | CPF |
| `CreatedAt` | DateTime | Data de cadastro |
| `IsActive` | bool | Indica se o registro está ativo |

### 6.2. Animal

Representa o animal acompanhado pela plataforma.

| Campo | Tipo | Descrição |
|---|---|---|
| `Id` | int | Identificador do animal |
| `ResponsavelId` | int | ID do responsável pelo animal |
| `Name` | string | Nome do animal |
| `Nickname` | string | Apelido do animal |
| `Species` | string | Espécie do animal |
| `Breed` | string | Raça |
| `BirthDate` | DateTime | Data de nascimento |
| `Weight` | decimal | Peso |
| `Sex` | string | Sexo |
| `Rga` | string | Registro Geral Animal |
| `CreatedAt` | DateTime | Data de cadastro |
| `IsActive` | bool | Indica se o registro está ativo |

Espécies permitidas nesta versão:

```text
DOG
CAT
```

### 6.3. CareEvent

Representa um evento de cuidado vinculado a um animal.

| Campo | Tipo | Descrição |
|---|---|---|
| `Id` | int | Identificador do evento |
| `PetId` | int | ID do animal relacionado |
| `Type` | string | Tipo do evento |
| `Title` | string | Título do evento |
| `Description` | string | Descrição |
| `ScheduledDate` | DateTime | Data prevista |
| `CompletedDate` | DateTime? | Data de conclusão |
| `Status` | string | Status do evento |
| `Priority` | string | Prioridade |
| `Notes` | string | Observações |
| `CreatedAt` | DateTime | Data de criação |
| `IsActive` | bool | Indica se o registro está ativo |

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

### 7.4. Configuração de Boolean no Oracle

Os campos booleanos foram mapeados como `NUMBER(1)`:

```text
true  → 1
false → 0
```

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

---

## 9. Como Executar o Projeto

### 9.1. Pré-requisitos

- .NET SDK instalado
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

```bash
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

---

## 11. Retornos HTTP Implementados

| Código | Uso |
|---|---|
| `200 OK` | Consulta realizada com sucesso |
| `201 Created` | Recurso criado com sucesso |
| `204 No Content` | Atualização, remoção ou conclusão realizada com sucesso |
| `400 Bad Request` | Dados inválidos ou regra de negócio violada |
| `404 Not Found` | Recurso não encontrado |

---

## 12. Fluxo de Teste Recomendado

1. Criar um responsável via `POST /api/responsaveis`;
2. Listar responsáveis via `GET /api/responsaveis`;
3. Criar um animal usando o `id` do responsável via `POST /api/animais`;
4. Listar animais do responsável via `GET /api/animais/responsavel/{responsavelId}`;
5. Criar um evento de cuidado usando o `id` do animal via `POST /api/care-events`;
6. Listar eventos do animal via `GET /api/care-events/animal/{animalId}`;
7. Filtrar eventos por status via `GET /api/care-events/status/PENDING`;
8. Concluir um evento via `PATCH /api/care-events/{id}/complete`;
9. Consultar novamente o evento concluído;
10. Validar os registros no Oracle.

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
- Nome, espécie, raça, sexo e peso válido são obrigatórios;
- O MVP aceita apenas `DOG` e `CAT`;
- Não permite RGA duplicado quando informado.

### Eventos de Cuidado
- Um evento só pode ser criado se o animal existir;
- Tipo, título, status, prioridade e data prevista são obrigatórios;
- O tipo, status e prioridade devem estar dentro das listas permitidas;
- Eventos cancelados não podem ser concluídos;
- Ao concluir um evento, o status é alterado para `COMPLETED` e a data de conclusão é preenchida.

---

## 15. Decisões Técnicas

### Controllers
A API utiliza Controllers para manter a organização das rotas e separação por recurso: `ResponsaveisController`, `AnimaisController` e `CareEventsController`.

### EF Core
O Entity Framework Core abstrai a comunicação com o banco Oracle, permitindo trabalhar com entidades C# e migrations versionadas.

### User Secrets
As credenciais do Oracle não são versionadas no GitHub. O projeto utiliza User Secrets para armazenar a connection string real durante o desenvolvimento local.

### Boolean no Oracle
Os campos booleanos são mapeados como `NUMBER(1)` com conversão automática via `HasConversion<int>()`.

---

## 16. Limitações Conhecidas

- O nome `DotNet.Api` é temporário;
- O MVP trabalha apenas com cães e gatos;
- Ainda não há autenticação;
- Ainda não há DTOs separados para entrada e saída;
- Ainda não há camada de Services/Repositories;
- As validações estão nos Controllers;
- Sem front-end integrado nesta entrega.

---

## 17. Comandos Úteis

```bash
# Build
dotnet build

# Run
dotnet run

# Criar migration
dotnet ef migrations add NomeDaMigration

# Aplicar migration
dotnet ef database update

# Listar migrations
dotnet ef migrations list

# Listar User Secrets
dotnet user-secrets list
```

---

## 18. Status Atual

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
| README técnico | ✅ Concluído |