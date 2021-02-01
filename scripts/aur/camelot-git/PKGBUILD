pkgname=camelot-git
_gitname="Camelot"
_outputfolder="Release"
pkgrel=1
pkgver=v0.2.1
url="https://github.com/IngvarX/Camelot"
pkgdesc="Cross-platform file manager written in C#"
arch=("any")
conflicts=("camelot")
provides=("camelot")
license=("GPL-3.0")
depends=("dotnet-runtime-3.1")
makedepends=("dotnet-sdk-3.1" "git")

source=("git+https://github.com/IngvarX/${_gitname}.git")
sha256sums=("SKIP")

pkgver() {
  cd $_pkgname/$srcdir/$_gitname
  echo "$(grep -E "<Version>.*</Version>" src/Camelot/Camelot.csproj | sed 's/.*<Version>\(.*\)<\/Version>/\1/').$(git rev-list --count HEAD)"
}

build() {
  cd $srcdir/$_gitname
  dotnet build --configuration release --output "${_outputfolder}"
}

package() { 
  mkdir -p "${pkgdir}/usr/share/"
  cp -r "${srcdir}/${_gitname}/${_outputfolder}/" "${pkgdir}/usr/share/${pkgname}"
  mkdir -p "${pkgdir}/usr/bin/"
  ln -s "/usr/share/${pkgname}/camelot" "${pkgdir}/usr/bin/${pkgname}"

  # License
  install -Dm644 "${srcdir}/${_gitname}/LICENSE.md" "${pkgdir}/usr/share/licenses/$pkgname/LICENSE.md"

  # Icon for .desktop
  install -Dm644 "${srcdir}/${_gitname}/src/Camelot/Assets/logo.png" "${pkgdir}/usr/share/icons/hicolor/512x512/apps/$pkgname.png"

  # .desktop
  install -Dm644 "${srcdir}/${_gitname}/camelot.desktop" "${pkgdir}/usr/share/applications/${pkgname}.desktop"
}
