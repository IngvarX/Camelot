# How to install Camelot

## Precompiled binary (all platforms)

Download precompiled binary from [releases page](https://github.com/IngvarX/Camelot/releases)

[![Release](https://img.shields.io/github/v/release/IngvarX/Camelot?style=for-the-badge)](https://github.com/IngvarX/Camelot/releases)

## Package (Linux)

**RPM and DEB:** precompiled rpm and deb packages are available on [releases page](https://github.com/IngvarX/Camelot/releases)

**AUR:**

Latest version:

[camelot-git](https://aur.archlinux.org/packages/camelot-git/)

[![camelot-git](https://img.shields.io/aur/last-modified/camelot-git?style=for-the-badge)](https://aur.archlinux.org/packages/camelot-git/)

Stable release:

[camelot](https://aur.archlinux.org/packages/camelot/)

[![camelot](https://img.shields.io/aur/last-modified/camelot?style=for-the-badge)](https://aur.archlinux.org/packages/camelot/)

## Build from sources (all platforms)

1) Install .NET 6 SDK

2) Clone repository and build solution:

```
git clone https://github.com/IngvarX/Camelot.git
cd Camelot
dotnet restore
dotnet build --no-restore
dotnet run --project src/Camelot/Camelot.csproj
```
