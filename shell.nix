with import <nixpkgs> {};

mkShell rec {
  name = "dotnet";
  buildInputs = with pkgs; [
    dotnet-sdk_3
  ];
}
