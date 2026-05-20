namespace Shipyard.Web.Configuration;

public sealed class ContainerEngineOptions
{
    public string Type { get; set; } = "docker";

    public string? Endpoint { get; set; }
}
