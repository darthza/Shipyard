namespace Shipyard.Web.Models;

public sealed record ContainerSummary(
    string Id,
    string Name,
    string Image,
    string State,
    string Status,
    DateTimeOffset Created,
    IReadOnlyList<string> Ports)
{
    public bool IsRunning => string.Equals(State, "running", StringComparison.OrdinalIgnoreCase);
}
