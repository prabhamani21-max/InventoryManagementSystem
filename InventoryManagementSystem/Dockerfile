FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy entire solution (VERY IMPORTANT)
COPY . .

# Restore solution
RUN dotnet restore InventoryManagementSystem.slnx

# Publish API
RUN dotnet publish InventoryManagementSystem/InventoryManagementSystem/InventoryManagementSystem.csproj -c Release -o /app/publish

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "InventoryManagementSystem.dll"]
