param ($version='latest')

$currentFolder = $PSScriptRoot
$slnFolder = Join-Path $currentFolder "../../"

Write-Host "********* BUILDING DbMigrator *********" -ForegroundColor Green
$dbMigratorFolder = Join-Path $slnFolder "src/BankSimulator.DbMigrator"
Set-Location $dbMigratorFolder
dotnet publish -c Release
docker build -f Dockerfile.local -t mycompanyname/banksimulator-db-migrator:$version .

Write-Host "********* BUILDING Blazor Application *********" -ForegroundColor Green
$blazorFolder = Join-Path $slnFolder "src/BankSimulator.Blazor"
Set-Location $blazorFolder
dotnet publish -c Release -p:PublishTrimmed=false
docker build -f Dockerfile.local -t mycompanyname/banksimulator-blazor:$version .

Write-Host "********* BUILDING Api.Host Application *********" -ForegroundColor Green
$hostFolder = Join-Path $slnFolder "src/BankSimulator.HttpApi.Host"
Set-Location $hostFolder
dotnet publish -c Release
docker build -f Dockerfile.local -t mycompanyname/banksimulator-api:$version .


### ALL COMPLETED
Write-Host "COMPLETED" -ForegroundColor Green
Set-Location $currentFolder