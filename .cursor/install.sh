#!/usr/bin/env bash
# Idempotent dependency refresh for Cloud Agent VMs.
#
# The repository grows one layer at a time (docs -> backend -> frontend), so every
# step is guarded: a revision without a frontend or without the .NET solution must
# still install successfully.
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$repo_root"

export DEBIAN_FRONTEND=noninteractive
export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_NOLOGO=1

log() { printf '\n=== %s\n' "$*"; }

if ! command -v pg_ctlcluster >/dev/null 2>&1; then
  log "Installing PostgreSQL"
  sudo apt-get update -qq
  sudo apt-get install -y -qq postgresql
fi

# Pinned rather than floating, and deliberately not Ubuntu's dotnet-sdk-8.0: the
# 8.0.1xx compiler reports CS0121 for the collection-expression call in
# Platform.Application/Users/UserSupport.cs that later 8.0.4xx SDKs resolve.
dotnet_sdk_version="8.0.425"
dotnet_install_dir="/usr/local/dotnet"

if ! dotnet --list-sdks 2>/dev/null | grep -q "^${dotnet_sdk_version} "; then
  log "Installing .NET SDK ${dotnet_sdk_version}"
  tmp_dir="$(mktemp -d)"
  curl -fsSL https://dot.net/v1/dotnet-install.sh -o "$tmp_dir/dotnet-install.sh"
  chmod +x "$tmp_dir/dotnet-install.sh"
  sudo "$tmp_dir/dotnet-install.sh" \
    --version "$dotnet_sdk_version" \
    --install-dir "$dotnet_install_dir" \
    --no-path
  sudo ln -sf "$dotnet_install_dir/dotnet" /usr/local/bin/dotnet
  rm -rf "$tmp_dir"
fi

log "Toolchain"
dotnet --version
node --version
pnpm --version

if [ -f pnpm-lock.yaml ]; then
  log "pnpm install"
  pnpm install --frozen-lockfile

  # pnpm 10 refuses to run dependency build scripts unless they are allowlisted,
  # which leaves esbuild's native binary without the execute bit and makes vite
  # fail to boot. Restore it here instead of relying on interactive approval.
  pnpm rebuild esbuild >/dev/null 2>&1 || true
  find node_modules/.pnpm -maxdepth 6 -path '*esbuild*/bin/esbuild' -type f \
    -exec chmod +x {} + 2>/dev/null || true
fi

if [ -f src/Naswood.OS.sln ]; then
  log "dotnet restore"
  dotnet restore src/Naswood.OS.sln
  log "dotnet build"
  dotnet build src/Naswood.OS.sln --no-restore
fi

log "Install complete"
