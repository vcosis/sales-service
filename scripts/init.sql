-- Seed data for Sales database
-- This file will be executed when the PostgreSQL container starts

-- Insert sample sales data
INSERT INTO "Sales" ("SaleNumber", "Date", "CustomerId", "CustomerName", "BranchId", "BranchName", "TotalAmount", "Cancelled") VALUES
('SALE-001', '2024-01-15 10:30:00+00', 1, 'João Silva', 1, 'Loja Centro', 1500.00, false),
('SALE-002', '2024-01-15 14:20:00+00', 2, 'Maria Santos', 1, 'Loja Centro', 2300.50, false),
('SALE-003', '2024-01-16 09:15:00+00', 3, 'Pedro Oliveira', 2, 'Loja Norte', 890.75, false),
('SALE-004', '2024-01-16 16:45:00+00', 4, 'Ana Costa', 2, 'Loja Norte', 3200.00, false),
('SALE-005', '2024-01-17 11:30:00+00', 5, 'Carlos Ferreira', 3, 'Loja Sul', 1750.25, false),
('SALE-006', '2024-01-17 13:20:00+00', 1, 'João Silva', 3, 'Loja Sul', 950.00, true),
('SALE-007', '2024-01-18 08:45:00+00', 6, 'Lucia Martins', 1, 'Loja Centro', 2100.00, false),
('SALE-008', '2024-01-18 15:10:00+00', 7, 'Roberto Lima', 2, 'Loja Norte', 1800.50, false),
('SALE-009', '2024-01-19 12:00:00+00', 8, 'Fernanda Rocha', 3, 'Loja Sul', 2750.75, false),
('SALE-010', '2024-01-19 17:30:00+00', 9, 'Marcos Alves', 1, 'Loja Centro', 1200.00, false);

-- Insert sample sale items data
INSERT INTO "SaleItems" ("ProductId", "ProductName", "Quantity", "UnitPrice", "Discount", "Total", "SaleId") VALUES
-- Items for SALE-001
(1, 'Notebook Dell Inspiron', 1, 1500.00, 0.00, 1500.00, 1),

-- Items for SALE-002
(2, 'Mouse Wireless Logitech', 2, 150.00, 10.00, 270.00, 2),
(3, 'Teclado Mecânico RGB', 1, 450.00, 0.00, 450.00, 2),
(4, 'Monitor 24" Samsung', 1, 1580.50, 0.00, 1580.50, 2),

-- Items for SALE-003
(5, 'Headphone Bluetooth', 1, 890.75, 0.00, 890.75, 3),

-- Items for SALE-004
(6, 'Smartphone iPhone 15', 1, 3200.00, 0.00, 3200.00, 4),

-- Items for SALE-005
(7, 'Tablet Samsung Galaxy', 1, 1750.25, 0.00, 1750.25, 5),

-- Items for SALE-006 (Cancelled sale)
(8, 'Smartwatch Apple Watch', 1, 950.00, 0.00, 950.00, 6),

-- Items for SALE-007
(9, 'Câmera DSLR Canon', 1, 2100.00, 0.00, 2100.00, 7),

-- Items for SALE-008
(10, 'Console PlayStation 5', 1, 1800.50, 0.00, 1800.50, 8),

-- Items for SALE-009
(11, 'Laptop MacBook Pro', 1, 2750.75, 0.00, 2750.75, 9),

-- Items for SALE-010
(12, 'Fone de Ouvido AirPods', 1, 1200.00, 0.00, 1200.00, 10); 