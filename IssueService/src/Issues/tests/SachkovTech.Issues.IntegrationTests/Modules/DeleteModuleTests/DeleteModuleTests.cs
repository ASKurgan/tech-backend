using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SachkovTech.Core.Abstractions;
using SachkovTech.Issues.Application.Features.Modules.Commands.Delete;

namespace SachkovTech.Issues.IntegrationTests.Modules.DeleteModuleTests;

public class DeleteModuleTests: ModuleTestsBase
{
    private readonly ICommandHandler<Guid, DeleteModuleCommand> _sut;
    public DeleteModuleTests(ModuleTestWebFactory factory) : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, DeleteModuleCommand>>();
    }
    
    [Fact]
    public async Task DeleteModule_should_be_success()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;
        
        var moduleId = await SeedModule();
        var command = Fixture.CreateDeleteModuleCommand(moduleId);

        // Act
        var result = await _sut.Handle(command, cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        var modules = await ReadDbContext.Modules
            .ToListAsync(cancellationToken);
        
        modules.Should().BeNullOrEmpty();
    }
}