#!/usr/bin/env bash

set -euo pipefail

SCRIPT_DIR="$(CDPATH= cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
# shellcheck source=_compose.sh
source "$SCRIPT_DIR/_compose.sh"

fastapi_container="$(service_container fastapi)"
docker exec --workdir /app "$fastapi_container" python -m compileall -q app tests
docker exec --workdir /app "$fastapi_container" python -c "from app.main import app; assert app.title == 'Recipe App Python Service'"

