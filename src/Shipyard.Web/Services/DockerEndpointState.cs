using Shipyard.Web.Configuration;

namespace Shipyard.Web.Services;

public sealed class DockerEndpointState
{
    public const string DockerEndpoint = "unix:///var/run/docker.sock";
    public const string PodmanEndpoint = "unix:///var/run/podman/podman.sock";

    public DockerEndpointState(IConfiguration configuration)
    {
        var options = configuration.GetSection("ContainerEngine").Get<ContainerEngineOptions>() ?? new ContainerEngineOptions();
        Endpoint = ResolveEndpoint(configuration, options);
    }

    public string Endpoint { get; }

    private static string ResolveEndpoint(IConfiguration configuration, ContainerEngineOptions options)
    {
        var configuredEndpoint = options.Endpoint ?? configuration["Docker:Endpoint"];

        if (!string.IsNullOrWhiteSpace(configuredEndpoint))
        {
            return ValidateEndpoint(configuredEndpoint);
        }

        return options.Type.Trim().ToLowerInvariant() switch
        {
            "podman" => PodmanEndpoint,
            "docker" => GetDefaultDockerEndpoint(),
            _ => throw new InvalidOperationException("ContainerEngine:Type must be either 'docker' or 'podman'.")
        };
    }

    private static string ValidateEndpoint(string endpoint)
    {
        var trimmedEndpoint = endpoint.Trim();

        if (!Uri.TryCreate(trimmedEndpoint, UriKind.Absolute, out var uri))
        {
            throw new ArgumentException("Container engine endpoint must be an absolute URL, such as unix:///var/run/docker.sock, unix:///var/run/podman/podman.sock, or tcp://localhost:2375.", nameof(endpoint));
        }

        return uri.ToString();
    }

    private static string GetDefaultDockerEndpoint() => OperatingSystem.IsWindows()
        ? "npipe://./pipe/docker_engine"
        : DockerEndpoint;
}
