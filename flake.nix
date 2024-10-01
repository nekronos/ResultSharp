{
  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/nixos-unstable";
    flake-compat = {
      url = "github:edolstra/flake-compat";
      flake = false;
    };
    systems.url = "github:nix-systems/default";
  };

  outputs =
    {
      self,
      nixpkgs,
      systems,
      ...
    }:
    let
      lastModifiedDate = self.lastModifiedDate or self.lastModified or "19700101";
      version = builtins.substring 0 8 lastModifiedDate;
      eachSystem = nixpkgs.lib.genAttrs (import systems);
      nixpkgsFor = eachSystem (system: import nixpkgs { inherit system; });
    in
    rec {
      packages = eachSystem (
        system:
        let
          pkgs = nixpkgsFor.${system};
        in
        {
          default = pkgs.buildDotnetModule {
            pname = "ResultSharp";
            #            inherit version;
            version = "3.0.0";

            src = ./.;
            nugetDeps = ./deps.nix;
            dotnet-sdk = pkgs.dotnet-sdk_8;
            dotnet-runtime = pkgs.dotnet-runtime_8;
            projectFile = "ResultSharp.sln";
            packNupkg = true;
          };
        }
      );
      devShells = eachSystem (
        system:
        let
          pkgs = nixpkgsFor.${system};
          defaultPackage = packages.${system}.default;
        in
        {
          default = pkgs.mkShell {
            packages =
              with pkgs;
              [
                omnisharp-roslyn
              ]
              ++ defaultPackage.nativeBuildInputs;
          };
        }
      );
    };
}
