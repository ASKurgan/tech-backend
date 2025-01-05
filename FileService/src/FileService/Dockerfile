FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8003


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


COPY ["src/FileService/FileService.csproj", "src/FileService/"]
COPY ["src/FileService.Contracts/FileService.Contracts.csproj", "src/FileService.Contracts/"]
RUN dotnet restore "src/FileService/FileService.csproj"
COPY . .
WORKDIR "/src/src/FileService"
RUN dotnet build "FileService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "FileService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FileService.dll"]
