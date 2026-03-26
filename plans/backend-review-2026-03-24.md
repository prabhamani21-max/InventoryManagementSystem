# Backend Review - 2026-03-24

## Scope

Reviewed the backend under:

- `InventoryManagementSystem/InventoryManagementSystem`
- `InventoryManagementSystem/InventoryManagementSystem.Service`
- `InventoryManagementSystem/InventoryManagementSystem.Repository`
- `InventoryManagementSystem/InventoryManagementSytem.Common`

I focused on security, hardcoded values, layering, data integrity, and general .NET API best practices. Generated migration files were not reviewed in detail except where they helped confirm behavior.

## Highest-impact findings

### 1. Secrets and environment-specific values are committed in source

Hardcoded values found:

- `InventoryManagementSystem/InventoryManagementSystem/appsettings.json:3`
  Database connection string includes host, username, and password.
- `InventoryManagementSystem/InventoryManagementSystem/appsettings.json:13`
  JWT signing key is stored directly in source.
- `InventoryManagementSystem/InventoryManagementSystem/appsettings.json:17`
  Company identity and legal metadata are hardcoded in app config.
- `InventoryManagementSystem/InventoryManagementSystem/Program.cs:219`
  CORS is hardcoded to `http://localhost:4200`.

Why this is a problem:

- Secrets in source control are a security incident waiting to happen.
- Dev-only values are mixed with deployable config.
- Rotating secrets or deploying multiple environments becomes harder.

Recommendation:

- Move secrets to environment variables, user-secrets, or a secret manager.
- Bind CORS, JWT, and company metadata through strongly typed `IOptions<T>`.

### 2. Password handling is not production-safe

Findings:

- `InventoryManagementSystem/InventoryManagementSystem/Helpers/PasswordHasher.cs:8`
  Passwords are hashed with plain SHA256 and no salt/work factor.
- `InventoryManagementSystem/InventoryManagementSystem/Controllers/UserController.cs:50`
  Login returns different responses for "user not found" vs "invalid password", which leaks account existence.
- `InventoryManagementSystem/InventoryManagementSystem/Middleware/RequestResponseLoggingMiddleware.cs:34`
  Full request bodies are logged. This will capture login credentials and potentially tokens or PII.

Why this is a problem:

- Plain SHA256 is not acceptable for password storage in a modern .NET app.
- Logging raw request bodies is a serious security and compliance risk.

Recommendation:

- Replace with `PasswordHasher<TUser>` from ASP.NET Core Identity or BCrypt/Argon2.
- Return one generic login failure response.
- Redact or skip sensitive routes and fields in request/response logging.

### 3. Seeder contains hardcoded master data and a hardcoded super-admin account, and it is not executed

Findings:

- `InventoryManagementSystem/InventoryManagementSystem.Repository/Data/DatabaseSeeder.cs:32`
  Raw SQL disables triggers.
- `InventoryManagementSystem/InventoryManagementSystem.Repository/Data/DatabaseSeeder.cs:41`
  Status IDs and names are hardcoded.
- `InventoryManagementSystem/InventoryManagementSystem.Repository/Data/DatabaseSeeder.cs:51`
  Role IDs and names are hardcoded.
- `InventoryManagementSystem/InventoryManagementSystem.Repository/Data/DatabaseSeeder.cs:61`
  Seed user name, email, phone, DOB, address, and password hash are hardcoded.
- `InventoryManagementSystem/InventoryManagementSystem/Program.cs:226`
  Seeder is registered, but I did not find any invocation of `SeedAsync()`.

Why this is a problem:

- Manual IDs, trigger disabling, and raw SQL seeding are brittle.
- Hardcoded admin credentials and personal data should not live in source.
- Registering a seeder without executing it creates false confidence.

Recommendation:

- Replace raw SQL trigger manipulation with EF-based seeding or a controlled migration.
- Move bootstrap admin creation to a one-time setup flow or protected operational script.
- If seeding is required, call it explicitly during startup with environment guards.

### 4. Some repository create methods return the input object instead of the persisted entity

Findings:

- `InventoryManagementSystem/InventoryManagementSystem.Repository/Implementation/UserRepository.cs:45`
  Saves `entity` but returns `user`.
- `InventoryManagementSystem/InventoryManagementSystem.Repository/Implementation/RoleRepository.cs:55`
  Saves `newRole` but returns `role`.
- `InventoryManagementSystem/InventoryManagementSystem.Repository/Implementation/StatusRepository.cs:54`
  Saves `entity` but returns `status`.

Why this is a problem:

- Callers may not receive generated IDs or database-populated fields.
- This can break follow-up logic and logging. `UserService` logs `registeredUser.Id`, but this can stay unset.

Recommendation:

- Return `_mapper.Map<T>(entity)` after `SaveChangesAsync()`.

### 5. Authorization is coarse and inconsistent

Findings:

- I did not find any role-based or policy-based authorization usage in controllers or startup.
- `InventoryManagementSystem/InventoryManagementSystem.Service/Implementation/JwtService.cs:31`
  Token carries a custom `RoleId` claim instead of a standard role claim.
- `InventoryManagementSystem/InventoryManagementSystem/Controllers/RoleController.cs:33`
  `GetAllRoles` is publicly accessible because the controller has no class-level `[Authorize]`.

Why this is a problem:

- Authenticated users appear to have broad access by default.
- The API is not using standard ASP.NET Core authorization primitives effectively.

Recommendation:

- Add role/policy authorization for sensitive endpoints.
- Emit standard role claims and define policies in startup.
- Review all controllers for least-privilege access.

### 6. Controllers are doing service-layer work

Examples:

- `InventoryManagementSystem/InventoryManagementSystem/Controllers/SaleOrderController.cs:107`
  Controller generates order number and sets audit/status fields.
- `InventoryManagementSystem/InventoryManagementSystem/Controllers/ExchangeController.cs:76`
  Controller sets audit/status fields.
- `InventoryManagementSystem/InventoryManagementSystem/Controllers/PaymentController.cs:98`
  Controller sets audit/status fields.
- `InventoryManagementSystem/InventoryManagementSystem/Controllers/UserController.cs:99`
  Controller sets audit/status fields and hashes the password.

Why this is a problem:

- Business rules are duplicated across entry points.
- Audit handling is inconsistent and harder to test.

Recommendation:

- Keep controllers thin.
- Move creation defaults, audit stamping, numbering, and state transitions into services or domain/application handlers.

### 7. Magic numbers and hardcoded business states are scattered across the codebase

Examples:

- `InventoryManagementSystem/InventoryManagementSystem.Service/Implementation/CategoryService.cs:95`
  `CreatedBy = 1`
- `InventoryManagementSystem/InventoryManagementSystem.Service/Implementation/CategoryService.cs:163`
  `StatusId = 1`
- `InventoryManagementSystem/InventoryManagementSystem.Service/Implementation/CategoryService.cs:182`
  `StatusId = 2`
- `InventoryManagementSystem/InventoryManagementSystem.Repository/Implementation/CategoryRepository.cs:139`
  `StatusId = 3`
- `InventoryManagementSystem/InventoryManagementSystem.Repository/Implementation/InvoiceRepository.cs:19`
  `CancelledStatusId = 4`
- `InventoryManagementSystem/InventoryManagementSystem.Repository/Implementation/InvoiceRepository.cs:283`
  `UpdatedBy = 1`
- `InventoryManagementSystem/InventoryManagementSystem.Service/Implementation/SaleOrderItemService.cs:196`
  `MakingChargesGstPercentage = 5m`
- `InventoryManagementSystem/InventoryManagementSystem.Service/Implementation/ExchangeService.cs:278`
  `exchangeType != 1`

Why this is a problem:

- The meaning of `1`, `2`, `3`, and `4` depends on developer memory.
- A status change in the database will silently break business logic.

Recommendation:

- Replace magic numbers with enums/constants/value objects.
- Centralize business rule values in one place.

### 8. Transactional integrity is incomplete in some workflows

Finding:

- `InventoryManagementSystem/InventoryManagementSystem.Service/Implementation/SaleOrderItemService.cs:207`
  Sale order item is created first.
- `InventoryManagementSystem/InventoryManagementSystem.Service/Implementation/SaleOrderItemService.cs:211`
  Stock is reserved afterward.
- `InventoryManagementSystem/InventoryManagementSystem.Service/Implementation/SaleOrderItemService.cs:214`
  If reservation fails, the item still remains created.

Why this is a problem:

- Order item and stock reservation can drift out of sync.

Recommendation:

- Wrap item creation and stock reservation in a single transaction or use a compensating rollback path.

### 9. Middleware contains unrelated hardcoded routes and overlapping response/error handling

Findings:

- `InventoryManagementSystem/InventoryManagementSystem/Middleware/ApiResponseMiddleware.cs:30`
  Debug log "MIDDLEWARE CHECK" runs on every request.
- `InventoryManagementSystem/InventoryManagementSystem/Middleware/ApiResponseMiddleware.cs:33`
  Hardcoded bypasses for unrelated routes:
  `/api/ACSChat/eventgrid-webhook`, `/chatHub`, `/api/ResumeParse/parse`
- `InventoryManagementSystem/InventoryManagementSystem/Program.cs:237`
  Custom exception wrapper middleware is added.
- `InventoryManagementSystem/InventoryManagementSystem/Middleware/ApiResponseMiddleware.cs:95`
  Response middleware also catches exceptions.

Why this is a problem:

- There is cross-project residue in production middleware.
- Error handling is split across multiple layers and can become unpredictabl

Recommendation:

- Remove unrelated bypass routes.
- Use one clear exception-handling strategy.
- Avoid wrapping all responses globally unless the contract is strictly enforced everywhere.

### 10. Query patterns can hurt database performance and correctness

Findings:

- `InventoryManagementSystem/InventoryManagementSystem.Repository/Implementation/CategoryRepository.cs:55`
  Uses `Name.ToLower() == name.ToLower()`
- `InventoryManagementSystem/InventoryManagementSystem.Repository/Implementation/CategoryRepository.cs:162`
  Same pattern in duplicate check
- `InventoryManagementSystem/InventoryManagementSystem.Repository/Implementation/StoneRepository.cs:74`
  Same pattern for exact match
- `InventoryManagementSystem/InventoryManagementSystem.Repository/Implementation/StoneRepository.cs:85`
  Same pattern for search

Why this is a problem:

- Lowercasing DB columns can prevent index usage and introduces culture-sensitive behavior.

Recommendation:

- Normalize values before persistence, or use case-insensitive indexes/collations.
- In PostgreSQL, prefer `EF.Functions.ILike` for case-insensitive search scenarios.

### 11. There are visible copy-paste and maintenance artifacts in production code

Examples:

- `InventoryManagementSystem/InventoryManagementSystem/Controllers/RoleController.cs:8`
  Namespace is `SalexiHRSystem.Controllers`.
- `InventoryManagementSystem/InventoryManagementSystem/Controllers/RoleController.cs:21`
  Unused `IStatusService` is injected.
- `InventoryManagementSystem/InventoryManagementSystem/Controllers/RoleController.cs:72`
  Error messages still say "lead status".
- `InventoryManagementSystem/InventoryManagementSystem/Controllers/StatusController.cs:8`
  Imports `SalexiHRSystem.Controllers`.
- `InventoryManagementSystem/InventoryManagementSystem/Controllers/StatusController.cs:19`
  Uses `ILogger<RoleController>` instead of `ILogger<StatusController>`.
- `InventoryManagementSystem/InventoryManagementSystem.Repository/Implementation/RoleRepository.cs:25`
  `r.StatusId == r.StatusId` is a no-op filter and suggests unfinished code.

Why this is a problem:

- It reduces trust in the codebase and makes future changes riskier.

Recommendation:

- Clean up stale references and unused dependencies.
- Review all legacy/copy-paste text in public APIs and logs.

### 12. Additional best-practice gaps

- `InventoryManagementSystem/InventoryManagementSystem.Service/Implementation/InvoicePdfService.cs:71`
  `Task.Run` is used inside ASP.NET request handling for PDF generation.
- `InventoryManagementSystem/InventoryManagementSystem/Controllers/InvoiceController.cs:148`
  `ex.Message.Contains("not found")` is used to decide whether to return 404.
- `InventoryManagementSystem/InventoryManagementSystem/Controllers/PaymentController.cs:62`
  ModelState validation is commented out in create.
- `InventoryManagementSystem/InventoryManagementSystem.Service/Implementation/TcsService.cs:33`
  TCS threshold and rates are hardcoded inside the service instead of being centralized/configurable.
- `InventoryManagementSystem/InventoryManagementSystem.Service/Implementation/TcsService.cs:405`
  `DateTime.Now` is used while most of the codebase uses `DateTime.UtcNow`.
- I did not find any backend test project or test files under `InventoryManagementSystem`.

## Hardcoded values inventory

This is the consolidated list of backend hardcoding I found that should be revisited first:

- Connection string, DB password, JWT key, company profile, invoice terms:
  `InventoryManagementSystem/InventoryManagementSystem/appsettings.json`
- CORS origin:
  `InventoryManagementSystem/InventoryManagementSystem/Program.cs:219`
- Seeder IDs, role names, status names, admin identity, password hash:
  `InventoryManagementSystem/InventoryManagementSystem.Repository/Data/DatabaseSeeder.cs`
- Magic status IDs and audit user IDs:
  `CategoryService.cs`, `CategoryRepository.cs`, `InvoiceRepository.cs`, `InvoiceGeneratorService.cs`, `SaleOrderItemService.cs`, `StoneRateService.cs`
- Order number string formats:
  `InventoryManagementSystem/InventoryManagementSystem/Controllers/SaleOrderController.cs:109`
  `InventoryManagementSystem/InventoryManagementSystem.Service/Implementation/ExchangeService.cs:56`
- Exchange phase hardcoding:
  `InventoryManagementSystem/InventoryManagementSystem.Service/Implementation/ExchangeService.cs:278`
- TCS threshold/rates and reporting literals such as `PANNA` and `Jewellery`:
  `InventoryManagementSystem/InventoryManagementSystem.Service/Implementation/TcsService.cs:33`
  `InventoryManagementSystem/InventoryManagementSystem.Service/Implementation/TcsService.cs:271`
  `InventoryManagementSystem/InventoryManagementSystem.Service/Implementation/TcsService.cs:280`
- API middleware bypass routes from another codebase:
  `InventoryManagementSystem/InventoryManagementSystem/Middleware/ApiResponseMiddleware.cs:33`

## Recommended remediation order

1. Remove secrets from source control and rotate compromised values.
2. Replace SHA256 password hashing and stop logging sensitive request bodies.
3. Fix repository create methods so they return persisted entities.
4. Centralize status IDs, audit defaults, and business constants.
5. Move business rules and audit stamping out of controllers.
6. Clean up authorization with roles/policies.
7. Make workflows transactional where stock/order/invoice state must stay consistent.
8. Add automated tests for auth, order creation, invoicing, exchange, and payment validation.

