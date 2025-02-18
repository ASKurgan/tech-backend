FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8003

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
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

# Установка переменных для ffmpeg
ENV BIN=/usr/bin/
ENV ffmpeg=/app/ffmpeg/
ENV ffmpegBin=/app/ffmpeg/ffmpeg-master-latest-linux64-lgpl/bin

# Копирование архива ffmpeg
WORKDIR /app
COPY ffmpeg/ffmpeg-master-latest-linux64-lgpl.tar.xz $ffmpeg/

# Выполнение установки под root
USER root

# Распаковка и настройка ffmpeg
RUN apt-get update && apt-get install -y xz-utils \
    && mkdir -p $ffmpeg \
    && tar -xvf $ffmpeg/ffmpeg-master-latest-linux64-lgpl.tar.xz -C $ffmpeg \
    && rm $ffmpeg/ffmpeg-master-latest-linux64-lgpl.tar.xz \
    && chmod +x $ffmpegBin/ffmpeg \
    && chmod +x $ffmpegBin/ffprobe \
    && ln -s $ffmpegBin/ffmpeg $BIN \
    && ln -s $ffmpegBin/ffprobe $BIN \
    && apt-get remove -y xz-utils \
    && apt-get clean && rm -rf /var/lib/apt/lists/* 

# Возвращаемся к приложению
USER $APP_UID

# Копирование опубликованного приложения
COPY --from=publish /app/publish .

# Точка входа для приложения
ENTRYPOINT ["dotnet", "FileService.dll"]