FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["FinishedEventMatchesSync/FinishedEventMatchesSync.csproj", "FinishedEventMatchesSync/"]
RUN dotnet restore "FinishedEventMatchesSync/FinishedEventMatchesSync.csproj"
COPY . .
WORKDIR "/src/FinishedEventMatchesSync"
RUN dotnet build "./FinishedEventMatchesSync.csproj" -c $BUILD_CONFIGURATION -o /app/build
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FinishedEventMatchesSync.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FinishedEventMatchesSync.dll"]
