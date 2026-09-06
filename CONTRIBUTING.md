# Contributing to Recipe App

Thank you for contributing. Keep changes focused, use synthetic data in
fixtures, and update documentation when commands, configuration, or API
behaviour changes. By contributing, you agree that your contribution may be
distributed under the [GNU AGPL v3.0](LICENSE).

## Development setup

```sh
cp .env.dev.example .env.dev
docker compose --env-file .env.dev -f compose.dev.yml up --build
```

The complete local workflow is documented in the [README](README.md).

## Before opening a pull request

- Run `./scripts/check-all.sh`.
- Add or update tests for changed behaviour.
- Add PostgreSQL changes through a new Alembic revision.
- Add Neo4j changes through a new numbered Cypher migration.
- Do not commit credentials, local environment files, generated build output,
  or real personal data.
- Explain migration, operational, and compatibility impact in the pull request.

