using Microsoft.AspNetCore.Mvc;
using SachkovTech.Core.Models;
using SharedKernel;

namespace SachkovTech.Framework;

[ApiController]
[Route("api/[controller]/{issueReviewId:guid}")]
public abstract class ApplicationController : ControllerBase
{
    public override OkObjectResult Ok(object? value)
    {
        var envelope = Envelope.Ok(value);

        return base.Ok(envelope);
    }
}