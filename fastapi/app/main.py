import os
from typing import Any

import psycopg
from fastapi import FastAPI

app = FastAPI(title="Recipe App Python Service", version="0.1.0")


def postgres_connection_string() -> str:
    return (
        f"host={os.environ['POSTGRES_HOST']} "
        f"port={os.getenv('POSTGRES_PORT', '5432')} "
        f"dbname={os.environ['POSTGRES_DB']} "
        f"user={os.environ['POSTGRES_USER']} "
        f"password={os.environ['POSTGRES_PASSWORD']}"
    )


@app.get("/")
def root() -> dict[str, str]:
    return {"service": "recipe-app-fastapi", "status": "ok"}


@app.get("/health")
def health() -> dict[str, str]:
    return {"status": "ok"}


@app.get("/ready")
def ready() -> dict[str, Any]:
    with psycopg.connect(postgres_connection_string()) as connection:
        with connection.cursor() as cursor:
            cursor.execute("SELECT 1")
            cursor.fetchone()
    return {"status": "ready", "database": "ready"}


@app.get("/api/recipes/count")
def recipe_count() -> dict[str, int]:
    with psycopg.connect(postgres_connection_string()) as connection:
        with connection.cursor() as cursor:
            cursor.execute("SELECT count(*) FROM recipes")
            count = cursor.fetchone()[0]
    return {"count": count}

