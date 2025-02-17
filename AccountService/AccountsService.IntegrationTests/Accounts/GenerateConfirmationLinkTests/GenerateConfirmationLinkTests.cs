using AccountService.Application.Commands.GenerateConfirmationLink;
using AccountService.Contracts.Responses;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SachkovTech.Core.Abstractions;

namespace AccountsService.IntegrationTests.Accounts.GenerateConfirmationLinkTests;

public class GenerateConfirmationLinkTests : AccountTestsBase
{
    private readonly ICommandHandler<ConfirmationLinkResponse, GenerateConfirmationLinkCommand> _sut;

    public GenerateConfirmationLinkTests(IntegrationTestsWebFactory factory)
        : base(factory)
    {
        _sut = Scope.ServiceProvider
            .GetRequiredService<ICommandHandler<ConfirmationLinkResponse, GenerateConfirmationLinkCommand>>();
    }

    [Fact]
    public async Task GenerateConfirmationLinkTest_ShouldSucceed()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;

        await SeedRoles();
        var userId = await SeedUser();

        var command = Fixture.CreateGenerateConfirmationLinkCommand(userId);

        // Act
        var result = await _sut.Handle(command, cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.ConfirmationLink.Should().NotBeNullOrEmpty();
    }
}