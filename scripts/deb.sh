cd ../src/Camelot

dotnet tool install --global dotnet-deb
dotnet deb install
dotnet deb --configuration Release

cd -