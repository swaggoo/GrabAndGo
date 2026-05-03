FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY . .
RUN dotnet restore "./src/services/Notification/GrabAndGo.Notification.Worker/GrabAndGo.Notification.Worker.csproj"
WORKDIR "/src/src/services/Notification/GrabAndGo.Notification.Worker"
RUN dotnet build "./GrabAndGo.Notification.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GrabAndGo.Notification.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GrabAndGo.Notification.Worker.dll"]
