FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution and project files first (for better cache)
COPY *.sln ./
COPY src/ ./src/

# Restore dependencies (this layer will be cached if project files don't change)
RUN dotnet restore

# Build the application
RUN dotnet build -c Release --no-restore

# Publish the application
RUN dotnet publish src/SalesService.Api/SalesService.Api.csproj -c Release -o /app/publish --no-build

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy only what's needed for runtime
COPY --from=build /app/publish ./

EXPOSE 5000 5001

# Start the application
ENTRYPOINT ["dotnet", "SalesService.Api.dll"] 