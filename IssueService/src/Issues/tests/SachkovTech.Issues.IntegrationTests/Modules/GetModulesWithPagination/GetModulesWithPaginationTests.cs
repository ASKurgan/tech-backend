using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SachkovTech.Core.Abstractions;
using SachkovTech.Core.Database;
using SachkovTech.Issues.Application.Features.Modules.Queries.GetModules;
using SachkovTech.Issues.Contracts.Module;

namespace SachkovTech.Issues.IntegrationTests.Modules.GetModulesWithPagination;

public class GetModulesWithPaginationTests : ModuleTestsBase
{
    private readonly IQueryHandler<PagedList<ModuleDto>, GetModulesQuery> _sut;

    public GetModulesWithPaginationTests(ModuleTestWebFactory factory)
        : base(factory)
    {
        _sut = Scope.ServiceProvider
            .GetRequiredService<IQueryHandler<PagedList<ModuleDto>, GetModulesQuery>>();
    }

    [Fact]
    public async Task GetModules_should_return_array_with_correct_number_of_items()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;

        await SeedModule();
        await SeedModule();
        await SeedModule();

        var query = new GetModulesQuery(null, 2, 2);

        // Act
        var result = await _sut.Handle(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();

        result.PageSize.Should().Be(query.PageSize);
        result.Page.Should().Be(query.Page);
        result.Items.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetModules_WithTitleFilter_should_return_array_with_correct_number_of_items()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;

        var seededModules = await SeedModules(3);

        var query = new GetModulesQuery(seededModules[0].Title.Value, 1, 2);

        // Act
        var result = await _sut.Handle(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();

        result.PageSize.Should().Be(query.PageSize);
        result.Page.Should().Be(query.Page);
        result.Items.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
    }
}