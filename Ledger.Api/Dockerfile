#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Ledger.Api/Ledger.Api.csproj", "Ledger.Api/"]
COPY ["Ledger.Application/Ledger.Application.csproj", "Ledger.Application/"]
COPY ["Ledger.Infrastructure/Ledger.Infrastructure.csproj", "Ledger.Infrastructure/"]
COPY ["Ledger.Domain/Ledger.Domain.csproj", "Ledger.Domain/"]
RUN dotnet restore "./Ledger.Api/./Ledger.Api.csproj"
COPY . .
WORKDIR "/src/Ledger.Api"
RUN dotnet build "./Ledger.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Ledger.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ledger.Api.dll"]