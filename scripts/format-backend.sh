#!/usr/bin/env bash

set -euo pipefail

SCRIPT_DIR="$(CDPATH= cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
# shellcheck source=_compose.sh
source "$SCRIPT_DIR/_compose.sh"

api_container="$(service_container api)"
docker exec --workdir /repo "$api_container" dotnet restore api.Tests/recipeApi.Tests.csproj
docker exec --workdir /repo "$api_container" dotnet format api/recipeApi.csproj --no-restore
docker exec --workdir /repo "$api_container" dotnet format api.Tests/recipeApi.Tests.csproj --no-restore

