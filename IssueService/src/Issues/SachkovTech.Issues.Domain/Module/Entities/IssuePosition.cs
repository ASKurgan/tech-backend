using CSharpFunctionalExtensions;
using SachkovTech.Issues.Domain.Module.ValueObjects;
using SachkovTech.Issues.Domain.ValueObjects.Ids;

namespace SachkovTech.Issues.Domain.Module.Entities;

public class IssuePosition : Entity<IssuePositionId>, IPositionable
{
    // ef core
    private IssuePosition()
    {
    }

    public IssuePosition(IssuePositionId id, IssueId issueId, Position position)
        : base(id)
    {
        IssueId = issueId;
        Position = position;
    }

    public IssueId IssueId { get; private set; }

    public Position Position { get; private set; }

    public void SetPosition(Position newPosition)
    {
        Position = newPosition;
    }
}