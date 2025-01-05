FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG NUGET_USERNAME
ARG NUGET_PASSWORD
WORKDIR /src

RUN dotnet nuget add source https://nuget.pkg.github.com/SachkovTech/index.json \
    -n github \
    --username $NUGET_USERNAME \
    --password $NUGET_PASSWORD \
    --store-password-in-clear-text
    
COPY ["src/NotificationService/NotificationService.csproj", "src/NotificationService/"]
RUN dotnet restore "src/NotificationService/NotificationService.csproj"
COPY . .
WORKDIR "/src/src/NotificationService"
RUN dotnet build "NotificationService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "NotificationService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NotificationService.dll"]
