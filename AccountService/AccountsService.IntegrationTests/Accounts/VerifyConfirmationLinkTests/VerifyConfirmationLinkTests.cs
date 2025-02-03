using AccountService.Contracts.Responses;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using ProjectTemplate.Application.Commands.GenerateConfirmationLink;
using ProjectTemplate.Application.Commands.VerifyConfirmationLink;
using SachkovTech.Core.Abstractions;

namespace AccountsService.IntegrationTests.Accounts.VerifyConfirmationLinkTests;

public class VerifyConfirmationLinkTests : AccountTestsBase
{
    private readonly ICommandHandler<VerifyConfirmationLinkCommand> _sut;

    public VerifyConfirmationLinkTests(IntegrationTestsWebFactory factory)
        : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<VerifyConfirmationLinkCommand>>();
    }

    [Fact]
    public async Task VerifyEmailIntegrationTest_ShouldSucceed()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;

        await SeedRoles();
        var userId = await SeedUser();

        var generateConfirmationLinkCommand = Fixture.CreateGenerateConfirmationLinkCommand(userId);
        var generateConfirmationLinkHandler = Scope.ServiceProvider
            .GetRequiredService<ICommandHandler<ConfirmationLinkResponse, GenerateConfirmationLinkCommand>>();
        var token = await generateConfirmationLinkHandler.Handle(generateConfirmationLinkCommand, cancellationToken);

        token.IsSuccess.Should().BeTrue();

        var command = Fixture.CreateVerifyConfirmationLinkCommand(userId, token.Value.ConfirmationLink);

        // Act
        var result = await _sut.Handle(command, cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var updatedUser = await UserManager.FindByIdAsync(userId.ToString());
        updatedUser?.EmailConfirmed.Should().BeTrue();
    }
}