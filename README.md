# Training Angular API

Web API em .NET 10 para autenticação (JWT) e CRUD de contas, com Clean Architecture em um único projeto e armazenamento em memória (EF Core InMemory).

## Requisitos

- .NET SDK 10.0.x

## Como executar

```sh
dotnet build
ASPNETCORE_URLS=http://localhost:5000 dotnet run
```

> [!NOTE]
> Não há `Properties/launchSettings.json`; a porta é fixada explicitamente via `ASPNETCORE_URLS`.

## Usuários em memória

O banco InMemory (nome `TrainingAngularApi`) é recriado a cada execução. Ambos os usuários têm role `Admin`.

| Usuário | Senha |
| --- | --- |
| `neuclair.junior` | `admin123` |
| `felipe.milhossi` | `admin123` |

## Endpoints

| Método | Rota | Auth | Observações |
| --- | --- | --- | --- |
| `POST` | `/v1/authenticate` | nenhuma | Aceita `application/json` e `application/x-www-form-urlencoded` (duas ações diferenciadas por `[Consumes]`). Retorna `{token, expireDate, username}` ou `400`. |
| `POST` | `/v1/accounts` | Bearer | Retorna `200` com a conta criada (inclui `id`). |
| `GET` | `/v1/accounts` | Bearer | Paginado via `?page=&pageSize=` (default `1`/`20`, máx. `100`, clamp em `PageResult<T>`). |
| `GET` | `/v1/accounts/{id}` | Bearer | Id inexistente → `200` com corpo `null`. |
| `PUT` | `/v1/accounts/{id}` | Bearer | Id inexistente → `200` no-op. |
| `DELETE` | `/v1/accounts/{id}` | Bearer | Id inexistente → `200` no-op. |

Todas as rotas, exceto `POST /v1/authenticate`, exigem Bearer token: `AuthorizationOptions.FallbackPolicy` = `RequireAuthenticatedUser` (o `[AllowAnonymous]` do controller de autenticação é a única exceção).

### Exemplo

```sh
# 1) Obter token
curl -s -X POST http://localhost:5000/v1/authenticate \
  -H "Content-Type: application/json" \
  -d '{"username": "neuclair.junior", "password": "admin123"}'

# 2) Usar o token em uma rota protegida
curl -s "http://localhost:5000/v1/accounts?page=1&pageSize=20" \
  -H "Authorization: Bearer <token>"
```

## Arquitetura

Projeto único (`training-angular-api.csproj`), sem `.sln`:

```text
training-angular-api/
├── Domain/           # Account, User, AccountType — zero dependências
├── Application/      # use cases (1 interface + 1 classe por caso de uso), DTOs (sealed records), PageResult<T>
├── Infrastructure/   # EF Core (ApplicationDbContext, repositórios, DatabaseSeeder) + JWT (JwtTokenGenerator, JwtOptions)
├── Controllers/      # AuthenticationController, AccountsController
└── Program.cs        # minimal hosting + wiring de DI
```

Regra de dependência: `Controllers → Application → Domain`; `Infrastructure` implementa as interfaces definidas em `Application`.

### JWT

- Configuração em `appsettings.json` (seção `Jwt`, bound em `JwtOptions`): issuer, audience, secret e validade.
- Tokens **criados** com `System.IdentityModel.Tokens.Jwt` `8.22.0` (pinned — não existe linha 10.x do pacote) e **validados** com `Microsoft.AspNetCore.Authentication.JwtBearer` `10.0.11`.
- Clock skew zero.

## Upgrade para .NET 10

O projeto foi migrado de **.NET Core 1.1** para **.NET 10.0** (commit `d226aed`) em duas fases — **planejamento** e **execução** — conduzidas no [OpenCode](https://opencode.ai) com modelos acessados via [OpenRouter](https://openrouter.ai), guiadas pela skill **dotnet-upgrade**.

### Skill utilizada

A skill **[dotnet-upgrade](https://github.com/github/awesome-copilot/blob/main/skills/dotnet-upgrade/SKILL.md)** do projeto [github/awesome-copilot](https://github.com/github/awesome-copilot) — prompts prontos para análise e execução de upgrades de framework .NET (classificação de projetos, ordenação de upgrade, compatibilidade de pacotes, validação de build, regressão e documentação). Também instalada localmente em `.opencode/skills/dotnet-upgrade/SKILL.md`.

### Fase 1 — Geração do plano de migração

- **Modelo:** Claude Opus 5 (OpenRouter), executado no OpenCode
- **Tokens consumidos:** 737K
- **Custo:** $2.53

**Prompt utilizado** (`docs/prompts/001.txt`):

```text
Papel: Você é um desenvolvedor em .NET senior, e quero que você não crie abstrações desnecessárias.

Tarefa:
- Realizar o upgrade da aplicação de .NET Core 1.0 para .NET 10.0.

Restrições:
- Todo código precisar ser desenvolvido em inglês, como identificadores, rotas, arquivos, pastas, comentários e etc.
- Deve seguir o padrão de Clean Architecture, com camadas definidas em pastas e um único projeto:
  - Domain/ (entidades, enum, sem dependência de nada).
  - Application/ (caso de uso, interface, DTOs e deve depender somente de Domain).
  - Controllers/ (camada de apresentação que deve depender apenas do Application).
  - SOLID é obrigatório seguir em todo o projeto, um caso de uso por consulta (SPR), o controller depende apenas das abstrações criadas (DIP), interfaces pequenas e específicas (ISP), sem regras de negócio no controller.
  - Controller com primary constructor, request e response com sealed record; nunca expor entidade de domínio fora do Application.
  - Paginação via um padrão único, crie em Application/Common um arquivo chamado PageResult. O padrão de paginação do PageSize é 20, com o máximo de 100 itens, e o Page default é 1.
  - Validação com DataAnnotations sem adicionar pacotes.
  - Todo método Async deve receber e passar CancellationToken.
  - Utilize o padrão Repository esconder os detalhes de como os dados são salvos ou buscados.
  - Mantenha a versão do EF Core compatível com .NET 10.
  - Todos os dados devem ser armazenados em memória.

A aplicação estará definida como pronto somente quando `dotnet build! passar e o POST /v1/authenticate, POST v1/accounts, GET v1/accounts, PUT v1/accounts/{id}, GET v1/accounts/{id} e DELETE v1/accounts{id} responderem com 200 OK.
```

**Plano gerado** (`docs/plans/UPGRADE_PLAN.md`):

````markdown
# Upgrade Plan: .NET Core 1.1 -> .NET 10 + Clean Architecture

Status: **approved, not yet implemented**.
Scope: single project, in-memory storage, no `.sln`.

## Acceptance criteria (from `prompt.txt`)

`dotnet build` passes AND all of these return **200 OK**:

- `POST /v1/authenticate`
- `POST /v1/accounts`
- `GET /v1/accounts`
- `GET /v1/accounts/{id}`
- `PUT /v1/accounts/{id}`
- `DELETE /v1/accounts/{id}`

## Verified environment facts

- Installed SDK: **10.0.111**. Installed runtimes: `Microsoft.NETCore.App` and `Microsoft.AspNetCore.App` both **10.0.11**.
- `dotnet build` currently fails at **restore**, before compiling:
  `error NU1202: Package Microsoft.AspNetCore.Authentication.JwtBearer 2.1.30 is not compatible with netcoreapp1.1`.
  Cause: Dependabot merge `8ee1964` bumped JwtBearer 1.1.2 -> 2.1.30 while the csproj still targets `netcoreapp1.1`.
- `Microsoft.AspNetCore.Authentication.JwtBearer` 10.0.12 depends **only** on `Microsoft.IdentityModel.Protocols.OpenIdConnect` 8.0.1.
- `System.IdentityModel.Tokens.Jwt` has **no 10.x line**; latest is **8.22.0**. It reaches the project only *transitively* (8.0.1, via OpenIdConnect).
- No `.sln`, no tests, no CI, no `.editorconfig`, no `global.json`, no `Properties/launchSettings.json`.

## Decisions locked with the user

| Question | Decision |
|---|---|
| Rename scope | **Full rename** — `training-angular-api.csproj`, `RootNamespace`/`AssemblyName` = `Training.Angular.Api`, `.vscode` paths updated |
| `/v1/authenticate` contract | **Accept both** JSON and form-urlencoded |
| Missing id on `/v1/accounts/{id}` | **Always 200** — missing id is a no-op, never a throw |
| `bin/`+`obj/` git hygiene | **Leave git alone** — no `.gitignore`, no untracking |

---

## 1. Project file: retarget + rename

Rename `treinamento-angular-api.csproj` -> `training-angular-api.csproj`.

```xml
<TargetFramework>net10.0</TargetFramework>
<Nullable>enable</Nullable>
<ImplicitUsings>enable</ImplicitUsings>
<RootNamespace>Training.Angular.Api</RootNamespace>
<AssemblyName>Training.Angular.Api</AssemblyName>
```

Packages pinned to **10.0.12** to match the installed shared framework exactly:

| Action | Package                                                                                                                                                     |
|---|-------------------------------------------------------------------------------------------------------------------------------------------------------------|
| remove | `Microsoft.AspNetCore` 1.1.2, `Microsoft.AspNetCore.Mvc` 1.1.3, `Microsoft.Extensions.Logging.Debug` 1.1.2 — now provided by the Web SDK / shared framework |
| retarget | `Microsoft.EntityFrameworkCore` -> 10.0.12                                                                                                                  |
| retarget | `Microsoft.EntityFrameworkCore.InMemory` -> 10.0.12                                                                                                         |
| retarget | `Microsoft.AspNetCore.Authentication.JwtBearer` 2.1.30 -> 10.0.11 (fixes `NU1202`)                                                                          |
| **add** | `System.IdentityModel.Tokens.Jwt` **8.22.0**                                                                                                                |

Why the added package: JwtBearer 10.0.11 only *validates* tokens. `JwtSecurityTokenHandler` (token **creation**) lives in `System.IdentityModel.Tokens.Jwt`, which would otherwise be a transitive-only dependency — the exact trap `Newtonsoft.Json` fell into in the legacy code. `Microsoft.EntityFrameworkCore` stays explicit for the same reason. The "no new packages" constraint is scoped to validation: DataAnnotations only, nothing added there.

Also drop `<Folder Include="wwwroot\" />` (Windows separator, directory does not exist).

## 2. `Domain/` — zero dependencies

- `Domain/Entities/Account.cs`
- `Domain/Entities/User.cs`
- `Domain/Enums/AccountType.cs` — `EAccountType` -> `AccountType`, dropping the Hungarian `E` prefix

## 3. `Application/` — depends only on Domain

- `Application/Common/PageResult.cs` — the single pagination standard: `DefaultPage = 1`, `DefaultPageSize = 20`, `MaxPageSize = 100`, plus the clamping helper (`page < 1 -> 1`, `pageSize < 1 -> 20`, `pageSize > 100 -> 100`). Clamping lives here so it cannot drift.
- `Application/Common/GeneratedToken.cs` — token + expiry, so `ITokenGenerator` needs no JWT types.
- `Application/Dtos/` — `sealed record` requests/responses with DataAnnotations. Domain entities never leave this layer; use cases map to `AccountResponse`.
- `Application/Interfaces/` — `IAccountRepository`, `IUserRepository`, `ITokenGenerator`, plus **one interface per use case**: `IAuthenticateUserUseCase`, `ICreateAccountUseCase`, `IGetAccountsUseCase`, `IGetAccountByIdUseCase`, `IUpdateAccountUseCase`, `IDeleteAccountUseCase`. Each exposes a single `ExecuteAsync` — SRP/ISP/DIP without a mediator, mapper, or `Result<T>` wrapper.
- `Application/Services/` — the six use cases, one file each, primary constructors, every method `async` taking and forwarding `CancellationToken`.

Password comparison sits in `AuthenticateUserUseCase` (`IUserRepository` only fetches by username) so the rule stays out of the repository and out of the controller.

## 4. `Infrastructure/` — uses the folders already scaffolded

- `Infrastructure/Persistence/ApplicationDbContext.cs` (renamed from `DataContext`)
- `Infrastructure/Persistence/AccountRepository.cs`
- `Infrastructure/Persistence/UserRepository.cs`
- `Infrastructure/Persistence/DatabaseSeeder.cs`
- `Infrastructure/Auth/JwtTokenGenerator.cs`
- `Infrastructure/Auth/JwtOptions.cs`

**Interpretation to confirm:** the constraints list only `Domain/`, `Application/`, `Controllers/`, but `Infrastructure/{Auth,Persistence}` already exists as empty scaffolding in the repo. EF Core and JWT details cannot go in `Application/` without breaking "Application depends only on Domain", so those existing folders are used.

## 5. `Controllers/` — depends only on Application

- `AuthenticationController.cs` — `[AllowAnonymous]`, primary constructor. Two actions on `POST v1/authenticate` disambiguated by `[Consumes("application/json")]` vs `[Consumes("application/x-www-form-urlencoded")]`. `ConsumesAttribute` is an action constraint, so no `AmbiguousMatchException`. Both delegate to the same use case — zero duplicated logic. `curl -d` sends form-encoded by default, so the existing curl keeps working.
- `AccountsController.cs` — `ControllerBase` + `[ApiController]`, primary constructor taking the five account use cases. Routes preserved verbatim (`v1/accounts`, `v1/accounts/{id}`).
- Delete `Controllers/UserController.cs`.

`POST /v1/accounts` returns `Ok(...)`, not `201 Created`, per the acceptance criteria.

## 6. Root wiring

`Program.cs` becomes minimal hosting. `Startup.cs`, `Data/`, `Models/`, `Security/` are deleted. Replacements for the removed 1.x APIs:

| Legacy | Replacement |
|---|---|
| `WebHostBuilder` + `UseStartup<Startup>` | `WebApplication.CreateBuilder` |
| `opt.UseInMemoryDatabase()` | `UseInMemoryDatabase("TrainingAngularApi")` — **name is now mandatory** |
| `app.UseJwtBearerAuthentication(...)` + `AutomaticAuthenticate`/`AutomaticChallenge` | `AddAuthentication(...).AddJwtBearer(...)` + `UseAuthentication()` / `UseAuthorization()` |
| global `AuthorizeFilter` | `AddAuthorization(o => o.FallbackPolicy = ...RequireAuthenticatedUser())` — **preserves auth on every endpoint**, `[AllowAnonymous]` still wins |
| `loggerFactory.AddConsole/AddDebug`, `IHostingEnvironment`, manual `ConfigurationBuilder` | removed; host defaults cover them |
| seeding inside `Startup.Configure` | scoped seeder after `app.Build()` |

`appsettings.json` gains a `Jwt` section (issuer/audience/secret moved out of source consts). The existing 44-char secret is 352 bits, satisfying HmacSha256's 256-bit floor.

`.vscode/launch.json` program path -> `bin/Debug/net10.0/Training.Angular.Api.dll`; `tasks.json` -> schema 2.0.0.

## 7. Bugs fixed in passing

- `UserController.cs:52` reads `identity.FindFirst("Store")` but the claim is created as `"Angular"` (line 102), so a `null` Claim lands in the array and token creation throws. **Auth is currently impossible**; switching to `ClaimTypes.Role` fixes it.
- `PUT`/`DELETE` on an unknown id threw `NullReferenceException` -> now a 200 no-op.
- `Newtonsoft.Json` double-encoded JSON string (`JsonConvert.SerializeObject` into `OkObjectResult`) -> return the record, let `System.Text.Json` serialize.
- Portuguese error strings translated to English.
- Unused `"Admin"` policy dropped (registered in `Startup.cs:52`, never applied anywhere).

## 8. Verification

1. `dotnet build` — expect 0 errors.
2. `ASPNETCORE_URLS=http://localhost:5000 dotnet run` — port set explicitly because no `launchSettings.json` exists.
3. Curl checks:
   - `POST /v1/authenticate` as JSON -> 200
   - `POST /v1/authenticate` as form-urlencoded -> 200
   - `GET /v1/accounts` **without** a token -> **401** (proves global auth survived)
   - `POST /v1/accounts` with `Authorization: Bearer <token>` -> 200, capture `Id`
   - `GET /v1/accounts?page=1&pageSize=20` -> 200
   - `GET /v1/accounts/{id}` -> 200
   - `PUT /v1/accounts/{id}` -> 200
   - `DELETE /v1/accounts/{id}` -> 200
   - `GET /v1/accounts?pageSize=1000` -> clamped to 100

Seeded in-memory users (both `Role = "Admin"`, password `admin123`): `neuclair.junior`, `felipe.milhossi`. Data resets every run.

## Execution order

1. csproj retarget + rename + packages
2. `Domain/`
3. `Application/` (Common, Dtos, Interfaces, Services)
4. `Infrastructure/` (Persistence, Auth)
5. `Controllers/`
6. `Program.cs` + `appsettings.json` + delete legacy files
7. `.vscode/` config
8. Build + manual endpoint verification
9. Rewrite `AGENTS.md`

## Open caveats

1. **Stale `obj/`.** Renaming the csproj orphans `obj/treinamento-angular-api.csproj.nuget.g.*` and the tracked `netcoreapp1.1` artifacts, which can poison restore. Deleting `bin/` and `obj/` before the first build is a build action, not a git action — but since those 13 files are tracked it **will** show as deletions in `git status`. Nothing will be committed.
2. **`AGENTS.md` goes stale.** It documents the broken build, the empty scaffolding and the legacy layout — nearly all of which this change invalidates. Rewrite is step 9.
````

### Fase 2 — Execução do plano

- **Modelo:** Qwen3.8 27B (OpenRouter), executado no OpenCode
- **Tokens consumidos:** 2.32M
- **Custo:** $0.375

**Prompt utilizado** (`docs/prompts/002.txt`):

```text
Papel: Você é um desenvolvedor em .NET senior, e quero que você não crie abstrações desnecessárias.

Tarefa:
- Execute o plano de migração docs/plans/@UPGRADE_PLAN.md

A aplicação estará definida como pronto somente quando dotnet build passar e todas as rotas a seguir responderem 200 OK.
- POST /v1/authenticate
- POST /v1/accounts
- GET /v1/accounts
- GET /v1/accounts/{id}
- PUT /v1/accounts/{id}
- DELETE /v1/accounts/{id}
```

**Resultado:** `dotnet build` com 0 erros e todas as rotas da tabela acima respondendo `200 OK`.

## Observações

> [!WARNING]
> Os artefatos `bin/` e `obj/` do antigo projeto `netcoreapp1.1` continuam versionados no git; a remoção deles aparece no `git status`. É intencional — não re-adicionar.

- `System.IdentityModel.Tokens.Jwt` não tem linha 10.x: pinned em `8.22.0` e referenciado explicitamente (criação de tokens); o `JwtBearer` `10.0.11` apenas valida.
