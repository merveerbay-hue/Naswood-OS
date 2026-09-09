#!/usr/bin/env bash
# Per-boot runtime reconciliation: bring PostgreSQL up and make sure the role and
# database the API expects exist. Must tolerate restarts and return when ready.
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$repo_root"

log() { printf '\n=== %s\n' "$*"; }

if ! command -v pg_ctlcluster >/dev/null 2>&1; then
  log "PostgreSQL is not installed; nothing to start"
  exit 0
fi

if ! pg_isready -q -h 127.0.0.1 -p 5432; then
  cluster="$(pg_lsclusters -h | awk 'NR==1 {print $1, $2}')"
  if [ -z "$cluster" ]; then
    echo "No PostgreSQL cluster found." >&2
    exit 1
  fi
  log "Starting PostgreSQL cluster $cluster"
  # shellcheck disable=SC2086
  sudo pg_ctlcluster $cluster start || true
fi

for _ in $(seq 1 30); do
  pg_isready -q -h 127.0.0.1 -p 5432 && break
  sleep 1
done

if ! pg_isready -h 127.0.0.1 -p 5432; then
  echo "PostgreSQL did not become ready on 127.0.0.1:5432." >&2
  exit 1
fi

# The API's own appsettings is the source of truth for the local dev credentials,
# so nothing is hardcoded here. Revisions without a connection string need no database.
settings="src/Hosts/Naswood.Api/appsettings.json"
if [ ! -f "$settings" ]; then
  log "No API settings on this revision; skipping database provisioning"
  exit 0
fi

conn="$(sed -n 's/.*"Platform"[[:space:]]*:[[:space:]]*"\([^"]*\)".*/\1/p' "$settings" | head -1)"
if [ -z "$conn" ]; then
  log "No Platform connection string configured; skipping database provisioning"
  exit 0
fi

conn_field() {
  printf '%s' "$conn" | tr ';' '\n' \
    | sed -n "s/^[[:space:]]*$1[[:space:]]*=[[:space:]]*//Ip" | head -1
}

db_name="$(conn_field Database)"
db_user="$(conn_field Username)"
db_pass="$(conn_field Password)"

if [ -z "$db_name" ] || [ -z "$db_user" ] || [ -z "$db_pass" ]; then
  log "Connection string is missing Database/Username/Password; skipping provisioning"
  exit 0
fi

if ! sudo -u postgres psql -tAc "SELECT 1 FROM pg_roles WHERE rolname = '$db_user'" | grep -q 1; then
  log "Creating role $db_user"
  sudo -u postgres psql -q -c "CREATE ROLE \"$db_user\" LOGIN CREATEDB PASSWORD '$db_pass'"
fi

if ! sudo -u postgres psql -tAc "SELECT 1 FROM pg_database WHERE datname = '$db_name'" | grep -q 1; then
  log "Creating database $db_name"
  sudo -u postgres createdb -O "$db_user" "$db_name"
fi

log "PostgreSQL ready: $db_name owned by $db_user"
