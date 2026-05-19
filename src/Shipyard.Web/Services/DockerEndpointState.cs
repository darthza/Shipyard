namespace Shipyard.Web.Services;

public sealed class DockerEndpointState
{
    public DockerEndpointState(IConfiguration configuration)
    {
        Endpoint = configuration["Docker:Endpoint"] ?? GetDefaultEndpoint();
    }

    public string Endpoint { get; private set; }

    public void SetEndpoint(string endpoint)
    {
        if (string.IsNullOrWhiteSpace(endpoint))
        {
            throw new ArgumentException("Docker endpoint is required.", nameof(endpoint));
        }

        var trimmedEndpoint = endpoint.Trim();

        if (!Uri.TryCreate(trimmedEndpoint, UriKind.Absolute, out var uri))
        {
            throw new ArgumentException("Docker endpoint must be an absolute URL, such as unix:///var/run/docker.sock or tcp://localhost:2375.", nameof(endpoint));
        }

        Endpoint = uri.ToString();
    }

    public void Reset() => Endpoint = GetDefaultEndpoint();

    private static string GetDefaultEndpoint()
    {
        return OperatingSystem.IsWindows()
            ? "npipe://./pipe/docker_engine"
            : "unix:///var/run/docker.sock";
    }
}
