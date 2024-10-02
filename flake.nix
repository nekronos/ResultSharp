{
  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/nixos-unstable";
    flake-compat = {
      url = "github:edolstra/flake-compat";
      flake = false;
    };
    flake-utils.url = "github:numtide/flake-utils";
  };

  outputs =
    {
      self,
      nixpkgs,
      flake-utils,
      ...
    }:
    flake-utils.lib.eachDefaultSystem (
      system:
      let
        pkgs = import nixpkgs { inherit system; };
        lastModifiedDate = self.lastModifiedDate or self.lastModified or "19700101";
        version = builtins.substring 0 8 lastModifiedDate;
      in
      rec {
        packages = {
          default = pkgs.buildDotnetModule {
            pname = "ResultSharp";
            version = "3.0.0";
            src = ./.;
            nugetDeps = ./deps.nix;
            dotnet-sdk = pkgs.dotnet-sdk_8;
            dotnet-runtime = pkgs.dotnet-runtime_8;
            projectFile = "ResultSharp/ResultSharp.csproj";
            testProjectFile = "ResultSharp.Tests/ResultSharp.Tests.csproj";
            doCheck = true;
            # packNupkg = true;
          };
        };
        devShells = {
          default = pkgs.mkShell {
            packages =
              with pkgs;
              [
                omnisharp-roslyn
              ]
              ++ packages.default.nativeBuildInputs;
          };
        };
      }
    );
}
