cd ../src/Camelot

dotnet tool install --global dotnet-rpm
dotnet rpm install
dotnet rpm --configuration Release

cd -