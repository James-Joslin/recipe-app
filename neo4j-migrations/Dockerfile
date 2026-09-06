FROM python:3.13-slim
ENV PYTHONDONTWRITEBYTECODE=1 PYTHONUNBUFFERED=1
WORKDIR /migrations
COPY requirements.txt ./
RUN pip install --no-cache-dir --requirement requirements.txt
COPY migrate.py ./
COPY migrations ./migrations
RUN useradd --create-home --uid 10001 migrations
USER migrations
ENTRYPOINT ["python", "migrate.py"]

