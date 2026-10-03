param(
    [Parameter(Position = 0)]
    [ValidateSet("up", "down", "restart", "logs")]
    [string]$Action = "up"
)

$ErrorActionPreference = "Stop"

$services = @("time2pay.postgres", "time2pay.aspire-dashboard")

Set-Location -Path $PSScriptRoot

switch ($Action)
{
    "up" {
        docker compose up -d @services
        Write-Host ""
        Write-Host "Aspire Dashboard: http:localhost:18888" -ForegroundColor Green
    }
    "down" {
        docker compose stop @services
        docker compose rm -f @services
    }
    "restart" {
        docker compose restart @services
    }
    "logs" {
        docker compose logs -f @services
    }
}