from pathlib import Path
import os

from neo4j import GraphDatabase


MIGRATIONS_DIR = Path(__file__).parent / "migrations"


def required(name: str) -> str:
    value = os.getenv(name)
    if not value:
        raise RuntimeError(f"Required environment variable {name} is not set.")
    return value


def migration_files() -> list[Path]:
    return sorted(MIGRATIONS_DIR.glob("V*.cypher"))


def apply_migration(session, name: str, cypher: str) -> bool:
    already_applied = session.run(
        "MATCH (m:SchemaMigration {name: $name}) RETURN m LIMIT 1", name=name
    ).single()
    if already_applied:
        return False

    session.run(cypher).consume()
    session.run(
        "CREATE (:SchemaMigration {name: $name, appliedAt: datetime()})", name=name
    ).consume()
    return True


def main() -> None:
    uri = f"bolt://{required('NEO4J_HOST')}:{os.getenv('NEO4J_BOLT_PORT', '7687')}"
    driver = GraphDatabase.driver(
        uri,
        auth=(required("NEO4J_USER"), required("NEO4J_PASSWORD")),
    )
    try:
        with driver.session() as session:
            for path in migration_files():
                cypher = path.read_text(encoding="utf-8").strip()
                if not cypher:
                    continue
                changed = apply_migration(session, path.name, cypher)
                print(f"{'Applied' if changed else 'Skipped'} {path.name}", flush=True)
    finally:
        driver.close()


if __name__ == "__main__":
    main()

