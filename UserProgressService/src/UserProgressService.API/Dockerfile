FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/UserProgressService.API/UserProgressService.API.csproj", "src/UserProgressService.API/"]
COPY ["src/UserProgressService.Application/UserProgressService.Application.csproj", "src/UserProgressService.Application/"]
COPY ["src/UserProgressService.Domain/UserProgressService.Domain.csproj", "src/UserProgressService.Domain/"]
COPY ["src/UserProgressService.Infrastructure/UserProgressService.Infrastructure.csproj", "src/UserProgressService.Infrastructure/"]
RUN dotnet restore "src/UserProgressService.API/UserProgressService.API.csproj"
COPY . .
WORKDIR "/src/src/UserProgressService.API"
RUN dotnet build "UserProgressService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "UserProgressService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserProgressService.API.dll"]
