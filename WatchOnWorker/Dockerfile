FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WatchOnWorker/WatchOnWorker.csproj", "WatchOnWorker/"]
RUN dotnet restore "WatchOnWorker/WatchOnWorker.csproj"
COPY . .
WORKDIR "/src/WatchOnWorker"
RUN dotnet build "./WatchOnWorker.csproj" -c $BUILD_CONFIGURATION -o /app/build
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WatchOnWorker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WatchOnWorker.dll"]
