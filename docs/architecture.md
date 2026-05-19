# Architecture

Shipyard is intentionally small. The app is a Blazor Server application that talks to the local Docker daemon through `Docker.DotNet`.

## Runtime Flow

```text
Browser
  -> Blazor Server UI
  -> IDockerContainerService
  -> Docker.DotNet
  -> Docker daemon socket
```

## Main Components

- `Components/Pages/Home.razor`
  - Lists containers.
  - Runs start and stop actions.
  - Supports manual refresh and optional auto-refresh.
  - Lets the user edit the active Docker endpoint for the current Blazor session.

- `Components/Pages/ContainerLogs.razor`
  - Fetches recent logs for a selected container.
  - Supports tail count and timestamp options.

- `Services/IDockerContainerService.cs`
  - Defines the Docker operations used by the UI.

- `Services/DockerContainerService.cs`
  - Implements container listing, start, stop, and log reads through `Docker.DotNet`.

- `Services/DockerEndpointState.cs`
  - Holds the active Docker endpoint for the current Blazor Server circuit.

- `Models/ContainerSummary.cs`
  - Holds the container data shown by the dashboard.

- `Models/ContainerLogOptions.cs`
  - Holds log retrieval options.

## Docker Access

By default, the service connects to:

```text
unix:///var/run/docker.sock
```

On Windows, it falls back to:

```text
npipe://./pipe/docker_engine
```

The endpoint can be overridden with configuration:

```json
{
  "Docker": {
    "Endpoint": "unix:///var/run/docker.sock"
  }
}
```

It can also be changed at runtime from the dashboard. The selected value is scoped to the current Blazor Server session.

## Dev Container

The dev container installs the .NET SDK and Docker CLI, then mounts the host Docker socket:

```json
"mounts": [
  "source=/var/run/docker.sock,target=/var/run/docker.sock,type=bind"
]
```

This keeps local development simple, but it means code running in the dev container can control the host Docker daemon.
