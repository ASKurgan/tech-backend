using Microsoft.AspNetCore.Routing;

namespace SachkovTech.Framework.Endpoints;

public interface IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app);
}