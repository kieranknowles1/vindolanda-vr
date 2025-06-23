#!/usr/bin/env bash
set -euo pipefail

# Script to simulate a devshell, since the repo is too big to practically
# make it a flake.
nix shell nixpkgs#texliveFull
