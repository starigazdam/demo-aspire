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
if PATH="$tmpdir:$PATH" "$repo_root/scripts/destroy.sh" dev >"$tmpdir/preview" 2>&1; then
  printf 'destroy requires --yes\n' >&2
  exit 1
fi
test ! -e "$DESTROY_TEST_LOG"
grep -Fqx 'Destroying resource group demo-aspire-dev in subscription test-subscription' "$tmpdir/preview"
grep -Fqx 'pass --yes to confirm destruction' "$tmpdir/preview"

if PATH="$tmpdir:$PATH" "$repo_root/scripts/destroy.sh" prod --yes >/dev/null 2>&1; then
  printf 'invalid environment succeeded\n' >&2
  exit 1
fi
test ! -e "$DESTROY_TEST_LOG"

if PATH="$tmpdir:$PATH" "$repo_root/scripts/destroy.sh" --yes dev >/dev/null 2>&1; then
  printf 'reversed destroy arguments succeeded\n' >&2
  exit 1
fi
test ! -e "$DESTROY_TEST_LOG"

PATH="$tmpdir:$PATH" "$repo_root/scripts/destroy.sh" dev --yes >"$tmpdir/output"

grep -Fqx 'subscription=test-subscription' "$DESTROY_TEST_LOG"
grep -Fqx 'location=uksouth' "$DESTROY_TEST_LOG"
grep -Fqx 'resource_group=demo-aspire-dev' "$DESTROY_TEST_LOG"
grep -Fqx 'Destroying resource group demo-aspire-dev in subscription test-subscription' "$tmpdir/output"
grep -F 'args=tool run aspire -- destroy ' "$DESTROY_TEST_LOG" >/dev/null
grep -F -- "--apphost $repo_root/DemoAspire.AppHost --environment dev --non-interactive --yes" "$DESTROY_TEST_LOG" >/dev/null
