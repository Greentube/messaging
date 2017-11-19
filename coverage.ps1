# Run locally to see html report: powershell ./coverage.ps1 -generateReport
# To publish results to Codecov, set var: CODECOV_TOKEN, powershell ./coverage.ps1 -uploadCodecov
# appveyor has variable CODECOV_TOKEN set

Param (
  [switch] $generateReport,
  [switch] $uploadCodecov
  )

$currentPath = Split-Path $MyInvocation.MyCommand.Path
$coverageOutputDirectory = Join-Path $currentPath "coverage"
$coverageFile = "coverage-results.xml"

Remove-Item $coverageOutputDirectory -Force -Recurse -ErrorAction SilentlyContinue
Remove-Item $coverageFile -ErrorAction SilentlyContinue

nuget install -Verbosity quiet -OutputDirectory packages -Version 4.6.519 OpenCover

$openCoverConsole = "packages\OpenCover.4.6.519\tools\OpenCover.Console.exe"
# OpenCover currently not supporting portable pdbs (https://github.com/OpenCover/opencover/issues/601)
$configuration = "Coverage"

Get-ChildItem -Filter .\test\ |
    ForEach-Object {
      $csprojPath = $_.FullName
      $testProjectName = $_.Name
      $projectName = $testProjectName -replace ".{6}$"
        cmd.exe /c $openCoverConsole `
          -target:"c:\Program Files\dotnet\dotnet.exe" `
          -targetargs:"test -c $configuration $csprojPath\$testProjectName.csproj" `
          -mergeoutput `
          -hideskipped:File `
          -output:$coverageFile `
          -oldStyle `
          -filter:"+[$projectName]* -[$testProjectName]* -[Greentube.Messaging]*Attribute -[xunit*]*" `
          -searchdirs:"$csprojPath\bin\$configuration\netcoreapp2.0\" `
          -register:user
    }

If ($generateReport) {
  nuget install -Verbosity quiet -OutputDirectory packages -Version 3.0.2 ReportGenerator
  $reportGenerator = "packages\ReportGenerator.3.0.2\tools\ReportGenerator.exe"
  cmd.exe /c $reportGenerator `
    -reports:$coverageFile `
    -targetdir:$coverageOutputDirectory `
    -verbosity:Error
}

# requires variable set: CODECOV_TOKEN
If ($uploadCodeCov) {
  nuget install -Verbosity quiet -OutputDirectory packages -Version 1.0.3 Codecov
  $Codecov = "packages\Codecov.1.0.3\tools\Codecov.exe"
  cmd.exe /c $Codecov -f $coverageFile
}