// using System.Diagnostics;
//
// namespace FileService.Services;
//
// public class VideoProcessor
// {
//     private readonly IS3Provider _s3Provider;
//     private readonly ILogger<VideoProcessor> _logger;
//
//     public VideoProcessor(IS3Provider s3Provider, ILogger<VideoProcessor> logger)
//     {
//         _s3Provider = s3Provider;
//         _logger = logger;
//     }
//
//     /// <summary>
//     /// Скачивает видео из S3, конвертирует в HLS с несколькими разрешениями и загружает результат обратно в S3.
//     /// Возвращает URL на master.m3u8.
//     /// </summary>
//     public async Task<string> ProcessVideoAsync(string bucketName, string fileId, CancellationToken cancellationToken)
//     {
//         // Допустим, мы хотим складывать всё рядом с проектом в подпапку "TempVideos".
//         // Возьмём за основу текущую рабочую директорию (где лежит ваш .dll/.exe).
//         string baseDir = AppContext.BaseDirectory;
//         // Или можете использовать Directory.GetCurrentDirectory(), если вам это удобнее.
//
//         // Создадим внутри неё поддиректорию TempVideos
//         string videosRoot = Path.Combine(baseDir, "TempVideos");
//         Directory.CreateDirectory(videosRoot);
//
//         // Назовём поддиректорию уникально, чтобы не столкнуться с конфликтами
//         string tempDir = Path.Combine(videosRoot, fileId);
//         Directory.CreateDirectory(tempDir);
//
//         // Указываем пути
//         string inputPath = Path.Combine(tempDir, "input.mp4");
//         string hlsOutputDir = Path.Combine(tempDir, "hls");
//
//         Directory.CreateDirectory(hlsOutputDir);
//
//         _logger.LogInformation("Start processing video. Bucket: {Bucket}, FileId: {FileId}, TempDir: {TempDir}",
//             bucketName, fileId, tempDir);
//
//         try
//         {
//             // 1. Скачиваем исходный файл из S3 во временный файл
//             _logger.LogInformation("Downloading file from S3. Bucket: {Bucket}, Key: {Key}, LocalPath: {LocalPath}",
//                 bucketName, fileId, inputPath);
//
//             await _s3Provider.DownloadFileAsync(bucketName, fileId, inputPath, cancellationToken);
//             _logger.LogInformation("Download completed.");
//
//             // 2. Формируем ffmpeg-команду
//             //    (Пример с двумя профилями: 360p и 720p)
//
//             // Обратите внимание: если у вас _не_ поддерживается libx264, замените его на нужный энкодер.
//             // Например, если ffmpeg собран с openh264: "-c:v:0", "libopenh264", и т.п.
//             var ffmpegArgs = new[]
//             {
//                 "-y", "-i", $"\"{inputPath}\"", "-preset", "veryfast", "-g", "48", "-sc_threshold", "0", "-c:a", "aac", "-ar", "48000", "-b:a", "128k",
//                 "-ac", "2",
//                 // Первый профиль (v:0,a:0) -> 360p
//                 "-map", "0:v", "-map", "0:a", "-c:v:0", "libx264", "-b:v:0", "800k", "-maxrate:v:0", "856k", "-bufsize:v:0", "1200k", "-s:v:0",
//                 "640x360",
//                 // Второй профиль (v:1,a:1) -> 720p
//                 "-map", "0:v", "-map", "0:a", "-c:v:1", "libx264", "-b:v:1", "2800k", "-maxrate:v:1", "2996k", "-bufsize:v:1", "4200k", "-s:v:1",
//                 "1280x720",
//                 // HLS вывод
//                 "-var_stream_map", "\"v:0,a:0 v:1,a:1\"", "-master_pl_name", "master.m3u8", "-hls_time", "6", "-hls_playlist_type", "vod",
//                 "-hls_segment_type", "mpegts", "-hls_segment_filename", $"\"{Path.Combine(hlsOutputDir, "%v_%03d.ts")}\"", "-f", "hls",
//                 $"\"{Path.Combine(hlsOutputDir, "%v.m3u8")}\""
//             };
//
//             // 3. Генерируем строку аргументов
//             var ffmpegCmd = string.Join(" ", ffmpegArgs);
//             _logger.LogInformation("Starting ffmpeg with arguments: {FfmpegArgs}", ffmpegCmd);
//
//             // 4. Предположим, что ffmpeg.exe лежит рядом с проектом, в папке ffmpeg
//             // Например, <Проект>/ffmpeg/bin/ffmpeg.exe
//             // Тогда соберём путь:
//
//             var processInfo = new ProcessStartInfo
//             {
//                 FileName = "D:\\Projects\\sachkov-tech-backend\\FileService\\ffmpeg\\ffmpeg\\bin\\ffmpeg.exe",
//                 Arguments = ffmpegCmd,
//                 CreateNoWindow = true,
//                 UseShellExecute = false,
//                 RedirectStandardInput = false,
//                 RedirectStandardOutput = true,
//                 RedirectStandardError = true,
//             };
//
//             using var process = new Process
//             {
//                 StartInfo = processInfo
//             };
//
//             process.OutputDataReceived += (sender, e) =>
//             {
//                 if (!string.IsNullOrEmpty(e.Data))
//                     _logger.LogInformation("[ffmpeg-stdout] {Data}", e.Data);
//             };
//             process.ErrorDataReceived += (sender, e) =>
//             {
//                 if (!string.IsNullOrEmpty(e.Data))
//                     _logger.LogError("[ffmpeg-stderr] {Data}", e.Data);
//             };
//
//             process.Start();
//             process.BeginOutputReadLine();
//             process.BeginErrorReadLine();
//
//             await process.WaitForExitAsync(cancellationToken);
//
//             if (process.ExitCode != 0)
//             {
//                 _logger.LogError("FFmpeg exited with non-zero code: {ExitCode}", process.ExitCode);
//                 throw new Exception($"ffmpeg exited with code {process.ExitCode}");
//             }
//
//             _logger.LogInformation("FFmpeg processing completed successfully.");
//
//             // 5. Загружаем файлы HLS обратно в S3
//             _logger.LogInformation("Uploading HLS files to S3. HLS directory: {HlsOutputDir}", hlsOutputDir);
//
//             var hlsFiles = Directory.GetFiles(hlsOutputDir);
//
//             foreach (var filePath in hlsFiles)
//             {
//                 var fileName = Path.GetFileName(filePath);
//                 var s3ObjectKey = $"N-{fileId}/hls/{fileName}";
//
//                 _logger.LogDebug("Uploading file {LocalFile} to S3 key {S3Key}", filePath, s3ObjectKey);
//
//                 await using var fs = File.OpenRead(filePath);
//                 await _s3Provider.UploadFileAsync(bucketName, s3ObjectKey, fs, cancellationToken);
//             }
//
//             _logger.LogInformation("Upload of HLS files completed.");
//
//             // 6. Формируем presigned URL для master.m3u8
//             string masterKey = $"{fileId}/hls/master.m3u8";
//             var presignedMasterUrl = await _s3Provider.GenerateDownloadUrlAsync(bucketName, masterKey, 24);
//
//             _logger.LogInformation("HLS master playlist URL: {Url}", presignedMasterUrl);
//
//             return presignedMasterUrl;
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "Error in ProcessVideoAsync for file {FileId}", fileId);
//             throw;
//         }
//         finally
//         {
//             try
//             {
//                 if (Directory.Exists(tempDir))
//                 {
//                     Directory.Delete(tempDir, recursive: true);
//                     _logger.LogInformation("Temporary directory {TempDir} removed.", tempDir);
//                 }
//             }
//             catch (Exception dirEx)
//             {
//                 _logger.LogWarning(dirEx, "Failed to remove temporary directory {TempDir}", tempDir);
//             }
//         }
//     }
// }