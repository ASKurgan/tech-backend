using AccountService.Application.Commands.UpdateUserSocialNetworks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SachkovTech.Core.Abstractions;

namespace AccountsService.IntegrationTests.Accounts.UpdateUserSocialNetworksTests;

public class UpdateUserSocialNetworksTests : AccountTestsBase
{
    private readonly ICommandHandler<Guid, UpdateUserSocialNetworksCommand> _sut;

    public UpdateUserSocialNetworksTests(IntegrationTestsWebFactory factory)
        : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, UpdateUserSocialNetworksCommand>>();
    }

    [Fact]
    public async Task UpdateUserSocialNetworksIntegrationTest_ShouldSucceed()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;

        await SeedRoles();

        var userId = await SeedUser();

        var command = Fixture.CreateUpdateUserSocialNetworksCommand(userId);

        // Act
        var result = await _sut.Handle(command, cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}