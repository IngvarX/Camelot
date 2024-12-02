cd ../src/Camelot

# Restore
/usr/local/bin/dotnet restore -r osx-x64

# Build
/usr/local/bin/dotnet msbuild -t:BundleApp -p:RuntimeIdentifier=osx-x64 -property:Configuration=Release

# Create icon
mkdir Assets/logo.iconset
cp Assets/logo.png Assets/logo.iconset/icon_512x512.png
iconutil -c icns Assets/logo.iconset
cp Assets/logo.icns bin/Release/net8.0/osx-x64/publish/Camelot.app/Contents/Resources/logo.icns
rm Assets/logo.icns
rm -rf Assets/logo.iconset

cd -
