#!/usr/bin/env bash

set -euo pipefail

# Resolve the build context relative to this script, regardless of the caller's cwd.
SCRIPT_DIR=$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)
cd "$SCRIPT_DIR"

if ! command -v docker >/dev/null 2>&1; then
    echo "Error: Docker is not installed or is not available in PATH." >&2
    exit 1
fi

if ! docker buildx version >/dev/null 2>&1; then
    echo "Error: Docker Buildx is not available." >&2
    exit 1
fi

if [ "$#" -gt 1 ]; then
    echo "Usage: $0 [image-tag]" >&2
    exit 1
fi

if [ "$#" -eq 1 ]; then
    IMAGE_TAG=$1
elif ! IMAGE_TAG=$(git rev-parse --short=12 HEAD 2>/dev/null); then
    echo "Error: unable to determine an image tag from Git." >&2
    echo "Pass one explicitly: $0 <image-tag>" >&2
    exit 1
fi

if [[ ! "$IMAGE_TAG" =~ ^[a-zA-Z0-9_][a-zA-Z0-9_.-]{0,127}$ ]]; then
    echo "Error: invalid Docker image tag: $IMAGE_TAG" >&2
    exit 1
fi

for BUILD_FILE in Dockerfile index.html index.js index.wasm index.pck; do
    if [ ! -s "$BUILD_FILE" ]; then
        echo "Error: missing or empty $SCRIPT_DIR/$BUILD_FILE" >&2
        echo "Export the Godot Web build to this directory before publishing." >&2
        exit 1
    fi
done

REGISTRY="${REGISTRY:-deadcat.ir:8500}"
REGISTRY="${REGISTRY%/}"
IMAGE_NAME="behzad-sf/mohsen-survivors-gd-nginx"
IMAGE_REF="$REGISTRY/$IMAGE_NAME:$IMAGE_TAG"
PUBLISH_PLATFORM="${TARGET_PLATFORM:-linux/amd64}"

REGISTRY_OUTPUT="type=registry"
if [ "${REGISTRY_INSECURE:-true}" = "true" ]; then
    REGISTRY_OUTPUT="$REGISTRY_OUTPUT,registry.insecure=true"
fi

# Match the reference publisher's direct Buildx registry output, including
# explicit HTTP-registry support. This single image does not need Compose.
echo "Building and publishing $IMAGE_REF for $PUBLISH_PLATFORM..."
docker buildx build \
    --file Dockerfile \
    --platform "$PUBLISH_PLATFORM" \
    --tag "$IMAGE_REF" \
    --output "$REGISTRY_OUTPUT" \
    --provenance=false \
    --progress=plain \
    .

echo "Published image: $IMAGE_REF"
