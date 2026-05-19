using Shipyard.Web.Models;

namespace Shipyard.Web.Services;

public interface IDockerContainerService
{
    Task<IReadOnlyList<ContainerSummary>> GetContainersAsync(CancellationToken cancellationToken = default);

    Task StartContainerAsync(string containerId, CancellationToken cancellationToken = default);

    Task StopContainerAsync(string containerId, CancellationToken cancellationToken = default);

    Task<string> GetLogsAsync(
        string containerId,
        ContainerLogOptions options,
        CancellationToken cancellationToken = default);
}
