using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SachkovTech.Core.Abstractions;
using SachkovTech.Issues.Application.Features.Modules.Commands.UpdateMainInfo;

namespace SachkovTech.Issues.IntegrationTests.Modules.UpdateMainInfoTests;

public class UpdateMainInfoTests : ModuleTestsBase
{
    private readonly ICommandHandler<Guid, UpdateMainInfoCommand> _sut;

    public UpdateMainInfoTests(ModuleTestWebFactory factory)
        : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, UpdateMainInfoCommand>>();
    }

    [Fact]
    public async Task UpdateMainInfo_should_succeed()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;

        var moduleId = await SeedModule();
        var command = Fixture.CreateUpdateMainInfoCommand(moduleId);

        // Act
        var result = await _sut.Handle(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        var module = await ReadDbContext.ReadModules
            .FirstOrDefaultAsync(x => x.Id == moduleId, cancellationToken);

        module.Should().NotBeNull();

        module?.Title.Value.Should().Be(command.Title);
        module?.Description.Value.Should().Be(command.Description);
    }
}