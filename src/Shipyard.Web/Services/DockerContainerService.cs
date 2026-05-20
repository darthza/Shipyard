using Shipyard.Web.Models;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace Shipyard.Web.Services;

public sealed class DockerContainerService : IDockerContainerService
{
    private readonly DockerEndpointState _dockerEndpoint;

    public DockerContainerService(DockerEndpointState dockerEndpoint)
    {
        _dockerEndpoint = dockerEndpoint;
    }

    public async Task<IReadOnlyList<ContainerSummary>> GetContainersAsync(CancellationToken cancellationToken = default)
    {
        using var client = CreateClient();
        var containers = await client.Containers.ListContainersAsync(
            new ContainersListParameters { All = true },
            cancellationToken);

        return containers
            .OrderByDescending(container => container.Created)
            .Select(container => new ContainerSummary(
                container.ID,
                CleanName(container.Names.FirstOrDefault() ?? container.ID[..12]),
                container.Image,
                container.State,
                container.Status,
                new DateTimeOffset(DateTime.SpecifyKind(container.Created, DateTimeKind.Utc)),
                FormatPorts(container.Ports),
                GetGroupName(container.Labels),
                GetGroupType(container.Labels)))
            .ToList();
    }

    public async Task StartContainerAsync(string containerId, CancellationToken cancellationToken = default)
    {
        using var client = CreateClient();
        var started = await client.Containers.StartContainerAsync(
            containerId,
            new ContainerStartParameters(),
            cancellationToken);

        if (!started)
        {
            throw new InvalidOperationException("Docker did not start the container.");
        }
    }

    public async Task StopContainerAsync(string containerId, CancellationToken cancellationToken = default)
    {
        using var client = CreateClient();
        var stopped = await client.Containers.StopContainerAsync(
            containerId,
            new ContainerStopParameters { WaitBeforeKillSeconds = 10 },
            cancellationToken);

        if (!stopped)
        {
            throw new InvalidOperationException("Docker did not stop the container.");
        }
    }

    public async Task<string> GetLogsAsync(
        string containerId,
        ContainerLogOptions options,
        CancellationToken cancellationToken = default)
    {
        using var client = CreateClient();
        using var stream = await client.Containers.GetContainerLogsAsync(
            containerId,
            tty: false,
            new ContainerLogsParameters
            {
                ShowStdout = true,
                ShowStderr = true,
                Timestamps = options.IncludeTimestamps,
                Tail = options.Tail.ToString()
            },
            cancellationToken);

        var (stdout, stderr) = await stream.ReadOutputToEndAsync(cancellationToken);
        return string.Join(Environment.NewLine, new[] { stdout, stderr }.Where(value => !string.IsNullOrWhiteSpace(value)));
    }

    private DockerClient CreateClient() => new DockerClientConfiguration(new Uri(_dockerEndpoint.Endpoint)).CreateClient();

    private static string CleanName(string name) => name.TrimStart('/');

    private static string? GetGroupName(IDictionary<string, string> labels)
    {
        if (labels.TryGetValue("com.docker.compose.project", out var composeProject))
        {
            return composeProject;
        }

        if (labels.TryGetValue("com.docker.stack.namespace", out var stackNamespace))
        {
            return stackNamespace;
        }

        if (labels.TryGetValue("io.podman.compose.project", out var podmanComposeProject))
        {
            return podmanComposeProject;
        }

        return null;
    }

    private static string? GetGroupType(IDictionary<string, string> labels)
    {
        if (labels.ContainsKey("com.docker.compose.project"))
        {
            return "Compose";
        }

        if (labels.ContainsKey("com.docker.stack.namespace"))
        {
            return "Stack";
        }

        if (labels.ContainsKey("io.podman.compose.project"))
        {
            return "Podman Compose";
        }

        return null;
    }

    private static IReadOnlyList<string> FormatPorts(IList<Port> ports)
    {
        return ports
            .OrderBy(port => port.PrivatePort)
            .Select(port =>
            {
                var protocol = string.IsNullOrWhiteSpace(port.Type) ? "tcp" : port.Type;
                return port.PublicPort > 0
                    ? $"{port.IP}:{port.PublicPort}->{port.PrivatePort}/{protocol}"
                    : $"{port.PrivatePort}/{protocol}";
            })
            .ToList();
    }
}
