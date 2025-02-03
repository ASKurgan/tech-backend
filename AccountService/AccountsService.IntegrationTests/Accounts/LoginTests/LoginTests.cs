using AccountService.Contracts.Responses;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using ProjectTemplate.Application.Commands.Login;
using SachkovTech.Core.Abstractions;

namespace AccountsService.IntegrationTests.Accounts.LoginTests;

public class LoginTests : AccountTestsBase
{
    private readonly ICommandHandler<LoginResponse, LoginCommand> _sut;

    public LoginTests(IntegrationTestsWebFactory factory)
        : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<LoginResponse, LoginCommand>>();
    }

    [Fact]
    public async Task Login_user_and_get_token()
    {
        // arrange
        var cancellationToken = new CancellationTokenSource().Token;

        await SeedRoles();

        var userId = await SeedUser();

        var command = Fixture.CreateLoginCommand();

        // act
        var result = await _sut.Handle(command, cancellationToken);

        // assert
        result.IsSuccess.Should().BeTrue();

        result.Value.UserId.Should().Be(userId);
        result.Value.AccessToken.Should().NotBeNullOrEmpty();
        result.Value.RefreshToken.Should().NotBe(Guid.Empty);
    }
}