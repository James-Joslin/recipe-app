#!/usr/bin/env bash

set -euo pipefail

SCRIPT_DIR="$(CDPATH= cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(CDPATH= cd -- "$SCRIPT_DIR/.." && pwd)"

docker compose --project-directory "$REPO_ROOT" --env-file "$REPO_ROOT/.env.dev.example" --file "$REPO_ROOT/compose.dev.yml" config --quiet
docker compose --project-directory "$REPO_ROOT" --env-file "$REPO_ROOT/.env.prod.example" --file "$REPO_ROOT/compose.prod.yml" config --quiet

for dockerfile in \
    api/Dockerfile api/Dockerfile.dev \
    fastapi/Dockerfile fastapi/Dockerfile.dev \
    frontend/Dockerfile frontend/Dockerfile.dev \
    migrations/Dockerfile migrations/Dockerfile.dev \
    neo4j-migrations/Dockerfile neo4j-migrations/Dockerfile.dev; do
    docker build --file "$REPO_ROOT/$dockerfile" --check "$REPO_ROOT/$(dirname "$dockerfile")"
done

