FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copies everything from the folder where the Dockerfile is (Image 2)
COPY . .

# ✅ Correct path relative to the Dockerfile
RUN dotnet restore InventoryManagementSystem/InventoryManagementSystem.csproj

# Build & publish using the same corrected path
RUN dotnet publish InventoryManagementSystem/InventoryManagementSystem.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "InventoryManagementSystem.dll"]
