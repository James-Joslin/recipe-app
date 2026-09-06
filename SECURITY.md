# Security Policy

## Reporting a vulnerability

Do not disclose vulnerabilities in a public issue, discussion, pull request,
or CI log. Contact the project maintainers privately with the affected
component, impact, reproduction steps using synthetic data, and any suggested
mitigation.

Never include credentials, connection strings, database dumps, or private
recipe content in a report.

## Deployment boundary

The starter stack is intended to run behind a trusted network or an
authenticated TLS reverse proxy. Production deployments must change all
example passwords, avoid publishing databases directly, and review the
container images and secrets before exposure to the internet.

