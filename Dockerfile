FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["TipMolde/TipMolde.API.csproj", "TipMolde/"]
COPY ["TipMolde.Domain/TipMolde.Domain.csproj", "TipMolde.Domain/"]
COPY ["TipMolde.Application/TipMolde.Application.csproj", "TipMolde.Application/"]
COPY ["TipMolde.Infrastructure/TipMolde.Infrastructure.csproj", "TipMolde.Infrastructure/"]

RUN dotnet restore "TipMolde/TipMolde.API.csproj"

COPY . .
WORKDIR "/src/TipMolde"
RUN dotnet publish "TipMolde.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TipMolde.API.dll"]
