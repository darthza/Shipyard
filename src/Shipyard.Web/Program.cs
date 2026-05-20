using Shipyard.Web.Components;
using Shipyard.Web.Services;

const string configDirectory = "/config";
const string configPath = "/config/shipyard.json";

if (Directory.Exists(configDirectory) && !File.Exists(configPath))
{
    Directory.CreateDirectory(configDirectory);
    var configuredEndpoint = Environment.GetEnvironmentVariable("Docker__Endpoint");
    var configuredType = Environment.GetEnvironmentVariable("ContainerEngine__Type");
    var engineType = configuredType ?? (configuredEndpoint?.Contains("podman", StringComparison.OrdinalIgnoreCase) == true ? "podman" : "docker");
    var endpoint = configuredEndpoint ?? (engineType.Equals("podman", StringComparison.OrdinalIgnoreCase)
        ? DockerEndpointState.PodmanEndpoint
        : DockerEndpointState.DockerEndpoint);

    File.WriteAllText(
        configPath,
        $$"""
        {
          "ContainerEngine": {
            "Type": "{{engineType}}",
            "Endpoint": "{{endpoint}}"
          }
        }
        """);
}

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile(configPath, optional: true, reloadOnChange: true);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddScoped<DockerEndpointState>();
builder.Services.AddScoped<IDockerContainerService, DockerContainerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
