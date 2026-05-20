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
  -e Docker__Endpoint=unix:///var/run/docker.sock \
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
  -e Docker__Endpoint=unix:///var/run/docker.sock \
  shipyard:local
```

## Remote Docker Endpoint

If you do not want to mount the local Docker socket, you can point Shipyard at a reachable Docker-compatible API endpoint:

```bash
docker run --rm \
  --name shipyard \
  -p 8080:8080 \
  -e Docker__Endpoint=tcp://docker-host:2375 \
  shipyard:local
```

You can also edit the endpoint from the Shipyard dashboard after the app starts.

## Security

Mounting `/var/run/docker.sock` gives Shipyard control over the host Docker daemon. A remote Docker-compatible API gives Shipyard control over that remote daemon. Only run Shipyard against endpoints you trust.
