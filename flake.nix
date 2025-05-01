{
  description = "A flake based on my NixOS configuration";

  inputs = {
    nixpkgs.url = "github:nixos/nixpkgs?ref=nixos-unstable";
    systems.url = "github:nix-systems/default-linux";

    flake-parts = {
      url = "github:hercules-ci/flake-parts";
      inputs.nixpkgs-lib.follows = "nixpkgs";
    };

    nixcfg = {
      url = "github:kieranknowles1/nixcfg";
      inputs.nixpkgs.follows = "nixpkgs";
      inputs.flake-parts.follows = "flake-parts";
      inputs.systems.follows = "systems";

      # Disable inputs not needed outside of nixcfg
      # These can be re-enabled if needed
      inputs.clan-core.follows = "";
      inputs.factorio-blueprints.follows = "";
      inputs.firefox-addons.follows = "";
      inputs.flake-utils-plus.follows = "";
      inputs.flake-utils.follows = "";
      inputs.home-manager.follows = "";
      inputs.nix-index-database.follows = "";
      inputs.nixos-cosmic.follows = "";
      inputs.nixpkgs-openmw.follows = "";
      inputs.nixpkgs-stable.follows = "";
      inputs.openmw.follows = "";
      inputs.snowfall-lib.follows = "";
      inputs.sops-nix.follows = "";
      inputs.src-factorio-blueprint-decoder.follows = "";
      inputs.src-tldr.follows = "";
      inputs.stylix.follows = "";
      inputs.vscode-extensions.follows = "";
    };
  };

  outputs = {
    flake-parts,
    nixcfg,
    ...
  } @ inputs: let
    cfgLib = nixcfg.lib;
  in
    flake-parts.lib.mkFlake {inherit inputs;} {
      systems = import inputs.systems;

      imports = [
        # Can be overridden with https://flake.parts/options/treefmt-nix options
        nixcfg.flakeModules.treefmt
      ];

      flake = {
        # Shared across all systems
      };

      perSystem = {
        pkgs,
        inputs',
        ...
      }: {
        # Per system type
        devShells = {
          # Wrapper that sets the magic DEVSHELL variable, and preserves the user's default shell
          # Usage: `nix develop [.#name=default]`
          default = cfgLib.shell.mkShellEx pkgs.mkShellNoCC {
            name = "dev";
            packages = with pkgs; [
              hello
            ];

            shellHook = ''
              echo "Hello, world!"
            '';
          };
        };

        packages = {
          sync-notes = let
            home = "/home/kieran";
          in
            inputs'.nixcfg.packages.export-notes.override {
              apiKeyFile = "${home}/.config/sops-nix/secrets/trilium/apikey";
              destinationDir = "${home}/Documents/src/vindolanda-vr/notes";
              # ID of my "Dissertation" note
              rootNote = "cezCWFEiud4n";
              autoCommit = false;
              format = "html";
            };
        };
      };
    };
}
