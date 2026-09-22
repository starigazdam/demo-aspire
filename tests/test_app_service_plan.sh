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

grep -RFq "Microsoft.App/managedEnvironments" "$output_path"
! grep -RFq "Microsoft.Web/serverfarms" "$output_path"
