#!/usr/bin/env bash
# Dual Frontier — Linux sandbox provisioning (Claude Code on the web / any Ubuntu-like host).
# Installs the exact toolchain the repo needs and prepares native-library discovery so the
# full solution builds and the entire test suite runs on a fresh container.
#
# Idempotent: every step is skip-if-present; safe to re-run. Manual use:
#   ./scripts/setup-linux-sandbox.sh
set -euo pipefail

[ "$(uname -s)" = "Linux" ] || exit 0

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
DOTNET_ROOT="${DOTNET_ROOT:-$HOME/.dotnet}"
SDK_PIN="10.0.204"          # SDK anchor per tools/DualFrontier.Governance csproj + STACK_UPDATE brief
RUNTIME8_CHANNEL="8.0"      # net8.0 test hosts need a Microsoft.NETCore.App 8.x runtime
NATIVE_DIR="$REPO_ROOT/native/DualFrontier.Core.Native"
NATIVE_BUILD="$NATIVE_DIR/build"

log() { echo "[setup-linux-sandbox] $*"; }

run_root() {
  if [ "${EUID:-$(id -u)}" -eq 0 ]; then "$@"
  elif command -v sudo >/dev/null 2>&1; then sudo "$@"
  else log "WARN: not root and no sudo — skipped: $*"
  fi
}

# --- 1. apt packages: Vulkan headers/loader (find_package(Vulkan) is REQUIRED),
#        software Vulkan device (lavapipe), shader/native toolchain ---
APT_PKGS=(libvulkan-dev mesa-vulkan-drivers vulkan-tools glslang-tools cmake ninja-build g++)
MISSING=()
for p in "${APT_PKGS[@]}"; do dpkg -s "$p" >/dev/null 2>&1 || MISSING+=("$p"); done
if [ "${#MISSING[@]}" -gt 0 ]; then
  log "installing apt packages: ${MISSING[*]}"
  run_root env DEBIAN_FRONTEND=noninteractive apt-get update -qq
  run_root env DEBIAN_FRONTEND=noninteractive apt-get install -y -qq "${MISSING[@]}"
else
  log "apt packages present"
fi

# --- 2. .NET SDK (exact pin) + .NET 8 runtime ---
if [ ! -d "$DOTNET_ROOT/sdk/$SDK_PIN" ]; then
  log "installing .NET SDK $SDK_PIN"
  curl -fsSL --retry 4 https://dot.net/v1/dotnet-install.sh -o /tmp/dotnet-install.sh
  bash /tmp/dotnet-install.sh --version "$SDK_PIN" --install-dir "$DOTNET_ROOT"
else
  log ".NET SDK $SDK_PIN present"
fi
if ! ls "$DOTNET_ROOT/shared/Microsoft.NETCore.App"/8.0.* >/dev/null 2>&1; then
  log "installing .NET 8 runtime (latest $RUNTIME8_CHANNEL.x)"
  [ -f /tmp/dotnet-install.sh ] || curl -fsSL --retry 4 https://dot.net/v1/dotnet-install.sh -o /tmp/dotnet-install.sh
  bash /tmp/dotnet-install.sh --runtime dotnet --channel "$RUNTIME8_CHANNEL" --install-dir "$DOTNET_ROOT"
else
  log ".NET 8 runtime present"
fi
run_root ln -sf "$DOTNET_ROOT/dotnet" /usr/local/bin/dotnet
# Register the install location so framework-dependent apphosts (dotnet tools)
# resolve the runtime without DOTNET_ROOT in the environment.
if [ -w /etc ] || [ "${EUID:-$(id -u)}" -eq 0 ] || command -v sudo >/dev/null 2>&1; then
  run_root mkdir -p /etc/dotnet
  printf '%s\n' "$DOTNET_ROOT" | run_root tee /etc/dotnet/install_location >/dev/null
  printf '%s\n' "$DOTNET_ROOT" | run_root tee /etc/dotnet/install_location_x64 >/dev/null
fi

# --- 3. shell profile (manual/interactive shells) ---
BLOCK_MARK="dual-frontier sandbox toolchain"
for f in "$HOME/.bashrc" "$HOME/.profile"; do
  if ! grep -q "$BLOCK_MARK" "$f" 2>/dev/null; then
    { echo ""
      echo "# >>> $BLOCK_MARK >>>"
      echo "export DOTNET_ROOT=\"$DOTNET_ROOT\""
      echo 'export PATH="$DOTNET_ROOT:$DOTNET_ROOT/tools:$PATH"'
      echo "export DOTNET_CLI_TELEMETRY_OPTOUT=1"
      # Persistent nodeReuse MSBuild nodes inherit redirected pipes and deadlock
      # tests that spawn child dotnet builds (M74BuildPipelineTests WaitForExit).
      echo "export MSBUILDDISABLENODEREUSE=1"
      echo "export LD_LIBRARY_PATH=\"$NATIVE_BUILD\${LD_LIBRARY_PATH:+:\$LD_LIBRARY_PATH}\""
      echo "# <<< $BLOCK_MARK <<<"
    } >> "$f"
  fi
done

# --- 4. native library build + loader discovery ---
# The lib-prefixed symlink + ld.so.conf.d entry make the .so discoverable by the
# standard DllImport probing without LD_LIBRARY_PATH (the csproj <None> copy step
# only covers the Windows build/Release/*.dll layout).
log "building native library (Release)"
cmake -S "$NATIVE_DIR" -B "$NATIVE_BUILD" -DCMAKE_BUILD_TYPE=Release >/dev/null
cmake --build "$NATIVE_BUILD" -j"$(nproc)" >/dev/null
ln -sf DualFrontier.Core.Native.so "$NATIVE_BUILD/libDualFrontier.Core.Native.so"
if [ -d /etc/ld.so.conf.d ]; then
  echo "$NATIVE_BUILD" | run_root tee /etc/ld.so.conf.d/dual-frontier-native.conf >/dev/null
  run_root ldconfig
fi
"$NATIVE_BUILD/df_native_selftest" >/dev/null && log "native selftest: ALL PASSED"

# --- 5. NuGet warm-up (first build/test becomes instant; cached with the container) ---
export PATH="$DOTNET_ROOT:$PATH"
export DOTNET_CLI_TELEMETRY_OPTOUT=1
export MSBUILDDISABLENODEREUSE=1
log "restoring NuGet packages"
dotnet restore "$REPO_ROOT/DualFrontier.sln" >/dev/null
log "restore done"

# --- 6. session env for Claude Code on the web ---
if [ -n "${CLAUDE_ENV_FILE:-}" ] && ! grep -q "DOTNET_ROOT" "$CLAUDE_ENV_FILE" 2>/dev/null; then
  { echo "export DOTNET_ROOT=\"$DOTNET_ROOT\""
    echo 'export PATH="$DOTNET_ROOT:$DOTNET_ROOT/tools:$PATH"'
    echo "export DOTNET_CLI_TELEMETRY_OPTOUT=1"
    echo "export MSBUILDDISABLENODEREUSE=1"
    echo "export LD_LIBRARY_PATH=\"$NATIVE_BUILD\${LD_LIBRARY_PATH:+:\$LD_LIBRARY_PATH}\""
  } >> "$CLAUDE_ENV_FILE"
fi

log "done"
