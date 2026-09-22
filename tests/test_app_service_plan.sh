#!/usr/bin/env bash
set -euo pipefail

repo_root=$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)
output_path=$(mktemp -d)
trap 'rm -rf "$output_path"' EXIT

export PATH="$HOME/.dotnet:$PATH"
export DOTNET_ROOT="$HOME/.dotnet"
dotnet tool run aspire -- publish \
  --apphost "$repo_root/DemoAspire.AppHost" \
  --environment dev \
  --non-interactive \
  --output-path "$output_path" >/dev/null

grep -Fq "name: 'B1'" "$output_path/functions/functions.bicep"
grep -Fq "tier: 'Basic'" "$output_path/functions/functions.bicep"
