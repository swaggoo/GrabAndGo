FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER root
RUN mkdir -p /app/wwwroot/uploads && chown -R app:app /app/wwwroot
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY . .
RUN dotnet restore "./src/services/Media/GrabAndGo.Media.API/GrabAndGo.Media.API.csproj"
WORKDIR "/src/src/services/Media/GrabAndGo.Media.API"
RUN dotnet build "./GrabAndGo.Media.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GrabAndGo.Media.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GrabAndGo.Media.API.dll"]
