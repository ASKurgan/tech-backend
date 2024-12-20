using SachkovTech.Issues.Domain.Module.ValueObjects;

namespace SachkovTech.Issues.Domain.Module;

public interface IPositionable
{
    Position Position { get; }
    IPositionable Move(Position position);
}