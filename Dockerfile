FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["TipMolde.API/TipMolde.API.csproj", "TipMolde.API/"]
COPY ["TipMolde.Core/TipMolde.Core.csproj", "TipMolde.Core/"]
COPY ["TipMolde.Infrastructure/TipMolde.Infrastructure.csproj", "TipMolde.Infrastructure/"]

RUN dotnet restore "TipMolde.API/TipMolde.API.csproj"

COPY . .
WORKDIR "/src/TipMolde.API"
RUN dotnet publish "TipMolde.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TipMolde.API.dll"]
