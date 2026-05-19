namespace Shipyard.Web.Models;

public sealed record ContainerLogOptions(int Tail = 200, bool IncludeTimestamps = true);
