#!/usr/bin/env bash
set -euo pipefail

# Find all references to an asset based on its ID, extracted from the .meta
# file. Unity's search is a bit unreliable, so use this to find if something
# is safe to delete

# Dirty hack since I don't have ripgrep on the PATH, so have copied it into
# PWD instead
PATH="$PATH:$PWD"

ASSET_ROOT=vindolanda/Assets

id="$1"

rg --files-with-matches "$id" "$ASSET_ROOT" || echo "No references found"
