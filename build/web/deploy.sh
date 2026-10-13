#!/usr/bin/env bash
set -Eeuo pipefail

if [[ "$#" -ne 1 ]]; then
    echo "Usage: $0 <tag-name>" >&2
    echo "Example: $0 v1.1.2" >&2
    exit 1
fi

TAG_NAME="$1"
if [[ ! "$TAG_NAME" =~ ^[A-Za-z0-9_][A-Za-z0-9_.-]{0,127}$ ]]; then
    echo "Invalid Docker image tag: ${TAG_NAME}" >&2
    exit 1
fi

if ! command -v docker >/dev/null 2>&1; then
    echo "Docker is not installed or is not available in PATH." >&2
    exit 1
fi

REGISTRY="${REGISTRY:-deadcat.ir:8500}"
REGISTRY="${REGISTRY%/}"
FRONT_IMAGE="${REGISTRY}/behzad-sf/mohsen-survivors-gd-nginx:${TAG_NAME}"
FRONT_CONTAINER="mohsen_survivors_gd_nginx"

# Pull successfully before interrupting the running container.
echo "Pulling ${FRONT_IMAGE}..."
docker pull "$FRONT_IMAGE"

if docker container inspect "$FRONT_CONTAINER" >/dev/null 2>&1; then
    echo "Stopping ${FRONT_CONTAINER}..."
    docker stop --time 30 "$FRONT_CONTAINER"
    docker rm "$FRONT_CONTAINER"
fi

echo "Starting ${FRONT_CONTAINER} from ${FRONT_IMAGE}..."
docker run --detach \
    --name "$FRONT_CONTAINER" \
    --restart unless-stopped \
    --init \
    --stop-timeout 30 \
    --read-only \
    --tmpfs /tmp:rw,nosuid,nodev,size=32m \
    --tmpfs /var/cache/nginx:rw,nosuid,nodev,size=32m \
    --tmpfs /var/run:rw,nosuid,nodev,size=8m \
    --publish 3110:80 \
    --log-opt max-size=20m \
    --log-opt max-file=5 \
    "$FRONT_IMAGE"

echo "Started Mohsen Survivors container on port 3100."
docker ps \
    --filter "name=^/${FRONT_CONTAINER}$"
