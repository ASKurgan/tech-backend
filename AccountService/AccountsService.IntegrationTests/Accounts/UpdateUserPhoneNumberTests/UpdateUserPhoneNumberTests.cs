using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using ProjectTemplate.Application.Commands.UpdateUserPhoneNumber;
using SachkovTech.Core.Abstractions;

namespace AccountsService.IntegrationTests.Accounts.UpdateUserPhoneNumberTests;

public class UpdateUserPhoneNumberTests : AccountTestsBase
{
    private readonly ICommandHandler<Guid, UpdateUserPhoneNumberCommand> _sut;

    public UpdateUserPhoneNumberTests(IntegrationTestsWebFactory factory)
        : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, UpdateUserPhoneNumberCommand>>();
    }

    [Fact]
    public async Task UpdateUserPhoneNumberIntegrationTest_ShouldSucceed()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;

        await SeedRoles();

        var userId = await SeedUser();

        var command = Fixture.CreateUpdateUserPhoneNumberCommand(userId);

        // Act
        var result = await _sut.Handle(command, cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}