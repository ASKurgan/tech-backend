using AccountService.Contracts.Requests;
using Microsoft.AspNetCore.Mvc;
using ProjectTemplate.Application.Commands.CompleteUploadPhoto;
using ProjectTemplate.Application.Commands.EnrollParticipant;
using ProjectTemplate.Application.Commands.GenerateConfirmationLink;
using ProjectTemplate.Application.Commands.Login;
using ProjectTemplate.Application.Commands.Logout;
using ProjectTemplate.Application.Commands.RefreshTokens;
using ProjectTemplate.Application.Commands.Register;
using ProjectTemplate.Application.Commands.StartUploadFile;
using ProjectTemplate.Application.Commands.UpdateUserEmail;
using ProjectTemplate.Application.Commands.UpdateUserFullName;
using ProjectTemplate.Application.Commands.UpdateUserName;
using ProjectTemplate.Application.Commands.UpdateUserPhoneNumber;
using ProjectTemplate.Application.Commands.UpdateUserSocialNetworks;
using ProjectTemplate.Application.Commands.VerifyConfirmationLink;
using ProjectTemplate.Application.Queries.GetUserById;
using ProjectTemplate.Application.Queries.GetUsers;
using ProjectTemplate.Providers;
using SachkovTech.Framework;
using SachkovTech.Framework.Authorization;

namespace ProjectTemplate.Controllers;

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

        var result = await handler.Handle(query, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
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

    [Permission(Permissions.Accounts.READ_ACCOUNT)]
    [HttpPost("confirmation-link/{userId:guid}")]
    public async Task<IActionResult> GetConfirmationLink(
        [FromRoute] Guid userId,
        [FromServices] GenerateConfirmationLinkHandler handler,
        CancellationToken cancellationToken = default)
    {
        var command = new GenerateConfirmationLinkCommand(userId);

        var result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

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
            return setRefreshSessionCookieRes.Error.ToResponse();

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
        var command = new VerifyConfirmationLinkCommand(userScopedData.UserId, code);

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
            return setRefreshSessionCookieRes.Error.ToResponse();

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
            return setRefreshSessionCookieRes.Error.ToResponse();

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
            return deleteRefreshSessionCookieRes.Error.ToResponse();

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

    [Permission(Permissions.Accounts.ENROLL_ACCOUNT)]
    [HttpPut("{userId:guid}/user-name")]
    public async Task<ActionResult> UpdateUserName(
        [FromRoute] Guid userId,
        [FromBody] UpdateUserNameRequest request,
        [FromServices] UpdateUserNameHandler handler,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateUserNameCommand(userId, request.UserName);

        var result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [Permission(Permissions.Accounts.ENROLL_ACCOUNT)]
    [HttpPut("{userId:guid}/full-name")]
    public async Task<ActionResult> UpdateUserFullName(
        [FromRoute] Guid userId,
        [FromBody] UpdateUserFullNameRequest request,
        [FromServices] UpdateUserFullNameHandler handler,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateUserFullNameCommand(userId, request.FullName);

        var result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [Permission(Permissions.Accounts.ENROLL_ACCOUNT)]
    [HttpPut("{userId:guid}/email")]
    public async Task<ActionResult> UpdateUserEmail(
        [FromRoute] Guid userId,
        [FromBody] UpdateUserEmailRequest request,
        [FromServices] UpdateUserEmailHandler handler,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateUserEmailCommand(userId, request.Email);

        var result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [Permission(Permissions.Accounts.ENROLL_ACCOUNT)]
    [HttpPut("{userId:guid}/phone-number")]
    public async Task<ActionResult> UpdateUserPhoneNumber(
        [FromRoute] Guid userId,
        [FromBody] UpdateUserPhoneNumberRequest request,
        [FromServices] UpdateUserPhoneNumberHandler handler,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateUserPhoneNumberCommand(userId, request.PhoneNumber);

        var result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [Permission(Permissions.Accounts.ENROLL_ACCOUNT)]
    [HttpPut("{userId:guid}/social-networks")]
    public async Task<ActionResult> UpdateUserSocialNetworks(
        [FromRoute] Guid userId,
        [FromBody] UpdateUserSocialNetworksRequest request,
        [FromServices] UpdateUserSocialNetworksHandler handler,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateUserSocialNetworksCommand(userId, request.SocialNetworks);

        var result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }
}