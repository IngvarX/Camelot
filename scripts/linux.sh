cd ../src/Camelot
# Publish
dotnet publish -c Release -r linux-x64 -p:PublishSingleFile=true -p:PublishReadyToRun=true --self-contained=true
cd -
