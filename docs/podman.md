# Running Shipyard With Podman

Shipyard can manage Podman containers through Podman's Docker-compatible API socket.

## Enable The Podman API Socket

For rootless Podman:

```bash
systemctl --user enable --now podman.socket
```

The default rootless socket is:

```text
unix://$XDG_RUNTIME_DIR/podman/podman.sock
```

For rootful Podman:

```bash
sudo systemctl enable --now podman.socket
```

The default rootful socket is:

```text
unix:///run/podman/podman.sock
```

## Run Shipyard With Podman

Rootless Podman:

```bash
podman run --rm \
  --name shipyard \
  -p 8080:8080 \
  -v "$XDG_RUNTIME_DIR/podman/podman.sock:/var/run/podman/podman.sock" \
  -v shipyard-config:/config \
  --security-opt label=disable \
  ghcr.io/darthza/shipyard:latest
```

Rootful Podman:

```bash
sudo podman run --rm \
  --name shipyard \
  -p 8080:8080 \
  -v /run/podman/podman.sock:/var/run/podman/podman.sock \
  -v shipyard-config:/config \
  --security-opt label=disable \
  ghcr.io/darthza/shipyard:latest
```

Open:

```text
http://localhost:8080
```

## Run With Podman Compose

For rootless Podman:

```bash
podman compose -f compose.podman.yaml up
```

If your Podman installation does not include `podman compose`, use your installed Compose provider:

```bash
docker compose -f compose.podman.yaml up
```

## Persistent Configuration

Shipyard reads `/config/shipyard.json` when it starts. Mount `/config` to a volume so this file survives container recreation.

Rootless Podman config:

```json
{
  "ContainerEngine": {
    "Type": "podman",
    "Endpoint": "unix:///var/run/podman/podman.sock"
  }
}
```

If you mount the Podman socket somewhere else, set `ContainerEngine:Endpoint` to the matching `unix://` URL.

## Notes

- Shipyard uses Docker-compatible API calls, so Podman must have its API service enabled.
- Podman-specific concepts like pods are not shown yet; Shipyard currently lists and manages containers.
- `--security-opt label=disable` helps avoid SELinux label issues when mounting the Podman socket into the Shipyard container.
