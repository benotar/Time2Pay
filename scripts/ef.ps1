$ErrorActionPreference = 'Stop'

$contexts = @{
    Time2Pay = @{
        Project = 'src/Time2Pay.Api'
        ClassType = 'Time2PayDbContext'
        Output = 'Migrations/Time2Pay'
    }
}

if ($args.Count -eq 0 -or -not $contexts.ContainsKey($args[0]))
{
    throw "Usage: ef.ps1 <context> <dotnet ef args>. Known contexts: $( $contexts.Keys -join ', ' )"
}

$target = $contexts[$args[0]]
$efArgs = @($args | Select-Object -Skip 1)

$root = Split-Path -Parent $PSScriptRoot
$project = Join-Path $root $target.Project

$isMigrationsAdd = $efArgs.Count -ge 2 -and $efArgs[0] -eq 'migrations' -and $efArgs[1] -eq 'add'
$hasOutput = $efArgs -contains '-o' -or $efArgs -contains '--output-dir'

if ($isMigrationsAdd -and -not $hasOutput)
{
    $efArgs += '-o', $target.Output
}

Write-Host "dotnet ef $($efArgs -join ' ') --context $($target.ClassType) --project $project" -ForegroundColor DarkGray

dotnet ef @efArgs --context $target.ClassType --project $project

# .\scripts\ef.ps1 Time2Pay migrations add <Migrations name>

# .\scripts\ef.ps1 Time2Pay database update