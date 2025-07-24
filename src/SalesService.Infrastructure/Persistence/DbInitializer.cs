using SalesService.Domain.Entities;

namespace SalesService.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task SeedAsync(SalesDbContext context)
    {
        if (context.Sales.Any())
            return; // Database already seeded

        var sales = new List<Sale>
        {
            new Sale("SALE-001", DateTime.UtcNow.AddDays(-4), 1, "João Silva", 1, "Loja Centro", new List<SaleItem>()),
            new Sale("SALE-002", DateTime.UtcNow.AddDays(-4), 2, "Maria Santos", 1, "Loja Centro", new List<SaleItem>()),
            new Sale("SALE-003", DateTime.UtcNow.AddDays(-3), 3, "Pedro Oliveira", 2, "Loja Norte", new List<SaleItem>()),
            new Sale("SALE-004", DateTime.UtcNow.AddDays(-3), 4, "Ana Costa", 2, "Loja Norte", new List<SaleItem>()),
            new Sale("SALE-005", DateTime.UtcNow.AddDays(-2), 5, "Carlos Ferreira", 3, "Loja Sul", new List<SaleItem>()),
            new Sale("SALE-006", DateTime.UtcNow.AddDays(-2), 1, "João Silva", 3, "Loja Sul", new List<SaleItem>()),
            new Sale("SALE-007", DateTime.UtcNow.AddDays(-1), 6, "Lucia Martins", 1, "Loja Centro", new List<SaleItem>()),
            new Sale("SALE-008", DateTime.UtcNow.AddDays(-1), 7, "Roberto Lima", 2, "Loja Norte", new List<SaleItem>()),
            new Sale("SALE-009", DateTime.UtcNow, 8, "Fernanda Rocha", 3, "Loja Sul", new List<SaleItem>()),
            new Sale("SALE-010", DateTime.UtcNow, 9, "Marcos Alves", 1, "Loja Centro", new List<SaleItem>())
        };

        context.Sales.AddRange(sales);
        await context.SaveChangesAsync();

        // Add items to sales using the AddItem method (only to non-cancelled sales)
        sales[0].AddItem(1, "Notebook Dell Inspiron", 1, 1500.00m);        
        sales[1].AddItem(2, "Mouse Wireless Logitech", 2, 150.00m);
        sales[1].AddItem(3, "Teclado Mecânico RGB", 1, 450.00m);
        sales[1].AddItem(4, "Monitor 24\" Samsung", 1, 1580.50m);        
        sales[2].AddItem(5, "Headphone Bluetooth", 1, 890.75m);        
        sales[3].AddItem(6, "Smartphone iPhone 15", 1, 3200.00m);        
        sales[4].AddItem(7, "Tablet Samsung Galaxy", 1, 1750.25m);        
        // sales[5] will be cancelled, so no items added
        sales[6].AddItem(9, "Câmera DSLR Canon", 1, 2100.00m);        
        sales[7].AddItem(10, "Console PlayStation 5", 1, 1800.50m);        
        sales[8].AddItem(11, "Laptop MacBook Pro", 1, 2750.75m);
        sales[9].AddItem(12, "Fone de Ouvido AirPods", 1, 1200.00m);
        await context.SaveChangesAsync();

        // Cancel SALE-006 after adding items to other sales
        sales[5].Cancel();
        await context.SaveChangesAsync();
    }
} 