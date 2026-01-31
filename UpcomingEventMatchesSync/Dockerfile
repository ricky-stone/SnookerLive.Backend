FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["UpcomingEventMatchesSync/UpcomingEventMatchesSync.csproj", "UpcomingEventMatchesSync/"]
RUN dotnet restore "UpcomingEventMatchesSync/UpcomingEventMatchesSync.csproj"
COPY . .
WORKDIR "/src/UpcomingEventMatchesSync"
RUN dotnet build "./UpcomingEventMatchesSync.csproj" -c $BUILD_CONFIGURATION -o /app/build
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./UpcomingEventMatchesSync.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UpcomingEventMatchesSync.dll"]
