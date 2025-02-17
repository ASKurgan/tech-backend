namespace SachkovTech.Framework.Observability;

public class ObservabilityOptions
{
    public const string OBSERVABILITY = "Observability";

    public string ServiceName { get; init; } = string.Empty;

    public string OltpEndpoint { get; init; } = string.Empty;
}