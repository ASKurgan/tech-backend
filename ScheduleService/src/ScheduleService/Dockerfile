FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

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


COPY ["src/ScheduleService/ScheduleService.csproj", "src/ScheduleService/"]
COPY ["src/ScheduleService.Infrastructure/ScheduleService.Infrastructure.csproj", "src/ScheduleService.Infrastructure/"]
COPY ["src/ScheduleService.Application/ScheduleService.Application.csproj", "src/ScheduleService.Application/"]
COPY ["src/ScheduleService.Domain/ScheduleService.Domain.csproj", "src/ScheduleService.Domain/"]
RUN dotnet restore "./src/ScheduleService/ScheduleService.csproj"
COPY . .
WORKDIR "/src/src/ScheduleService"
RUN dotnet build "./ScheduleService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ScheduleService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ScheduleService.dll"]