using Microsoft.AspNetCore.Http;
using SharedKernel;

namespace SachkovTech.Framework.Endpoints;

public static class ResultResponse
{
    public static IResult Ok(object? value = null)
    {
        var envelope = Envelope.Ok(value);
        return Results.Ok(envelope);
    }

    public static IResult BadRequest(ErrorList errors)
    {
        var envelope = Envelope.Error(errors);
        return Results.BadRequest(envelope);
    }

    public static IResult NotFound(ErrorList errors)
    {
        var envelope = Envelope.Error(errors);
        return Results.NotFound(envelope);
    }
}