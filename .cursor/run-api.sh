#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$repo_root"

if [ ! -f src/Hosts/Naswood.Api/Naswood.Api.csproj ]; then
  echo "This revision has no API host (src/Hosts/Naswood.Api); nothing to run."
  exit 0
fi

export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_NOLOGO=1
export ASPNETCORE_ENVIRONMENT=Development
# Bind all interfaces so the port forward reaches the host, not just loopback.
export ASPNETCORE_URLS=http://0.0.0.0:5080

exec dotnet run --project src/Hosts/Naswood.Api --no-launch-profile
