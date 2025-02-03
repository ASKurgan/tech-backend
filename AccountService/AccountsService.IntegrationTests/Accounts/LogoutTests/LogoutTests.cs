using AccountService.Contracts.Responses;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using ProjectTemplate.Application.Commands.Login;
using ProjectTemplate.Application.Commands.Logout;
using SachkovTech.Core.Abstractions;

namespace AccountsService.IntegrationTests.Accounts.LogoutTests;

public class LogoutTests : AccountTestsBase
{
    private readonly ICommandHandler<LogoutCommand> _sut;

    public LogoutTests(IntegrationTestsWebFactory factory)
        : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<LogoutCommand>>();
    }

    [Fact]
    public async Task Logout_user_to_database()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;

        await SeedRoles();
        var userId = await SeedUser();

        var loginCommand = Fixture.CreateLoginCommand();
        var loginHandler = Scope.ServiceProvider.GetRequiredService<ICommandHandler<LoginResponse, LoginCommand>>();
        var loginResult = await loginHandler.Handle(loginCommand, cancellationToken);

        loginResult.IsSuccess.Should().BeTrue();
        loginResult.Value.UserId.Should().Be(userId);

        var refreshToken = loginResult.Value.RefreshToken;

        var command = Fixture.CreateLogoutCommand(refreshToken);

        // Act
        var result = await _sut.Handle(command, cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}