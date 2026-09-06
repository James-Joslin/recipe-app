#!/usr/bin/env bash

set -euo pipefail

SCRIPT_DIR="$(CDPATH= cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
# shellcheck source=_compose.sh
source "$SCRIPT_DIR/_compose.sh"

api_container="$(service_container api)"
docker exec --workdir /repo "$api_container" dotnet restore api.Tests/recipeApi.Tests.csproj
docker exec --workdir /repo "$api_container" dotnet build api.Tests/recipeApi.Tests.csproj --configuration Release --no-restore --warnaserror
docker exec --workdir /repo "$api_container" dotnet test api.Tests/recipeApi.Tests.csproj --configuration Release --no-restore --verbosity normal

