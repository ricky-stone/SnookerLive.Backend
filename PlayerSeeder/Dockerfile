FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["PlayerSeeder/PlayerSeeder.csproj", "PlayerSeeder/"]
RUN dotnet restore "PlayerSeeder/PlayerSeeder.csproj"
COPY . .
WORKDIR "/src/PlayerSeeder"
RUN dotnet build "./PlayerSeeder.csproj" -c $BUILD_CONFIGURATION -o /app/build
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./PlayerSeeder.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PlayerSeeder.dll"]
