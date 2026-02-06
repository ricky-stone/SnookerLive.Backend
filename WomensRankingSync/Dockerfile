FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WomensRankingSync/WomensRankingSync.csproj", "WomensRankingSync/"]
RUN dotnet restore "WomensRankingSync/WomensRankingSync.csproj"
COPY . .
WORKDIR "/src/WomensRankingSync"
RUN dotnet build "./WomensRankingSync.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WomensRankingSync.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WomensRankingSync.dll"]
