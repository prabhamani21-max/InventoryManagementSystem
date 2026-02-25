
using FluentValidation;
using FluentValidation.AspNetCore;
using InventoryManagementSystem.ExceptionHandler;
using InventoryManagementSystem.Hubs;
using InventoryManagementSystem.Mapper;
using InventoryManagementSystem.Middlewares;
using InventoryManagementSystem.Repository.Data;
using InventoryManagementSystem.Repository.Implementation;
using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Service.Implementation;
using InventoryManagementSystem.Service.Interface;
using InventoryManagementSystem.Services;
using InventoryManagementSystem.Validation;
using InventoryManagementSytem.Common.Enums;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;
using Npgsql;
using Npgsql.NameTranslation;
using System.Reflection;

// Enable legacy timestamp behavior for Npgsql to allow DateTime with Kind=Unspecified
// to work with PostgreSQL 'timestamp with time zone' columns
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{
    logger.Info("Starting Inventory Management System  Web API");
    var builder = WebApplication.CreateBuilder(args);

    // Clear default logging providers and use NLog
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    builder.Services.AddControllers()
        .AddFluentValidation(fv => fv.ImplicitlyValidateChildProperties = true);
    // Add HttpContext
    builder.Services.AddHttpContextAccessor();

    // DbContext
    NpgsqlConnection.GlobalTypeMapper.MapEnum<MakingChargeType>(
        "public.making_charge_type",
        new NpgsqlSnakeCaseNameTranslator()
    );
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


    // AutoMapper Profiles
    builder.Services.AddAutoMapper(config => config.AddMaps(typeof(AutoMapperProfile).Assembly));

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();

    //FluentValidation
    //   builder.Services.AddValidatorsFromAssemblyContaining<LoginDtoValidator>();
    builder.Services.AddValidatorsFromAssemblyContaining<StatusDtoValidator>();
    builder.Services.AddValidatorsFromAssemblyContaining<UserDtoValidator>();
    builder.Services.AddValidatorsFromAssemblyContaining<RoleDtoValidator>();
    builder.Services.AddValidatorsFromAssemblyContaining<UserKycDtoValidator>();
    builder.Services.AddValidatorsFromAssemblyContaining<WarehouseDtoValidator>();
    builder.Services.AddValidatorsFromAssemblyContaining<SupplierDtoValidator>();
    builder.Services.AddValidatorsFromAssemblyContaining<StoneDtoValidator>();
    builder.Services.AddValidatorsFromAssemblyContaining<SaleOrderItemDtoValidator>();
    builder.Services.AddValidatorsFromAssemblyContaining<SaleOrderDtoValidator>();
    builder.Services.AddValidatorsFromAssemblyContaining<PurchaseOrderDtoValidator>();
    builder.Services.AddValidatorsFromAssemblyContaining<PaymentDtoValidator>();
    builder.Services.AddValidatorsFromAssemblyContaining<PurityDtoValidator>();
    builder.Services.AddValidatorsFromAssemblyContaining<MetalDtoValidator>();
    builder.Services.AddValidatorsFromAssemblyContaining<JewelleryItemDtoValidator>();
    builder.Services.AddValidatorsFromAssemblyContaining<ItemStoneDtoValidator>();
    builder.Services.AddValidatorsFromAssemblyContaining<CategoryDtoValidator>();
    builder.Services.AddValidatorsFromAssemblyContaining<CreateSaleOrderItemDtoValidator>();

    // Repositories
    builder.Services.AddScoped<IStatusRepository, StatusRepository>();
    builder.Services.AddScoped<IRoleRepository, RoleRepository>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IUserKycRepository, UserKycRepository>();
    builder.Services.AddScoped<IWarehouseRepository, WarehouseRepository>();
    builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
    builder.Services.AddScoped<IStoneRepository, StoneRepository>();
    builder.Services.AddScoped<ISaleOrderItemRepository, SaleOrderItemRepository>();
    builder.Services.AddScoped<ISaleOrderRepository, SaleOrderRepository>();
    builder.Services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
    builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
    builder.Services.AddScoped<IPurityRepository, PurityRepository>();
    builder.Services.AddScoped<IMetalRepository, MetalRepository>();
    builder.Services.AddScoped<IJewelleryItemRepository, JewelleryItemRepository>();
    builder.Services.AddScoped<IItemStoneRepository, ItemStoneRepository>();
    builder.Services.AddScoped<IItemStockRepository, ItemStockRepository>();
    builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
    builder.Services.AddScoped<IMetalRateRepository, MetalRateRepository>();
    builder.Services.AddScoped<IStoneRateRepository, StoneRateRepository>();
    builder.Services.AddScoped<IExchangeRepository, ExchangeRepository>();
    builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
    builder.Services.AddScoped<ITcsRepository, TcsRepository>();
    // Services
    builder.Services.AddScoped<IStatusService, StatusService>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IRoleService, RoleService>();
    builder.Services.AddScoped<IUserKycService, UserKycService>();
    builder.Services.AddScoped<IWarehouseService, WarehouseService>();
    builder.Services.AddScoped<ISupplierService, SupplierService>();
    builder.Services.AddScoped<IStoneService, StoneService>();
    builder.Services.AddScoped<ISaleOrderItemService, SaleOrderItemService>();
    builder.Services.AddScoped<ISaleOrderService, SaleOrderService>();
    builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
    builder.Services.AddScoped<IPaymentService, PaymentService>();
    builder.Services.AddScoped<IPurityService, PurityService>();
    builder.Services.AddScoped<IMetalService, MetalService>();
    builder.Services.AddScoped<IJewelleryItemService, JewelleryItemService>();
    builder.Services.AddScoped<IItemStoneService, ItemStoneService>();
    builder.Services.AddScoped<IItemStockService, ItemStockService>();
    builder.Services.AddScoped<ICategoryService, CategoryService>();
    builder.Services.AddScoped<ICurrentUser, CurrentUser>();
    // Price Calculation Services
    builder.Services.AddScoped<IMetalRateService, MetalRateService>();
    builder.Services.AddScoped<IStoneRateService, StoneRateService>();
    builder.Services.AddScoped<IExchangeService, ExchangeService>();
    builder.Services.AddScoped<IInvoiceService, InvoiceService>();
    builder.Services.AddScoped<IEInvoiceService, EInvoiceService>();
    builder.Services.AddScoped<ITcsService, TcsService>();

    // Register HttpClient for EInvoiceService
    builder.Services.AddHttpClient<EInvoiceService>();

    // SignalR Services
    builder.Services.AddScoped<IUserNotificationService, UserNotificationService>();
    builder.Services.AddScoped<ISaleOrderNotificationService, SaleOrderNotificationService>();
    builder.Services.AddSignalR();


    // Logging
    builder.Services.AddLogging(logging =>
    {
        logging.AddConsole();
        logging.AddDebug();
    });
    // Global Exception Handling
    builder.Services.AddScoped<IExceptionHandler, GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    // Swagger
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter a valid token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
        });
    });
    // JWT Configuration
    builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddAuthorization();

// Register JwtService
builder.Services.AddScoped<InventoryManagementSystem.Service.Interface.IJwtService>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var key = config["Jwt:Key"];
    var issuer = config["Jwt:Issuer"];
    var audience = config["Jwt:Audience"];
    return new InventoryManagementSystem.Service.Implementation.JwtService(key, issuer, audience);
});
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAngularApp",
            builder => builder.WithOrigins("http://localhost:4200")
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials());
    });

    // Register Database Seeder
    builder.Services.AddScoped<InventoryManagementSystem.Repository.Data.DatabaseSeeder>();

    var app = builder.Build();

    // Apply migrations and seed data
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<InventoryManagementSystem.Repository.Data.AppDbContext>();
        var seeder = scope.ServiceProvider.GetRequiredService<InventoryManagementSystem.Repository.Data.DatabaseSeeder>();
        
        // Apply any pending migrations
        dbContext.Database.Migrate();
        
        // Seed initial data
        seeder.SeedAsync().GetAwaiter().GetResult();
    }

    // Middleware - Request & Response Logging

    app.UseMiddleware<RequestResponseLoggingMiddleware>();

    // Global Exception Middleware
    app.Use(async (context, next) =>
    {
        var exceptionHandler = context.RequestServices.GetRequiredService<IExceptionHandler>();
        try
        {
            await next();
        }
        catch (Exception ex)
        {
            var handled = await exceptionHandler.TryHandleAsync(context, ex, CancellationToken.None);
            if (!handled) throw;
        }
    });
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    if (!app.Environment.IsDevelopment())
    {
        app.UseHttpsRedirection();
    }
    app.UseCors("AllowAngularApp");
    app.UseAuthentication();
    app.UseMiddleware<CurrentUserMiddleware>();
    app.UseMiddleware<ApiResponseMiddleware>();
    app.UseAuthorization();

app.MapControllers();

// Map SignalR Hub endpoints
app.MapHub<UserHub>("/hubs/user");
app.MapHub<SaleOrderHub>("/hubs/saleorder");

app.Run();
}
catch (Exception ex)
{
    logger.Error(ex, "Application stopped due to an exception");
    throw;
}
finally
{
    LogManager.Shutdown();
}
