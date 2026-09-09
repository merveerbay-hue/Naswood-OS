#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$repo_root"

if [ ! -f apps/web/package.json ]; then
  echo "This revision has no frontend (apps/web); nothing to run."
  exit 0
fi

exec pnpm --filter @naswood/web dev
