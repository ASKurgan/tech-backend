using System.Text;
using System.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Hosting;
using SachkovTech.Accounts.Application.Commands.CompleteUploadPhoto;
using SachkovTech.Accounts.Application.Commands.EnrollParticipant;
using SachkovTech.Accounts.Application.Commands.GenerateConfirmationLink;
using SachkovTech.Accounts.Application.Commands.Login;
using SachkovTech.Accounts.Application.Commands.Logout;
using SachkovTech.Accounts.Application.Commands.RefreshTokens;
using SachkovTech.Accounts.Application.Commands.Register;
using SachkovTech.Accounts.Application.Commands.StartUploadFile;
using SachkovTech.Accounts.Application.Commands.VerifyConfirmationLink;
using SachkovTech.Accounts.Application.Queries.GetUserById;
using SachkovTech.Accounts.Application.Queries.GetUsers;
using SachkovTech.Accounts.Application.Requests;
using SachkovTech.Accounts.Contracts.Requests;
using SachkovTech.Accounts.Contracts.Responses;
using SachkovTech.Accounts.Presentation.Providers;
using SachkovTech.Core.Models;
using SachkovTech.Framework;
using SachkovTech.Framework.Authorization;
using SachkovTech.Framework.Models;
using SachkovTech.SharedKernel.ValueObjects;

namespace SachkovTech.Accounts.Presentation;

public class AccountsController : ApplicationController
{
    private readonly HttpContextProvider _httpContextProvider;

    public AccountsController(HttpContextProvider httpContextProvider)
    {
        _httpContextProvider = httpContextProvider;
    }

    [HttpGet("{userId:guid}")]
    [Permission(Permissions.Accounts.READ_ACCOUNT)]
    public async Task<IActionResult> GetUserById(
        [FromRoute] Guid userId,
        [FromServices] GetUserByIdHandler handler,
        CancellationToken cancellationToken = default)
    {
        var query = new GetUserByIdQuery(userId);

        return Ok(await handler.Handle(query, cancellationToken));
    }

    [HttpGet]
    [Permission(Permissions.Accounts.READ_ACCOUNT)]
    public async Task<IActionResult> GetUsers(
        [FromQuery] GetUsersQuery query,
        [FromServices] GetUsersHandler handler,
        CancellationToken cancellationToken = default)
    {
        return Ok(await handler.Handle(query, cancellationToken));
    }

    [HttpPost("confirmation-link/{userId:guid}")]
    public async Task<IActionResult> GetConfirmationLink(
        [FromRoute] Guid userId,
        [FromServices] GenerateConfirmationLinkHandler handler,
        CancellationToken cancellationToken = default)
    {
        var command = new GenerateConfirmationLinkCommand(userId);

        var result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.ToResponse();
        }

        return Ok(result.Value);
    }

    [HttpPost("admin-token-for-test")]
    public async Task<IActionResult> Testing(
        [FromServices] LoginHandler handler,
        [FromServices] IWebHostEnvironment env,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(
            new LoginCommand("admin@admin.com", "!Admin123"),
            cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        var setRefreshSessionCookieRes = _httpContextProvider.SetRefreshSessionCookie(result.Value.RefreshToken);

        if (setRefreshSessionCookieRes.IsFailure)
        {
            return setRefreshSessionCookieRes.Error.ToResponse();
        }

        return Ok(result.Value);
    }

    [HttpPost("registration")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterUserRequest request,
        [FromServices] RegisterUserHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(
            new RegisterUserCommand(
                request.Email,
                request.UserName,
                request.Password),
            cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok();
    }

    [HttpPost("email-verification/{code:required}")]
    public async Task<IActionResult> VerifyEmail(
        [FromRoute] string code,
        [FromServices] VerifyConfirmationLinkHandler handler,
        [FromServices] UserScopedData userScopedData,
        CancellationToken cancellationToken)
    {
        string decodedToken = NormalizeBase64UrlStringAndGetResult(code);

        var command = new VerifyConfirmationLinkCommand(userScopedData.UserId, decodedToken);

        var result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginUserRequest request,
        [FromServices] LoginHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(
            new LoginCommand(request.Email, request.Password),
            cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        var setRefreshSessionCookieRes = _httpContextProvider.SetRefreshSessionCookie(result.Value.RefreshToken);

        if (setRefreshSessionCookieRes.IsFailure)
        {
            return setRefreshSessionCookieRes.Error.ToResponse();
        }

        return Ok(result.Value);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshTokens(
        [FromServices] RefreshTokensHandler handler,
        CancellationToken cancellationToken)
    {
        var getRefreshSessionCookieRes = _httpContextProvider.GetRefreshSessionCookie();

        if (getRefreshSessionCookieRes.IsFailure)
        {
            return Unauthorized();
        }

        var result = await handler.Handle(
            new RefreshTokensCommand(getRefreshSessionCookieRes.Value),
            cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        var setRefreshSessionCookieRes = _httpContextProvider.SetRefreshSessionCookie(result.Value.RefreshToken);

        if (setRefreshSessionCookieRes.IsFailure)
        {
            return setRefreshSessionCookieRes.Error.ToResponse();
        }

        return Ok(result.Value);
    }

    // TODO: уберите это отсюда
    [HttpPost("/start-upload-photo")]
    [Permission(Permissions.Issues.UPDATE_ISSUE)]
    public async Task<IActionResult> StartUploadPhoto(
        [FromServices] StartUploadPhotoHandler handler,
        [FromServices] UserScopedData userScopedData,
        [FromBody] FileMetadataRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new StartUploadPhotoCommand(
            userScopedData.UserId,
            request.FileName,
            request.ContentType,
            request.FileSize);

        var result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
            result.Error.ToResponse();

        return Ok(result.Value);
    }

    // TODO: уберите это отсюда
    [HttpPost("/complete-upload-photo")]
    [Permission(Permissions.Issues.UPDATE_ISSUE)]
    public async Task<IActionResult> CompleteUploadPhoto(
        [FromServices] CompleteUploadPhotoHandler handler,
        [FromServices] UserScopedData userScopedData,
        [FromBody] CompleteMultipartUploadRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new CompleteUploadPhotoCommand(
            userScopedData.UserId,
            request.FileMetadata.FileName,
            request.FileMetadata.ContentType,
            request.FileMetadata.FileSize,
            request.UploadId,
            request.Parts);

        var result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
            result.Error.ToResponse();

        return Ok();
    }

    // TODO: для общей стилистики можно переделать в log-out , но придется искать еще на фронте
    [HttpPost("logOut")]
    public async Task<IActionResult> Logout(
        [FromServices] LogoutHandler handler,
        CancellationToken cancellationToken)
    {
        var getRefreshSessionCookieRes = _httpContextProvider.GetRefreshSessionCookie();

        if (getRefreshSessionCookieRes.IsFailure)
        {
            return Unauthorized();
        }

        var result = await handler.Handle(
            new LogoutCommand(getRefreshSessionCookieRes.Value),
            cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        var deleteRefreshSessionCookieRes = _httpContextProvider.DeleteRefreshSessionCookie();

        if (deleteRefreshSessionCookieRes.IsFailure)
        {
            return deleteRefreshSessionCookieRes.Error.ToResponse();
        }

        return Ok();
    }

    [Permission(Permissions.Accounts.ENROLL_ACCOUNT)]
    [HttpPut("student-role")]
    public async Task<ActionResult> EnrollParticipant(
        [FromBody] EnrollParticipantRequest request,
        [FromServices] EnrollParticipantHandler handler,
        CancellationToken cancellationToken = default)
    {
        var command = new EnrollParticipantCommand(request.Email);

        var result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok();
    }

    private static string NormalizeBase64UrlStringAndGetResult(string input)
    {
        // Заменяем символы URL в стандартные элементы Base64
        var base64 = input.Replace('-', '+').Replace('_', '/');

        // Длина должна делиться на 4, поэтому мы добавляем недостающие знаки "="
        switch (input.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }

        var decodedBytes = WebEncoders.Base64UrlDecode(base64);

        return Encoding.UTF8.GetString(decodedBytes);
    }
}