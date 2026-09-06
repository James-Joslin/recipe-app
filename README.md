# Recipe App

Recipe App is a barebones full-stack starter for managing recipes. It is
intentionally small: the scaffold demonstrates the service boundaries and
database workflow, leaving domain features to be built on top.

## Stack

- Next.js frontend
- ASP.NET Core 8 C# API
- FastAPI Python service
- PostgreSQL 17 with Alembic migrations
- Neo4j 5 with an independent Cypher migration runner
- Docker Compose development and production stacks

## Getting started

Install Docker with the Compose plugin. No host installation of .NET, Node.js,
Python, PostgreSQL, or Neo4j tooling is required.

### Development

```sh
cp .env.dev.example .env.dev
docker compose --env-file .env.dev -f compose.dev.yml up --build
```

Services are available at:

| Service | Address |
| --- | --- |
| Next.js | <http://localhost:3000> |
| ASP.NET Core API | <http://localhost:5154> |
| FastAPI | <http://localhost:8001/docs> |
| PostgreSQL | `localhost:55433` |
| Neo4j browser | <http://localhost:7475> |
| Neo4j Bolt | `bolt://localhost:7688` |

The API exposes liveness at `/status/live`, readiness at `/status/ready`, and
Swagger at `/swagger`. The Python service exposes `/health` and `/docs`.

Stop the stack and retain data:

```sh
docker compose --env-file .env.dev -f compose.dev.yml down
```

Remove development databases too:

```sh
docker compose --env-file .env.dev -f compose.dev.yml down --volumes
```

### Production

Copy the production environment template, set both database passwords, and
review the image tags:

```sh
cp .env.prod.example .env.prod
chmod 600 .env.prod
docker compose --env-file .env.prod -f compose.prod.yml up --build --detach
```

Only the Next.js application is published by the production compose file.
PostgreSQL, Neo4j, and both APIs stay on the private Compose network.

## Database migrations

PostgreSQL migrations are run by the one-shot `migrations` service before the
APIs start:

```sh
docker compose --env-file .env.dev -f compose.dev.yml run --rm migrations current
docker compose --env-file .env.dev -f compose.dev.yml run --rm migrations revision -m "describe the schema change"
docker compose --env-file .env.dev -f compose.dev.yml run --rm migrations upgrade head
```

Neo4j uses its own ordered migration files under
`neo4j-migrations/migrations/`. Each file contains one Cypher statement and is
recorded in the `SchemaMigration` label. The one-shot service applies pending
migrations on startup:

```sh
docker compose --env-file .env.dev -f compose.dev.yml run --rm neo4j-migrations
```

Name new files with the next sequence, for example
`V002__add_recipe_ingredient_relationship.cypher`. Neo4j migrations are not
Alembic revisions and are intentionally tracked independently.

## Workflow scripts

```sh
./scripts/check-all.sh       # compose validation, syntax, and source checks
./scripts/format-all.sh      # dotnet format and Prettier in dev containers
./scripts/check-migrations.sh
./scripts/check-images.sh
```

Set `RECIPE_ENV_FILE` to use an environment file other than `.env.dev` for
development scripts. The scripts expect the relevant Compose services to be
running unless they explicitly start a disposable check.

## Project layout

```text
api/                 ASP.NET Core API and production/dev Dockerfiles
api.Tests/            C# API test project
fastapi/              Python service and production/dev Dockerfiles
frontend/             Next.js app and production/dev Dockerfiles
migrations/           PostgreSQL Alembic environment and revisions
neo4j-migrations/     Independent Neo4j migration runner and Cypher files
scripts/              Local workflow and validation commands
compose.dev.yml       Hot-reload development stack
compose.prod.yml      Non-bind-mounted production stack
```

## GitHub automation

Pull requests and pushes to `main` run the CI workflow in `.github/workflows/ci.yml`. It checks the C# API, FastAPI service, Next.js frontend, PostgreSQL and Neo4j migrations, production image builds, Compose configuration, Semgrep, and CodeQL. Dependabot monitors the Dockerfiles, npm dependencies, Python dependencies, and GitHub Actions.

A GHCR publishing workflow is intentionally not included until a registry-publishing policy and authorization are defined.


## License and conduct

Recipe App is licensed under the [GNU AGPL v3.0](LICENSE). Please read the
[contribution guide](CONTRIBUTING.md), [security policy](SECURITY.md), and
[Code of Conduct](CODE_OF_CONDUCT.md) before contributing.

