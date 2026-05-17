# DotNet.Api — API REST para Jornada Contínua de Cuidado Pet

> **Nome temporário do projeto:** `DotNet.Api`  
> **Nome comercial:** a definir pelo grupo  
> **Contexto:** Challenge FIAP 2026 — solução digital para apoiar a continuidade do cuidado veterinário, com foco em tutor, pet e eventos de cuidado.

---

## 1. Visão Geral

A `DotNet.Api` é uma API RESTful desenvolvida em **ASP.NET Core Web API** para representar o núcleo back-end de uma solução de acompanhamento contínuo da jornada de saúde de animais de estimação.

O objetivo da API é permitir o cadastro e gerenciamento de:

- Tutores;
- Pets vinculados a tutores;
- Eventos de cuidado vinculados aos pets.

A proposta é estruturar dados essenciais para que uma aplicação web/app possa consultar, registrar e acompanhar informações relacionadas ao cuidado preventivo, terapêutico e recorrente de cada animal.

Nesta primeira versão, o MVP trabalha com **cães** e **gatos**, mantendo a arquitetura preparada para evolução futura para outras espécies.

---

## 2. Problema de Negócio

A jornada de saúde do pet costuma ser fragmentada. O tutor normalmente procura atendimento veterinário apenas em situações pontuais, como vacinação, sintomas, emergência ou retorno solicitado.

Essa fragmentação gera problemas como:

- Esquecimento de vacinas, retornos e check-ups;
- Baixa continuidade no tratamento;
- Falta de histórico centralizado do animal;
- Dificuldade para o tutor acompanhar cuidados recorrentes;
- Menor recorrência e fidelização para clínicas e profissionais veterinários.

A API proposta atua como base para uma plataforma que organiza a relação entre tutor, pet e eventos de cuidado.

---

## 3. Objetivo do MVP

O MVP da API tem como objetivo entregar um fluxo funcional mínimo:

1. Cadastrar um tutor;
2. Cadastrar um pet vinculado ao tutor;
3. Cadastrar eventos de cuidado vinculados ao pet;
4. Consultar a jornada de cuidado do pet;
5. Filtrar eventos por status, tipo e pet;
6. Persistir os dados em banco Oracle via Entity Framework Core.

Fluxo principal:

```text
Tutor → Pet → CareEvent
```

Exemplo:

```text
Tutor: Mariana Oliveira
Pet: Thor
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

A arquitetura atual segue uma estrutura simples e objetiva para atender ao escopo da Sprint.

```text
DotNet.Api/
├── Controllers/
│   ├── TutorsController.cs
│   ├── PetsController.cs
│   └── CareEventsController.cs
│
├── Data/
│   ├── AppDbContext.cs
│   └── InMemoryDatabase.cs
│
├── Models/
│   ├── Tutor.cs
│   ├── Pet.cs
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

> Observação: `InMemoryDatabase.cs` foi usado na fase inicial de prototipação, antes da integração com Oracle. A versão final dos controllers utiliza `AppDbContext`.

---

## 6. Modelo de Domínio

### 6.1. Tutor

Representa o responsável pelo pet.

| Campo | Tipo | Descrição |
|---|---|---|
| `Id` | int | Identificador do tutor |
| `Name` | string | Nome do tutor |
| `Email` | string | E-mail do tutor |
| `Phone` | string | Telefone |
| `Cpf` | string | CPF |
| `CreatedAt` | DateTime | Data de cadastro |
| `IsActive` | bool | Indica se o registro está ativo |

### 6.2. Pet

Representa o animal acompanhado pela plataforma.

| Campo | Tipo | Descrição |
|---|---|---|
| `Id` | int | Identificador do pet |
| `TutorId` | int | ID do tutor responsável |
| `Name` | string | Nome do pet |
| `Nickname` | string | Apelido do pet |
| `Species` | string | Espécie do animal |
| `Breed` | string | Raça |
| `BirthDate` | DateTime | Data de nascimento |
| `Weight` | decimal | Peso |
| `Sex` | string | Sexo |
| `Rga` | string | Registro Geral Animal |
| `CreatedAt` | DateTime | Data de cadastro |
| `IsActive` | bool | Indica se o registro está ativo |

Nesta versão, as espécies permitidas são:

```text
DOG
CAT
```

Essa limitação faz parte do escopo do MVP. A arquitetura permite evolução futura para outras espécies.

### 6.3. CareEvent

Representa um evento de cuidado vinculado a um pet.

| Campo | Tipo | Descrição |
|---|---|---|
| `Id` | int | Identificador do evento |
| `PetId` | int | ID do pet relacionado |
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
VACCINE
DEWORMING
MEDICATION
CHECKUP
RETURN
EXAM
GROOMING
SURGERY
OTHER
```

Status permitidos:

```text
PENDING
COMPLETED
OVERDUE
CANCELED
```

Prioridades permitidas:

```text
LOW
MEDIUM
HIGH
CRITICAL
```

---

## 7. Banco de Dados

A aplicação utiliza **Oracle Database** com **Entity Framework Core**.

### 7.1. Tabelas

| Entidade | Tabela Oracle |
|---|---|
| `Tutor` | `T_CP_TUTORS` |
| `Pet` | `T_CP_PETS` |
| `CareEvent` | `T_CP_CARE_EVENTS` |

Além dessas tabelas, o EF Core cria a tabela interna:

```text
__EFMigrationsHistory
```

Ela registra quais migrations já foram aplicadas no banco.

### 7.2. Relacionamentos lógicos

```text
T_CP_TUTORS 1 ─── N T_CP_PETS
T_CP_PETS   1 ─── N T_CP_CARE_EVENTS
```

Na versão atual, a validação dos relacionamentos é feita nos controllers antes de inserir registros. Exemplo:

- Um pet só pode ser criado se o `TutorId` existir;
- Um evento de cuidado só pode ser criado se o `PetId` existir.

### 7.3. Configuração de Boolean no Oracle

Como o Oracle utilizado no projeto não trabalha diretamente com `bool` da mesma forma que o C#, os campos booleanos foram mapeados como:

```text
NUMBER(1)
```

Conversão usada:

```text
true  → 1
false → 0
```

Isso evita erros de tipo inválido no Oracle.

---

## 8. Configuração de Credenciais

O arquivo `appsettings.json` deve conter apenas um placeholder, sem senha real:

```json
{
  "ConnectionStrings": {
    "OracleConnection": "User Id=RMXXXXXX;Password=SUA_SENHA;Data Source=oracle.fiap.com.br:1521/orcl;"
  },
  "AllowedHosts": "*"
}
```

A connection string real deve ser configurada localmente com **User Secrets**:

```bash
dotnet user-secrets init
```

```bash
dotnet user-secrets set "ConnectionStrings:OracleConnection" "User Id=SEU_RM;Password=SUA_SENHA_REAL;Data Source=oracle.fiap.com.br:1521/orcl;"
```

Para conferir:

```bash
dotnet user-secrets list
```

Essa abordagem evita expor credenciais no GitHub.

---

## 9. Como Executar o Projeto

### 9.1. Pré-requisitos

- .NET SDK instalado;
- Oracle Database acessível;
- Credenciais Oracle válidas;
- Git;
- Insomnia, Postman ou Swagger para testes.

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

A aplicação roda localmente em:

```text
http://localhost:5296
```

Swagger:

```text
http://localhost:5296/swagger
```

---

## 10. Endpoints da API

Base URL local:

```text
http://localhost:5296
```

### 10.1. Tutors

| Método | Rota | Descrição | Retornos |
|---|---|---|---|
| GET | `/api/tutors` | Lista todos os tutores | 200 |
| GET | `/api/tutors/{id}` | Busca tutor por ID | 200, 404 |
| GET | `/api/tutors/cpf/{cpf}` | Busca tutor por CPF | 200, 404 |
| POST | `/api/tutors` | Cria um tutor | 201, 400 |
| PUT | `/api/tutors/{id}` | Atualiza um tutor | 204, 400, 404 |
| DELETE | `/api/tutors/{id}` | Remove um tutor | 204, 404 |

#### Exemplo de POST `/api/tutors`

```json
{
  "name": "Mariana Oliveira",
  "email": "mariana.oliveira@email.com",
  "phone": "11977776666",
  "cpf": "45678912300"
}
```

### 10.2. Pets

| Método | Rota | Descrição | Retornos |
|---|---|---|---|
| GET | `/api/pets` | Lista todos os pets | 200 |
| GET | `/api/pets/{id}` | Busca pet por ID | 200, 404 |
| GET | `/api/pets/tutor/{tutorId}` | Lista pets de um tutor | 200, 404 |
| GET | `/api/pets/species/{species}` | Lista pets por espécie | 200 |
| GET | `/api/pets/breed/{breed}` | Lista pets por raça | 200 |
| GET | `/api/pets/rga/{rga}` | Busca pet por RGA | 200, 404 |
| POST | `/api/pets` | Cria um pet | 201, 400 |
| PUT | `/api/pets/{id}` | Atualiza um pet | 204, 400, 404 |
| DELETE | `/api/pets/{id}` | Remove um pet | 204, 404 |

#### Exemplo de POST `/api/pets`

```json
{
  "tutorId": 1,
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
| GET | `/api/care-events/pet/{petId}` | Lista eventos de um pet | 200, 404 |
| GET | `/api/care-events/status/{status}` | Lista eventos por status | 200, 400 |
| GET | `/api/care-events/type/{type}` | Lista eventos por tipo | 200, 400 |
| GET | `/api/care-events/pet/{petId}/status/{status}` | Lista eventos de um pet por status | 200, 400, 404 |
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

## 11. Fluxo de Teste Recomendado

Para validar o caminho principal da aplicação:

1. Criar um tutor;
2. Listar tutores;
3. Criar um pet usando o `id` do tutor;
4. Listar pets do tutor;
5. Criar um evento de cuidado usando o `id` do pet;
6. Listar eventos do pet;
7. Filtrar eventos por status;
8. Concluir um evento;
9. Consultar novamente o evento concluído;
10. Validar os registros no Oracle.

---

## 12. Consultas SQL para Validação

### Listar tutores

```sql
SELECT * FROM T_CP_TUTORS;
```

### Listar pets

```sql
SELECT * FROM T_CP_PETS;
```

### Listar eventos

```sql
SELECT * FROM T_CP_CARE_EVENTS;
```

### Validar fluxo completo com JOIN

```sql
SELECT
    t.ID AS TUTOR_ID,
    t.NAME AS TUTOR_NAME,
    p.ID AS PET_ID,
    p.NAME AS PET_NAME,
    p.SPECIES,
    p.BREED,
    ce.ID AS EVENT_ID,
    ce.TYPE,
    ce.TITLE,
    ce.STATUS,
    ce.PRIORITY,
    ce.SCHEDULED_DATE
FROM T_CP_TUTORS t
INNER JOIN T_CP_PETS p
    ON p.TUTOR_ID = t.ID
INNER JOIN T_CP_CARE_EVENTS ce
    ON ce.PET_ID = p.ID
ORDER BY t.ID, p.ID, ce.ID;
```

---

## 13. Retornos HTTP Implementados

A API implementa os principais retornos esperados para uma API REST:

| Código | Uso |
|---|---|
| `200 OK` | Consulta realizada com sucesso |
| `201 Created` | Recurso criado com sucesso |
| `204 No Content` | Atualização, remoção ou conclusão realizada com sucesso |
| `400 Bad Request` | Dados inválidos ou regra de negócio violada |
| `404 Not Found` | Recurso não encontrado |

---

## 14. Regras de Negócio Implementadas

### Tutores

- Nome, e-mail, telefone e CPF são obrigatórios;
- Não permite CPF duplicado.

### Pets

- Um pet só pode ser criado se o tutor existir;
- Nome, espécie, raça, sexo e peso válido são obrigatórios;
- O MVP aceita apenas `DOG` e `CAT`;
- Não permite RGA duplicado quando informado.

### Eventos de Cuidado

- Um evento só pode ser criado se o pet existir;
- Tipo, título, status, prioridade e data prevista são obrigatórios;
- O tipo deve estar dentro da lista permitida;
- O status deve estar dentro da lista permitida;
- A prioridade deve estar dentro da lista permitida;
- Eventos cancelados não podem ser concluídos;
- Ao concluir um evento, o status é alterado para `COMPLETED` e a data de conclusão é preenchida.

---

## 15. Decisões Técnicas

### Uso de Controllers

A API utiliza Controllers para manter a organização das rotas e facilitar a separação por recurso:

```text
TutorsController
PetsController
CareEventsController
```

### Uso de EF Core

O Entity Framework Core foi utilizado para abstrair a comunicação com o banco Oracle, permitindo trabalhar com entidades C# e migrations.

### Uso de User Secrets

As credenciais do Oracle não são versionadas no GitHub. O projeto utiliza User Secrets para armazenar a connection string real durante o desenvolvimento local.

### Uso de validações nos Controllers

As validações foram implementadas diretamente nos controllers para manter o escopo simples nesta Sprint. Em uma evolução futura, essas regras podem ser movidas para Services, Validators ou DTOs.

### Tratamento de compatibilidade com Oracle

Durante a integração, o projeto foi ajustado para lidar com diferenças entre C# e Oracle, especialmente no mapeamento de booleanos e em consultas que poderiam gerar literais incompatíveis com o banco.

---

## 16. Limitações Conhecidas

- O nome `DotNet.Api` é temporário;
- O MVP trabalha apenas com cães e gatos;
- Ainda não há autenticação;
- Ainda não há DTOs separados para entrada e saída;
- Ainda não há camada de Services/Repositories;
- As validações estão nos Controllers;
- A aplicação não possui front-end integrado nesta entrega;
- A aplicação não possui IA própria nesta etapa;
- A API está focada na base funcional e persistência Oracle.

---

## 17. Roadmap de Evolução

Possíveis evoluções:

1. Definir nome comercial final do produto;
2. Criar DTOs para requests e responses;
3. Criar camada de Services;
4. Criar camada de Repositories;
5. Adicionar autenticação;
6. Expandir espécies além de cães e gatos;
7. Adicionar recomendações personalizadas por raça, idade, peso e histórico;
8. Integrar com app/web;
9. Integrar com módulo de IA;
10. Implementar notificações e lembretes;
11. Criar dashboard para veterinário;
12. Criar dashboard de métricas e KPIs;
13. Containerizar a aplicação para Cloud;
14. Publicar em ambiente de nuvem.

---

## 18. Comandos Úteis

### Build

```bash
dotnet build
```

### Run

```bash
dotnet run
```

### Criar migration

```bash
dotnet ef migrations add InitialCreate
```

### Aplicar migration

```bash
dotnet ef database update
```

### Listar User Secrets

```bash
dotnet user-secrets list
```

---

## 19. Status Atual

| Item | Status |
|---|---|
| API ASP.NET Core | Concluído |
| Swagger | Concluído |
| CRUD de Tutor | Concluído |
| CRUD de Pet | Concluído |
| CRUD de CareEvent | Concluído |
| Rotas parametrizadas | Concluído |
| Retornos HTTP principais | Concluído |
| Oracle + EF Core | Concluído |
| Migrations | Concluído |
| Persistência real no banco | Concluído |
| README técnico | Concluído |
| Nome comercial final | Pendente |
| Front-end/app | Pendente |
| IA | Pendente |

---

## 20. Conclusão

A `DotNet.Api` entrega a base back-end do MVP para uma solução de cuidado contínuo pet.

A API permite cadastrar tutores, pets e eventos de cuidado, mantendo os dados persistidos em Oracle por meio do Entity Framework Core. Com isso, a aplicação já oferece uma base funcional para evolução futura em app/web, notificações, personalização por perfil animal, IA e dashboards de acompanhamento veterinário.
