using CSharpFunctionalExtensions;
using EmailNotificationService.API.Common;

namespace EmailNotificationService.API.Extensions;

public static class UnitResultExtensions
{
    public static Response ToResponse(this UnitResult<string> result)
    {
        if (result.IsFailure)
            return new Response { StatusCode = 400, Success = false, Message = result.Error };

        return new Response { StatusCode = 200, Success = true };
    }
}

