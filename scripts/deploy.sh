#!/usr/bin/env bash
set -euo pipefail

environment=${1:?usage: scripts/deploy.sh <dev|tst> [--plan]}
mode=${2:-}

case "$environment" in
  dev|tst) ;;
  *) printf 'environment must be dev or tst\n' >&2; exit 2 ;;
esac

case "$mode" in
  '') deploy_args=() ;;
  --plan) deploy_args=(--list-steps) ;;
  *) printf 'only --plan is supported\n' >&2; exit 2 ;;
esac

: "${AZURE_SUBSCRIPTION_ID:?set AZURE_SUBSCRIPTION_ID}"
: "${AZURE_LOCATION:?set AZURE_LOCATION}"

export Azure__SubscriptionId="$AZURE_SUBSCRIPTION_ID"
export Azure__Location="$AZURE_LOCATION"
export Azure__ResourceGroup="demo-aspire-$environment"

repo_root=$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)
dotnet tool restore --tool-manifest "$repo_root/dotnet-tools.json" >/dev/null

dotnet tool run aspire deploy \
  --apphost "$repo_root/DemoAspire.AppHost" \
  --environment "$environment" \
  --non-interactive \
  "${deploy_args[@]}"
