using AccountService.Application.Commands.Register;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SachkovTech.Core.Abstractions;

namespace AccountsService.IntegrationTests.Accounts.RegisterTests;

public class RegisterTests : AccountTestsBase
{
    private readonly ICommandHandler<RegisterUserCommand> _sut;

    public RegisterTests(IntegrationTestsWebFactory factory)
        : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<RegisterUserCommand>>();
    }

    [Fact]
    public async Task RegisterUserIntegrationTest_ShouldSucceed()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;

        await SeedRoles();

        var command = Fixture.CreateRegisterUserCommand();

        // Act
        var result = await _sut.Handle(command, cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}