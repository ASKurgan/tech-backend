using AccountService.Application.Commands.UpdateProfile;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SachkovTech.Core.Abstractions;

namespace AccountsService.IntegrationTests.Accounts.UpdateUserFullNameTests;

public class UpdateProfileTests : AccountTestsBase
{
    private readonly ICommandHandler<Guid, UpdateProfileCommand> _sut;

    public UpdateProfileTests(IntegrationTestsWebFactory factory)
        : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, UpdateProfileCommand>>();
    }

    [Fact]
    public async Task UpdateUserFullNameIntegrationTest_ShouldSucceed()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;

        await SeedRoles();

        var userId = await SeedUser();

        var command = Fixture.CreateUpdateProfileCommand(userId);

        // Act
        var result = await _sut.Handle(command, cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}