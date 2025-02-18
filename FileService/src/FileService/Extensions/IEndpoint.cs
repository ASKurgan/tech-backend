namespace FileService.Extensions;

public interface IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app);
}