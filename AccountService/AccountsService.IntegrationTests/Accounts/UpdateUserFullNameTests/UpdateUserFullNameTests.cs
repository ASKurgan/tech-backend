using AccountService.Application.Commands.UpdateUserFullName;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SachkovTech.Core.Abstractions;

namespace AccountsService.IntegrationTests.Accounts.UpdateUserFullNameTests;

public class UpdateUserFullNameTests : AccountTestsBase
{
    private readonly ICommandHandler<Guid, UpdateUserFullNameCommand> _sut;

    public UpdateUserFullNameTests(IntegrationTestsWebFactory factory)
        : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, UpdateUserFullNameCommand>>();
    }

    [Fact]
    public async Task UpdateUserFullNameIntegrationTest_ShouldSucceed()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;

        await SeedRoles();

        var userId = await SeedUser();

        var command = Fixture.CreateUpdateUserFullNameCommand(userId);

        // Act
        var result = await _sut.Handle(command, cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}