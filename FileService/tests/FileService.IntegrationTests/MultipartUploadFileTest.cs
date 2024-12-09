using System.Net.Http.Json;
using Amazon.S3.Model;
using FileService.Contracts;
using FileService.Contracts.Responses;
using FluentAssertions;
using MongoDB.Driver;
using CompleteMultipartUploadResponse = FileService.Contracts.Responses.CompleteMultipartUploadResponse;

namespace FileService.IntegrationTests;

public class MultipartUploadFileTest : FileServiceTestsBase
{
    public MultipartUploadFileTest(
        IntegrationTestsWebFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Multipart_Upload_File()
    {
        // arrange
        FileInfo fileInfo = new FileInfo(
            $"{Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName}/test.mp4");
        
        var cancellationToken = new CancellationTokenSource().Token;
        
        var startMultipartResult = await StartMultipartUpload(fileInfo, cancellationToken);
        
        int chunkSize = 1024 * 1024 * 5;
        
        var parts = new List<PartETagInfo>();
        int partNumber = 1;
        
        await using var stream = fileInfo.OpenRead();

        for (int start = 0; start < fileInfo.Length; start += chunkSize)
        {
            byte[] chunk = new byte[chunkSize];
            await stream.ReadAsync(chunk, 0, chunkSize, cancellationToken);

            var uploadPresignedPartUrlResult = await UploadPresignedPartUrl(
                startMultipartResult,
                partNumber,
                cancellationToken);

            var eTag = await UploadFilePartToMinio(
                uploadPresignedPartUrlResult,
                chunk,
                cancellationToken); 
            
            parts.Add(new PartETagInfo(partNumber, eTag!));
            partNumber++;
        }
        
        // act
        var completeMultipartResult = await CompleteMultipartUpload(
            startMultipartResult,
            parts,
            cancellationToken);

        // assert
        var file = MongoDbContext.Files.Find(f => f.StoragePath == completeMultipartResult.Id).FirstOrDefault();

        file.Should().NotBeNull();
    }

    private async Task<StartMultipartUploadResponse> StartMultipartUpload(
        FileInfo fileInfo, 
        CancellationToken cancellationToken = default)
    {
        var request = new StartMultipartUploadRequest(
            fileInfo.Name,
            "video/mp4",
            fileInfo.Length);
        
        var response = await AppHttpClient
            .PostAsJsonAsync("files/multipart", request, cancellationToken);

        response.EnsureSuccessStatusCode();
        
        return (await response.Content
            .ReadFromJsonAsync<StartMultipartUploadResponse>(cancellationToken))!;
    }
    
    private async Task<UploadPresignedPartUrlResponse> UploadPresignedPartUrl(
        StartMultipartUploadResponse startMultipartResult,
        int partNumber,
        CancellationToken cancellationToken = default)
    {
        var request = new UploadPresignedPartUrlRequest
            (startMultipartResult!.UploadId, partNumber);

        var response = await AppHttpClient
            .PostAsJsonAsync(
                $"files/{startMultipartResult.Key}/presigned-part",
                request,
                cancellationToken);

        response.EnsureSuccessStatusCode();

        return (await response.Content
            .ReadFromJsonAsync<UploadPresignedPartUrlResponse>(cancellationToken))!;
    }

    private async Task<string> UploadFilePartToMinio(
        UploadPresignedPartUrlResponse uploadPresignedPartUrlResult,
        byte[] chunk,
        CancellationToken cancellationToken = default)
    {
        using var content = new ByteArrayContent(chunk);
                
        var response = await HttpClient
            .PutAsync(uploadPresignedPartUrlResult!.Url, content, cancellationToken);

        var message = HttpClient.BaseAddress;

        response.EnsureSuccessStatusCode();
                
        return response.Headers.GetValues("etag").FirstOrDefault()!;
    }

    private async Task<CompleteMultipartUploadResponse> CompleteMultipartUpload(
        StartMultipartUploadResponse startMultipartResult,
        List<PartETagInfo> parts,
        CancellationToken cancellationToken = default)
    {
        var completeMultipartRequest = new CompleteMultipartRequest(startMultipartResult!.UploadId, parts);

        var response =  await AppHttpClient.PostAsJsonAsync(
            $"files/{startMultipartResult.Key}/complete-multipart",
            completeMultipartRequest,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        return (await response.Content
            .ReadFromJsonAsync<CompleteMultipartUploadResponse>(cancellationToken))!;
    }
}