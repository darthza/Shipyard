namespace Shipyard.Web.Models;

public sealed record ContainerGroup(
    string Name,
    string Type,
    IReadOnlyList<ContainerSummary> Containers)
{
    public int RunningCount => Containers.Count(container => container.IsRunning);

    public int StoppedCount => Containers.Count - RunningCount;

    public DateTimeOffset Created => Containers.Max(container => container.Created);
}
