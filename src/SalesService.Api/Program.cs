using Microsoft.EntityFrameworkCore;
using SalesService.Infrastructure.Persistence;
using SalesService.Application.Interfaces;
using SalesService.Infrastructure.Events;
using SalesService.Api.Middleware;
using Rebus.Config;
using Rebus.Activation;
using Rebus.Transport.InMem;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext
builder.Services.AddDbContext<SalesDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Repositories
builder.Services.AddScoped<ISaleRepository, SaleRepository>();

// Add Event Publisher (using Rebus In-Memory implementation)
builder.Services.AddScoped<IEventPublisher, RebusEventPublisher>();

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly, typeof(SalesService.Application.Profiles.SaleProfile).Assembly);

// Add MediatR
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(SalesService.Application.Commands.CreateSaleCommand).Assembly);
});

// Configure Rebus with In-Memory transport
var rebusConfig = Configure.With(new BuiltinHandlerActivator())
    .Transport(t => t.UseInMemoryTransport(new InMemNetwork(), "sales-service"))
    .Logging(l => l.Console())
    .Start();

builder.Services.AddSingleton(rebusConfig);

var app = builder.Build();

// Run migrations automatically on startup
Console.WriteLine("🔄 Running database migrations...");

using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<SalesDbContext>();

try
{
    // Ensure database is created and migrations are applied
    context.Database.Migrate();
    Console.WriteLine("✅ Migrations completed successfully!");
    
    // Seed database with sample data
    await DbInitializer.SeedAsync(context);
    Console.WriteLine("✅ Database seeded successfully!");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error running migrations: {ex.Message}");
    Console.WriteLine("⚠️  Application will continue without migrations...");
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // In production, still enable Swagger but you might want to restrict access
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Add global exception handler middleware
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

// Only use HTTPS redirection if HTTPS is available and not in development
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

Console.WriteLine("🚀 Sales Service API started successfully!");
Console.WriteLine($"🌐 Swagger UI available at: http://localhost:5000/swagger");
app.Run(); 