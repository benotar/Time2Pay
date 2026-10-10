param(
    [Parameter(Position = 0)]
    [ValidateSet("up", "down", "restart", "logs")]
    [string]$Action = "up"
)

$ErrorActionPreference = "Stop"

$services = @("time2pay.postgres", "time2pay.aspire-dashboard")

$root = Split-Path -Parent $PSScriptRoot
$composeFile = Join-Path $root "compose.yaml"

function Invoke-Compose
{
    docker compose -f $composeFile @args

    if ($LASTEXITCODE -ne 0)
    {
        throw "docker compose $( $args -join ' ' ) failed with exit code $LASTEXITCODE"
    }
}

switch ($Action)
{
    "up" {
        Invoke-Compose up -d @services
        Write-Host ""
        Write-Host "Aspire Dashboard: http://localhost:18888" -ForegroundColor Green
    }
    "down" {
        Invoke-Compose stop @services
        Invoke-Compose rm -f @services
    }
    "restart" {
        Invoke-Compose restart @services
    }
    "logs" {
        Invoke-Compose logs -f @services
    }
}

# .\scripts\dev-infra.ps1 up