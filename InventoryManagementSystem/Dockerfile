FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .

# ✅ Restore using project instead of solution
RUN dotnet restore InventoryManagementSystem/InventoryManagementSystem/InventoryManagementSystem.csproj

# Build & publish
RUN dotnet publish InventoryManagementSystem/InventoryManagementSystem/InventoryManagementSystem.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "InventoryManagementSystem.dll"]
