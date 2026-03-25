$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path -Parent $PSScriptRoot
$envFilePath = Join-Path $repoRoot '.env'

if (-not (Test-Path $envFilePath)) {
    throw "Missing .env file. Copy .env.example to .env and fill in the values first."
}

function Get-EnvFileValues {
    param(
        [string]$Path
    )

    $values = @{}

    foreach ($line in Get-Content $Path) {
        $trimmed = $line.Trim()

        if ([string]::IsNullOrWhiteSpace($trimmed) -or $trimmed.StartsWith('#')) {
            continue
        }

        $key, $value = $trimmed -split '=', 2

        if (-not $key -or -not $value) {
            continue
        }

        $values[$key.Trim()] = $value.Trim()
    }

    return $values
}

function Escape-PowerShellValue {
    param(
        [string]$Value
    )

    return $Value.Replace("'", "''")
}

$envValues = Get-EnvFileValues -Path $envFilePath

$requiredKeys = @(
    'SA_PASSWORD',
    'SQL_DATABASE',
    'JWT_SECRET_KEY',
    'JWT_ISSUER',
    'JWT_AUDIENCE',
    'IDENTITY_API_URL',
    'PEOPLE_API_URL',
    'POST_API_URL'
)

foreach ($requiredKey in $requiredKeys) {
    if (-not $envValues.ContainsKey($requiredKey) -or [string]::IsNullOrWhiteSpace($envValues[$requiredKey])) {
        throw "Missing required key '$requiredKey' in .env."
    }
}

$defaultConnection = "Server=localhost,1433;Database=$($envValues['SQL_DATABASE']);User Id=sa;Password=$($envValues['SA_PASSWORD']);TrustServerCertificate=True;"

$sharedAssignments = @(
    "`$env:Jwt__SecretKey = '$(Escape-PowerShellValue $envValues['JWT_SECRET_KEY'])'",
    "`$env:Jwt__Issuer = '$(Escape-PowerShellValue $envValues['JWT_ISSUER'])'",
    "`$env:Jwt__Audience = '$(Escape-PowerShellValue $envValues['JWT_AUDIENCE'])'"
)

$apiAssignments = @(
    "`$env:ConnectionStrings__DefaultConnection = '$(Escape-PowerShellValue $defaultConnection)'"
) + $sharedAssignments

$frontendAssignments = @(
    "`$env:IdentityApiUrl = '$(Escape-PowerShellValue $envValues['IDENTITY_API_URL'])'",
    "`$env:PeopleApiUrl = '$(Escape-PowerShellValue $envValues['PEOPLE_API_URL'])'",
    "`$env:PostApiUrl = '$(Escape-PowerShellValue $envValues['POST_API_URL'])'"
) + $sharedAssignments

$services = @(
    @{
        Name = 'IdentityApi'
        Project = '.\IdentityApi\IdentityApi.csproj'
        Assignments = $apiAssignments
    },
    @{
        Name = 'PeopleApi'
        Project = '.\PeopleApi\PeopleApi.csproj'
        Assignments = $apiAssignments
    },
    @{
        Name = 'PostApi'
        Project = '.\PostApi\PostApi.csproj'
        Assignments = $apiAssignments
    },
    @{
        Name = 'SocialMediaApp'
        Project = '.\SocialMediaApp\SocialMediaApp.csproj'
        Assignments = $frontendAssignments
    }
)

foreach ($service in $services) {
    $commandParts = @(
        "Set-Location '$($repoRoot.Replace("'", "''"))'"
    ) + $service.Assignments + @(
        "dotnet watch run --project $($service.Project)"
    )

    $command = $commandParts -join '; '

    Start-Process powershell -WorkingDirectory $repoRoot -ArgumentList '-NoExit', '-Command', $command | Out-Null
}

Write-Host 'Started IdentityApi, PeopleApi, PostApi, and SocialMediaApp in separate PowerShell windows.'
Write-Host 'Make sure SQL Server is already running with: docker compose up -d mssql'