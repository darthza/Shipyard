# Shipyard

Shipyard is a minimal Blazor Server app for monitoring and managing local Docker containers.

## Features

- View all Docker containers
- See image, state, status, ports, and creation time
- Start stopped containers
- Stop running containers
- View recent stdout and stderr logs
- Optional dashboard auto-refresh
- Edit the Docker endpoint URL from the dashboard

## Tech Stack

- .NET 8
- Blazor Server
- Docker.DotNet
- Docker socket access through `/var/run/docker.sock`
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
  -e Docker__Endpoint=unix:///var/run/docker.sock \
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
  -e Docker__Endpoint=unix:///var/run/docker.sock \
  shipyard:local
```

More Docker options are documented in [docs/docker.md](docs/docker.md).

### Run Locally

The recommended workflow is to open this repository in the dev container. The dev container mounts the host Docker socket so Shipyard can inspect and control local Docker containers.

Run the app:

```bash
dotnet run --project src/Shipyard.Web/Shipyard.Web.csproj --urls http://0.0.0.0:8080
```

Open:

```text
http://localhost:8080
```

## Docker Endpoint

Shipyard uses the local Docker socket by default:

```text
unix:///var/run/docker.sock
```

You can change the endpoint from the dashboard. Examples:

```text
unix:///var/run/docker.sock
tcp://server-name:2375
http://server-name:2375
```

Remote Docker APIs must be reachable from the machine or dev container running Shipyard.

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

Pointing Shipyard at a remote Docker API gives it control over that remote daemon. Only connect to trusted Docker endpoints.

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
│   └── docker.md
├── src/
│   └── Shipyard.Web/
│       ├── Components/
│       ├── Models/
│       ├── Services/
│       └── Program.cs
├── LICENSE
├── README.md
├── compose.yaml
├── Dockerfile
└── Shipyard.sln
```

## License

Shipyard is licensed under the [MIT License](LICENSE).
