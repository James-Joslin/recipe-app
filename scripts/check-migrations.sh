#!/usr/bin/env bash

set -euo pipefail

SCRIPT_DIR="$(CDPATH= cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
# shellcheck source=_compose.sh
source "$SCRIPT_DIR/_compose.sh"

db_container="$(service_container db)"
neo4j_container="$(service_container neo4j)"

compose run --rm --no-deps migrations current
compose run --rm --no-deps migrations heads
compose run --rm --no-deps neo4j-migrations

docker exec "$db_container" psql \
    --username "$(docker exec "$db_container" printenv POSTGRES_USER)" \
    --dbname "$(docker exec "$db_container" printenv POSTGRES_DB)" \
    --tuples-only --no-align \
    --command "SELECT version_num FROM alembic_version;"

