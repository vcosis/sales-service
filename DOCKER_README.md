# 🐳 Docker Setup - Sales Service

## 🚀 Quick Start

```bash
# Start the application with automatic migrations
docker-compose up --build -d

# Check if everything is running
docker-compose ps

# View logs
docker-compose logs sales-api
```

## 📋 What Happens Automatically

When you run `docker-compose up`, the following happens **automatically**:

1. ✅ **PostgreSQL** starts and becomes healthy
2. ✅ **API** waits for database to be ready
3. ✅ **Migrations** run automatically in the code
4. ✅ **Application** starts and becomes available

## 🌱 Manual Seed (Optional)

To add sample data to the database, execute the SQL file:

```bash
# Execute the SQL file directly
docker-compose exec postgres psql -U postgres -d salesdb -f /app/scripts/init.sql
```

**Note:** The seed data includes:
- 10 sales records with various customers and branches
- 12 sale items with different products and prices
- 1 cancelled sale for testing

## 🔧 Useful Commands

```bash
# View application logs
docker-compose logs sales-api

# View database logs
docker-compose logs postgres

# Access the database directly
docker-compose exec postgres psql -U postgres -d salesdb

# Restart the application
docker-compose restart sales-api

# Stop everything
docker-compose down

# Stop and remove volumes (⚠️ deletes all data)
docker-compose down -v
```

## 🌐 Access Points

- **Swagger UI:** http://localhost:5000/swagger
- **API Base URL:** http://localhost:5000
- **PostgreSQL:** localhost:5432

## 🏗️ Architecture

### Services
- **sales-api:** .NET 8 API with automatic migrations in code
- **postgres:** PostgreSQL 15 database

### Volumes
- **postgres_data:** Persistent database storage

### Networks
- **sales-network:** Internal communication between services

## 🔍 Troubleshooting

### Container won't start
```bash
# Check logs
docker-compose logs sales-api

# Rebuild and restart
docker-compose down
docker-compose up --build -d
```

### Database connection issues
```bash
# Check if postgres is healthy
docker-compose ps

# Check postgres logs
docker-compose logs postgres
```

### Migration issues
```bash
# Check migration logs
docker-compose logs sales-api | grep -i migration
```

## 📝 Environment Variables

| Variable | Default | Description |
|----------|---------|-------------|
| `POSTGRES_DB` | `salesdb` | Database name |
| `POSTGRES_USER` | `postgres` | Database user |
| `POSTGRES_PASSWORD` | `S@le5#01!` | Database password |
| `ASPNETCORE_ENVIRONMENT` | `Development` | .NET environment |

## 🎯 Best Practices

1. **Always use `--build`** when making code changes
2. **Check logs** if containers fail to start
3. **Seed manually** only when needed
4. **Use volumes** to persist database data
5. **Health checks** ensure proper startup order
6. **Migrations run automatically** in the application code

## 🚀 Performance Optimizations

- **Fast builds:** Optimized Dockerfile with better cache usage
- **Efficient layers:** Combined RUN commands and optimized copy operations
- **NuGet optimization:** Fast package sources and caching
- **No startup scripts:** Migrations run directly in .NET code
- **SQL file execution:** Clean and organized seed approach 