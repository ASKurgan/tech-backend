FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8002

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

COPY ["src/SachkovTech.Web/SachkovTech.Web.csproj", "src/SachkovTech.Web/"]
COPY ["src/Issues/SachkovTech.Issues.Infrastructure/SachkovTech.Issues.Infrastructure.csproj", "src/Issues/SachkovTech.Issues.Infrastructure/"]
COPY ["src/Issues/SachkovTech.Issues.Application/SachkovTech.Issues.Application.csproj", "src/Issues/SachkovTech.Issues.Application/"]
COPY ["src/Issues/SachkovTech.Issues.Contracts/SachkovTech.Issues.Contracts.csproj", "src/Issues/SachkovTech.Issues.Contracts/"]
COPY ["src/Issues/SachkovTech.Issues.Domain/SachkovTech.Issues.Domain.csproj", "src/Issues/SachkovTech.Issues.Domain/"]
COPY ["src/Issues/SachkovTech.Issues.Presentation/SachkovTech.Issues.Presentation.csproj", "src/Issues/SachkovTech.Issues.Presentation/"]
RUN dotnet restore "src/SachkovTech.Web/SachkovTech.Web.csproj"
COPY . .
WORKDIR "/src/src/SachkovTech.Web"
RUN dotnet build "SachkovTech.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SachkovTech.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SachkovTech.Web.dll"]
