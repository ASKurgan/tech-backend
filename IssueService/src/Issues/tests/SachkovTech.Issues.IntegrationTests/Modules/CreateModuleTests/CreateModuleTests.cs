using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SachkovTech.Core.Abstractions;
using SachkovTech.Issues.Application.Features.Modules.Commands.Create;

namespace SachkovTech.Issues.IntegrationTests.Modules.CreateModuleTests;

public class CreateModuleTests : ModuleTestsBase
{
    private readonly ICommandHandler<Guid, CreateModuleCommand> _sut;
    public CreateModuleTests(ModuleTestWebFactory factory) : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, CreateModuleCommand>>();
    }

    [Fact]
    public async Task CreateModule_should_be_success()
    {
        // Arrange

        var cancellationToken = new CancellationTokenSource().Token;
        var command = Fixture.CreateCreateModuleCommand();

        // Act
        var result = await _sut.Handle(command, cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        var modules = await ReadDbContext.Modules
            .ToListAsync(cancellationToken);

        modules.Should().NotBeNull();
        modules.Should().HaveCount(1);
    }
}