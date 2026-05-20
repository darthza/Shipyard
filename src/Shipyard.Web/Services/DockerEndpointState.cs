namespace Shipyard.Web.Services;

public sealed class DockerEndpointState
{
    public const string DockerEndpoint = "unix:///var/run/docker.sock";
    public const string PodmanEndpoint = "unix:///var/run/podman/podman.sock";

    public DockerEndpointState(IConfiguration configuration)
    {
        Endpoint = configuration["Docker:Endpoint"] ?? GetDefaultEndpoint();
    }

    public string Endpoint { get; private set; }

    public void SetEndpoint(string endpoint)
    {
        if (string.IsNullOrWhiteSpace(endpoint))
        {
            throw new ArgumentException("Container engine endpoint is required.", nameof(endpoint));
        }

        var trimmedEndpoint = endpoint.Trim();

        if (!Uri.TryCreate(trimmedEndpoint, UriKind.Absolute, out var uri))
        {
            throw new ArgumentException("Container engine endpoint must be an absolute URL, such as unix:///var/run/docker.sock, unix:///var/run/podman/podman.sock, or tcp://localhost:2375.", nameof(endpoint));
        }

        Endpoint = uri.ToString();
    }

    public void Reset() => Endpoint = GetDefaultEndpoint();

    public void UseDocker() => Endpoint = DockerEndpoint;

    public void UsePodman() => Endpoint = PodmanEndpoint;

    private static string GetDefaultEndpoint()
    {
        return OperatingSystem.IsWindows()
            ? "npipe://./pipe/docker_engine"
            : DockerEndpoint;
    }
}
