cd ../src/Camelot
# Publish
dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true -p:PublishReadyToRun=true --self-contained=true
cd ../../scripts
