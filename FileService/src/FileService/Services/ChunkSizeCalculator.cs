namespace FileService.Services;

public static class ChunkSizeCalculator
{
    private const long MinChunkSize = 5 * 1024 * 1024; // Минимальный размер чанка — 5 MB
    private const int MaxChunks = 10_000; // Максимальное количество чанков

    /// <summary>
    /// Рассчитывает параметры чанков: размер одного чанка и их количество
    /// </summary>
    /// <param name="fileSize">Размер файла в байтах.</param>
    /// <returns>Размер чанка и общее количество чанков.</returns>
    public static (long ChunkSize, int TotalChunks) Calculate(long fileSize)
    {
        // Расчёт размера чанка
        long chunkSize = Math.Max(MinChunkSize, (fileSize + MaxChunks - 1) / MaxChunks);

        // Расчёт количества чанков
        int totalChunks = (int)Math.Ceiling((double)fileSize / chunkSize);

        return (chunkSize, totalChunks);
    }
}