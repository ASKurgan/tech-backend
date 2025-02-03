using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using ProjectTemplate.Application.Commands.UpdateUserName;
using SachkovTech.Core.Abstractions;

namespace AccountsService.IntegrationTests.Accounts.UpdateUserName;

public class UpdateUserNameTests : AccountTestsBase
{
    private readonly ICommandHandler<Guid, UpdateUserNameCommand> _sut;

    public UpdateUserNameTests(
        IntegrationTestsWebFactory factory)
        : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, UpdateUserNameCommand>>();
    }

    [Fact]
    public async Task UpdateUserNameIntegrationTest_ShouldSucceed()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;

        await SeedRoles();

        var userId = await SeedUser();

        var command = Fixture.CreateUpdateUserNameCommand(userId);

        // Act
        var result = await _sut.Handle(command, cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}