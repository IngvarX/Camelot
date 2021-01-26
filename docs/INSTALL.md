# How to install Camelot

## Precompiled binary (all platforms)

Download precompiled binary from [releases page](https://github.com/IngvarX/Camelot/releases)

## Package (Linux)

**RPM and DEB:** precompiled rpm and deb packages are availbale on [releases page](https://github.com/IngvarX/Camelot/releases)

**AUR:** [camelot-git](https://aur.archlinux.org/packages/camelot-git/) and [camelot](https://aur.archlinux.org/packages/camelot/) packages are available

## Build from sources (all platforms)

1) Install .Net Core SDK

2) Clone repository and build solution:

```
git clone https://github.com/IngvarX/Camelot.git
cd Camelot
dotnet restore
dotnet build --no-restore
dotnet run --project src/Camelot/Camelot.csproj
```
