#!/bin/bash
# SessionStart hook — Claude Code on the web only. Provisions the Linux sandbox
# toolchain (pinned .NET SDK, Vulkan/native build, NuGet warm-up) via the
# idempotent repo script. No-op on local machines (CLAUDE_CODE_REMOTE unset).
set -euo pipefail
if [ "${CLAUDE_CODE_REMOTE:-}" != "true" ]; then
  exit 0
fi
exec "$CLAUDE_PROJECT_DIR/scripts/setup-linux-sandbox.sh"
