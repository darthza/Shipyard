# Shipyard

Shipyard is a minimal Blazor Server app for monitoring and managing Docker or Podman containers.

## Features

- View containers
- See image, state, status, ports, and creation time
- Start stopped containers
- Stop running containers
- View recent stdout and stderr logs
- Optional dashboard auto-refresh
- Configure Docker or Podman from a persistent config file

## Tech Stack

- .NET 8
- Blazor Server
- Docker.DotNet
- Docker socket access through `/var/run/docker.sock`
- Podman API socket support
- Docker image and Compose support
- Dev container for local development

## Getting Started

### Pull From GitHub Container Registry

After the container image workflow has published the image, run Shipyard without cloning the repository:

```bash
docker pull ghcr.io/darthza/shipyard:latest

docker run --rm \
  --name shipyard \
  -p 8080:8080 \
  -v /var/run/docker.sock:/var/run/docker.sock \
  -v shipyard-config:/config \
  ghcr.io/darthza/shipyard:latest
```

### Run With Docker Compose

Build and run Shipyard:

```bash
docker compose up --build
```

Open:

```text
http://localhost:8080
```

### Run With Docker

Build the image:

```bash
docker build -t shipyard:local .
```

Run Shipyard with access to the host Docker socket:

```bash
docker run --rm \
  --name shipyard \
  -p 8080:8080 \
  -v /var/run/docker.sock:/var/run/docker.sock \
  -v shipyard-config:/config \
  shipyard:local
```

More Docker options are documented in [docs/docker.md](docs/docker.md).

### Run With Podman

Enable the Podman API socket:

```bash
systemctl --user enable --now podman.socket
```

Run Shipyard with access to the rootless Podman socket:

```bash
podman run --rm \
  --name shipyard \
  -p 8080:8080 \
  -v "$XDG_RUNTIME_DIR/podman/podman.sock:/var/run/podman/podman.sock" \
  -v shipyard-config:/config \
  --security-opt label=disable \
  ghcr.io/darthza/shipyard:latest
```

More Podman options are documented in [docs/podman.md](docs/podman.md).

### Run Locally

The recommended workflow is to open this repository in the dev container. The dev container mounts the host Docker socket so Shipyard can inspect and control local Docker containers. You can also configure Shipyard to use a Podman socket.

Run the app:

```bash
dotnet run --project src/Shipyard.Web/Shipyard.Web.csproj --urls http://0.0.0.0:8080
```

Open:

```text
http://localhost:8080
```

## Configuration

Shipyard reads optional configuration from:

```text
/config/shipyard.json
```

Example Docker configuration:

```json
{
  "ContainerEngine": {
    "Type": "docker",
    "Endpoint": "unix:///var/run/docker.sock"
  }
}
```

Example Podman configuration:

```json
{
  "ContainerEngine": {
    "Type": "podman",
    "Endpoint": "unix:///var/run/podman/podman.sock"
  }
}
```

Mount `/config` to a Docker or Podman volume so this file survives container recreation.

## Development Commands

Restore packages:

```bash
dotnet restore Shipyard.sln
```

Build:

```bash
dotnet build Shipyard.sln
```

Clean build:

```bash
dotnet clean Shipyard.sln
dotnet build Shipyard.sln
```

Build the dev container image manually:

```bash
docker build -f .devcontainer/Dockerfile -t shipyard-dev .devcontainer
```

Build inside the dev container image:

```bash
docker run --rm \
  -v "$PWD:/workspaces/shipyard" \
  -v /var/run/docker.sock:/var/run/docker.sock \
  -w /workspaces/shipyard \
  shipyard-dev \
  dotnet build Shipyard.sln
```

## Security Note

Shipyard uses the Docker socket to manage containers. Mounting `/var/run/docker.sock` gives the app broad control over the host Docker daemon. Use this setup only in trusted local development environments.

Pointing Shipyard at a Docker or Podman API gives it control over that container engine. Only configure trusted endpoints.

## Project Structure

```text
.
├── .devcontainer/
│   ├── devcontainer.json
│   └── Dockerfile
├── .github/
│   └── workflows/
│       └── container-image.yml
├── docs/
│   ├── architecture.md
│   ├── docker.md
│   └── podman.md
├── src/
│   └── Shipyard.Web/
│       ├── Components/
│       ├── Models/
│       ├── Services/
│       └── Program.cs
├── LICENSE
├── README.md
├── config/
│   ├── shipyard.example.json
│   └── shipyard.podman.example.json
├── compose.podman.yaml
├── compose.yaml
├── Dockerfile
└── Shipyard.sln
```

## License

Shipyard is licensed under the [MIT License](LICENSE).
