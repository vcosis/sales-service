using Microsoft.EntityFrameworkCore;
using SalesService.Infrastructure.Persistence;
using SalesService.Application.Interfaces;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext with placeholder connection string
builder.Services.AddDbContext<SalesDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Host=localhost;Database=salesdb;Username=postgres;Password=postgres"));

// Register repository
builder.Services.AddScoped<ISaleRepository, SaleRepository>();

// Add MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(SalesService.Application.DTOs.SaleDto).Assembly));

// Configure AutoMapper
builder.Services.AddAutoMapper(typeof(SalesService.Application.DTOs.SaleDto).Assembly);

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run(); 