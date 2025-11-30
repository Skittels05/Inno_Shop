FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["Products.API/Products.API/Products.API.csproj", "Products.API/Products.API/"]
COPY ["Products.Application/Products.Application.csproj", "Products.Application/"]
COPY ["Products.Domain/Products.Domain.csproj", "Products.Domain/"]
COPY ["Products.Infrastructure/Products.Infrastructure/Products.Infrastructure.csproj", "Products.Infrastructure/Products.Infrastructure/"]

RUN dotnet restore "Products.API/Products.API/Products.API.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/Products.API/Products.API"
RUN dotnet build "Products.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Products.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Products.API.dll"]

