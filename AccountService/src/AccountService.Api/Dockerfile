FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8001

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


COPY ["src/AccountService.Api/AccountService.Api.csproj", "src/AccountService.Api/"]
COPY ["src/AccountService.Application/AccountService.Application.csproj", "src/AccountService.Application/"]
COPY ["src/AccountService.Contracts/AccountService.Contracts.csproj", "src/AccountService.Contracts/"]
COPY ["src/AccountService.Domain/AccountService.Domain.csproj", "src/AccountService.Domain/"]
COPY ["src/AccountService.Infrastructure/AccountService.Infrastructure.csproj", "src/AccountService.Infrastructure/"]
RUN dotnet restore "src/AccountService.Api/AccountService.Api.csproj"
COPY . .
WORKDIR "/src/src/AccountService.Api"
RUN dotnet build "AccountService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AccountService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AccountService.Api.dll"]
