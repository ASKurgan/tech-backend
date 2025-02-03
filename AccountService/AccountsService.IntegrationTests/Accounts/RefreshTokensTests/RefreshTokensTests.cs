using AccountService.Contracts.Responses;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using ProjectTemplate.Application.Commands.Login;
using ProjectTemplate.Application.Commands.RefreshTokens;
using SachkovTech.Core.Abstractions;

namespace AccountsService.IntegrationTests.Accounts.RefreshTokensTests;

public class RefreshTokensTests : AccountTestsBase
{
    private readonly ICommandHandler<LoginResponse, RefreshTokensCommand> _sut;

    public RefreshTokensTests(IntegrationTestsWebFactory factory)
        : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<LoginResponse, RefreshTokensCommand>>();
    }

    [Fact]
    public async Task RefreshTokens_ShouldGenerateNewTokens()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;

        await SeedRoles();
        await SeedUser();

        var loginCommand = Fixture.CreateLoginCommand();
        var loginHandler = Scope.ServiceProvider.GetRequiredService<ICommandHandler<LoginResponse, LoginCommand>>();
        var loginResult = await loginHandler.Handle(loginCommand, cancellationToken);

        loginResult.IsSuccess.Should().BeTrue();
        var oldRefreshToken = loginResult.Value.RefreshToken;

        var refreshTokensCommand = Fixture.CreateRefreshTokensCommand(oldRefreshToken);

        // Act
        var refreshResult = await _sut.Handle(refreshTokensCommand, cancellationToken);

        // Assert
        refreshResult.IsSuccess.Should().BeTrue();
        refreshResult.Value.Should().NotBeNull();

        var newAccessToken = refreshResult.Value.AccessToken;
        var newRefreshToken = refreshResult.Value.RefreshToken;

        newAccessToken.Should().NotBeNullOrEmpty();
        newRefreshToken.Should().NotBe(Guid.Empty).And.NotBe(oldRefreshToken);

        var oldSessionResult = await RefreshSessionManager.GetByRefreshToken(oldRefreshToken, cancellationToken);

        oldSessionResult.IsFailure.Should().BeTrue();
    }
}