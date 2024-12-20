using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;

namespace SachkovTech.ArchitectureTests;

public class ArchitectureTests : ArchUnitBaseTest
{
    [Fact]
    public void PresentationLayer_Should_NotHaveDependenciesOnInfrastructure()
    {
        ArchRuleDefinition
            .Types()
            .That()
            .Are(PresentationLayer)
            .Should()
            .NotDependOnAny(InfrastructureLayer)
            .Check(Architecture);
    }
    
    [Fact]
    public void ApplicationLayer_Should_NotHaveDependenciesOnOtherLayers()
    {
        ArchRuleDefinition
            .Types()
            .That()
            .Are(ApplicationLayer)
            .Should()
            .NotDependOnAny(PresentationLayer)
            .AndShould()
            .NotDependOnAny(InfrastructureLayer)
            .Check(Architecture);
    }

    [Fact]
    public void DomainLayer_Should_NotHaveDependenciesOnOtherLayers()
    {
        ArchRuleDefinition
            .Types()
            .That()
            .Are(DomainLayer)
            .Should()
            .NotDependOnAny(PresentationLayer)
            .AndShould()
            .NotDependOnAny(ApplicationLayer)
            .AndShould()
            .NotDependOnAny(InfrastructureLayer)
            .Check(Architecture);
    }

    [Fact]
    public void InfrastructureLayer_Should_NotHaveDependenciesOnPresentation()
    {
        ArchRuleDefinition
            .Types()
            .That()
            .Are(InfrastructureLayer)
            .Should()
            .NotDependOnAny(PresentationLayer)
            .Check(Architecture);
    }
}