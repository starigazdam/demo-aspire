#!/usr/bin/env bash
set -euo pipefail

repo_root=$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)
tmpdir=$(mktemp -d)
trap 'rm -rf "$tmpdir"' EXIT

printf '%s\n' \
  '#!/usr/bin/env bash' \
  'printf "subscription=%s\\nlocation=%s\\nresource_group=%s\\n" "$Azure__SubscriptionId" "$Azure__Location" "$Azure__ResourceGroup" >>"$DESTROY_TEST_LOG"' \
  'printf "args=%s\\n" "$*" >>"$DESTROY_TEST_LOG"' >"$tmpdir/dotnet"
chmod +x "$tmpdir/dotnet"

export DESTROY_TEST_LOG="$tmpdir/log"
export AZURE_SUBSCRIPTION_ID=test-subscription
export AZURE_LOCATION=uksouth
PATH="$tmpdir:$PATH" "$repo_root/scripts/destroy.sh" dev

grep -Fqx 'subscription=test-subscription' "$DESTROY_TEST_LOG"
grep -Fqx 'location=uksouth' "$DESTROY_TEST_LOG"
grep -Fqx 'resource_group=demo-aspire-dev' "$DESTROY_TEST_LOG"
grep -F 'args=tool run aspire destroy ' "$DESTROY_TEST_LOG" >/dev/null
grep -F -- "--apphost $repo_root/DemoAspire.AppHost --environment dev --non-interactive --yes" "$DESTROY_TEST_LOG" >/dev/null
