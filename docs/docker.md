# Running Shipyard in Docker

Shipyard can run as a container. To manage Docker containers on the host, mount the host Docker socket into the Shipyard container. For Podman, see [podman.md](podman.md).

## Pull From GHCR

After the GitHub Actions container workflow has published an image, pull and run Shipyard:

```bash
docker pull ghcr.io/darthza/shipyard:latest

docker run --rm \
  --name shipyard \
  -p 8080:8080 \
  -v /var/run/docker.sock:/var/run/docker.sock \
  -v shipyard-config:/config \
  ghcr.io/darthza/shipyard:latest
```

## Docker Compose

Build and start Shipyard:

```bash
docker compose up --build
```

Open:

```text
http://localhost:8080
```

Stop it:

```bash
docker compose down
```

## Docker Run

Build the image:

```bash
docker build -t shipyard:local .
```

Run the container:

```bash
docker run --rm \
  --name shipyard \
  -p 8080:8080 \
  -v /var/run/docker.sock:/var/run/docker.sock \
  -v shipyard-config:/config \
  shipyard:local
```

## Persistent Configuration

Shipyard reads `/config/shipyard.json` when it starts. Mount `/config` to a named volume so this file survives container recreation:

```bash
docker volume create shipyard-config
```

Create or edit the config file in that volume:

```bash
docker run --rm \
  -v shipyard-config:/config \
  alpine sh -c 'cat > /config/shipyard.json <<EOF
{
  "ContainerEngine": {
    "Type": "docker",
    "Endpoint": "unix:///var/run/docker.sock"
  }
}
EOF'
```

## Remote Docker Endpoint

If you do not want to mount the local Docker socket, you can point Shipyard at a reachable Docker-compatible API endpoint:

```bash
docker run --rm \
  --name shipyard \
  -p 8080:8080 \
  -v shipyard-config:/config \
  -e Docker__Endpoint=tcp://docker-host:2375 \
  shipyard:local
```

Environment variables still work, but `/config/shipyard.json` is preferred for persistent configuration.

## Security

Mounting `/var/run/docker.sock` gives Shipyard control over the host Docker daemon. A remote Docker-compatible API gives Shipyard control over that remote daemon. Only run Shipyard against endpoints you trust.
