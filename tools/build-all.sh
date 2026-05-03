#!/usr/bin/env bash
# tools/build-all.sh
# Full Dual Frontier build: .sln (kernel + tests + tools) + Presentation (Godot).
#
# Usage:
#   ./tools/build-all.sh                    # Debug build
#   ./tools/build-all.sh --config Release   # Release build
#   ./tools/build-all.sh --help

set -euo pipefail

CONFIG="Debug"

while [[ $# -gt 0 ]]; do
    case "$1" in
        --config|-c)
            if [[ $# -lt 2 ]]; then
                echo "Error: --config requires a value (Debug or Release)" >&2
                exit 1
            fi
            CONFIG="$2"
            shift 2
            ;;
        --help|-h)
            cat <<'EOF'
Builds the full Dual Frontier solution including DualFrontier.Presentation
(Godot project, deliberately not in .sln due to Godot SDK CI fragility on
machines without Godot installed).

Equivalent to:
    dotnet build DualFrontier.sln -c <Configuration>
    dotnet build src/DualFrontier.Presentation/DualFrontier.Presentation.csproj -c <Configuration>

See docs/DEVELOPMENT_HYGIENE.md §2 ('dotnet build' is green) for rationale.
EOF
            exit 0
            ;;
        *)
            echo "Unknown argument: $1" >&2
            echo "Run with --help for usage." >&2
            exit 1
            ;;
    esac
done

# Resolve repo root from script location (CWD-agnostic).
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"

cd "$REPO_ROOT"

echo "==> Building DualFrontier.sln ($CONFIG) ..."
dotnet build DualFrontier.sln -c "$CONFIG"

echo
echo "==> Building DualFrontier.Presentation ($CONFIG) ..."
dotnet build src/DualFrontier.Presentation/DualFrontier.Presentation.csproj -c "$CONFIG"

echo
echo "==> Full build successful ($CONFIG)"
