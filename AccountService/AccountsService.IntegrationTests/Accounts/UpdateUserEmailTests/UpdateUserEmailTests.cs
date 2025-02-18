using AccountService.Application.Commands.UpdateEmail;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SachkovTech.Core.Abstractions;

namespace AccountsService.IntegrationTests.Accounts.UpdateUserEmailTests;

public class UpdateUserEmailTests : AccountTestsBase
{
    private readonly ICommandHandler<Guid, UpdateEmailCommand> _sut;

    public UpdateUserEmailTests(IntegrationTestsWebFactory factory)
        : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, UpdateEmailCommand>>();
    }

    [Fact]
    public async Task UpdateUserEmailIntegrationTest_ShouldSucceed()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;

        await SeedRoles();

        var userId = await SeedUser();

        var command = Fixture.CreateUpdateUserEmailCommand(userId, "NewEmail@email.com");

        // Act
        var result = await _sut.Handle(command, cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}