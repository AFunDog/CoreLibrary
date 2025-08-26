$org = Get-Location
Set-Location ..
dotnet build -c Release
dotnet pack -c Release --no-build -o ./nuget
Set-Location $org