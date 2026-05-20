# Architecture

Shipyard is intentionally small. The app is a Blazor Server application that talks to a Docker-compatible container engine API through `Docker.DotNet`.

## Runtime Flow

```text
Browser
  -> Blazor Server UI
  -> IDockerContainerService
  -> Docker.DotNet
  -> Docker or Podman API socket
```

## Main Components

- `Components/Pages/Home.razor`
  - Lists containers.
  - Runs start and stop actions.
  - Supports manual refresh and optional auto-refresh.
  - Lets the user edit the active container engine endpoint for the current Blazor session.

- `Components/Pages/ContainerLogs.razor`
  - Fetches recent logs for a selected container.
  - Supports tail count and timestamp options.

- `Services/IDockerContainerService.cs`
  - Defines the Docker operations used by the UI.

- `Services/DockerContainerService.cs`
  - Implements container listing, start, stop, and log reads through `Docker.DotNet`.

- `Services/DockerEndpointState.cs`
  - Holds the active container engine endpoint for the current Blazor Server circuit.

- `Models/ContainerSummary.cs`
  - Holds the container data shown by the dashboard.

- `Models/ContainerLogOptions.cs`
  - Holds log retrieval options.

## Docker Access

By default, the service connects to:

```text
unix:///var/run/docker.sock
```

The Podman dashboard preset uses:

```text
unix:///var/run/podman/podman.sock
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

The dev container is for contributors working on the codebase. It is separate from the production `Dockerfile` used to run Shipyard as an app container.

The dev container installs the .NET SDK and Docker CLI, then mounts the host Docker socket:

```json
"mounts": [
  "source=/var/run/docker.sock,target=/var/run/docker.sock,type=bind"
]
```

This keeps local development simple, but it means code running in the dev container can control the host Docker daemon.

## App Container

The root `Dockerfile` builds and publishes the Blazor app with the .NET SDK image, then runs it with the ASP.NET runtime image.

The `compose.yaml` file runs Shipyard on port `8080` and mounts:

```text
/var/run/docker.sock:/var/run/docker.sock
```

This lets Shipyard inspect and control containers managed by the host Docker daemon.

The `compose.podman.yaml` file mounts a rootless Podman socket to:

```text
/var/run/podman/podman.sock
```

This lets Shipyard inspect and control containers managed by Podman through Podman's Docker-compatible API.

## Published Image

The `.github/workflows/container-image.yml` workflow builds the root `Dockerfile` and publishes images to GitHub Container Registry:

```text
ghcr.io/darthza/shipyard
```

Pushes to `main` publish `latest`, branch, and SHA tags. Version tags like `v1.0.0` publish matching image tags.
