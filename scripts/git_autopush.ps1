<#
PowerShell helper: git_autopush.ps1
Usage:
  ./scripts/git_autopush.ps1 -Message "Update" [-Branch "main"] [-Tag "v1.2.3"] [-Push]

What it does:
 - configures git user if environment vars GIT_USER / GIT_EMAIL are set
 - stages all changes, commits with provided message
 - optionally creates a tag
 - pushes to remote origin on specified branch

Notes:
 - The script uses your existing git authentication (SSH agent / credential manager).
 - To run non-interactively from CI, pass -Push and supply proper git credentials (PAT or SSH).
 - To allow running on Windows with default policy: powershell -ExecutionPolicy Bypass -File .\scripts\git_autopush.ps1 -Message "..." -Push
#>
param(
    [Parameter(Mandatory=$true)][string]$Message,
    [string]$Branch = "main",
    [string]$Tag = "",
    [switch]$Push
)

function Write-Info($m){ Write-Host "[info] $m" -ForegroundColor Cyan }
function Write-Err($m){ Write-Host "[error] $m" -ForegroundColor Red }

try {
    # optional global identity
    if ($env:GIT_USER) { git config user.name $env:GIT_USER | Out-Null }
    if ($env:GIT_EMAIL) { git config user.email $env:GIT_EMAIL | Out-Null }

    Write-Info "Checking git status..."
    $status = git status --porcelain
    if ([string]::IsNullOrWhiteSpace($status)) {
        Write-Info "No changes to commit. Exiting."
        exit 0
    }

    Write-Info "Staging changes..."
    git add -A

    Write-Info "Committing: $Message"
    git commit -m "$Message" || {
        Write-Err "Commit failed or nothing to commit."
        exit 1
    }

    if ($Tag -and $Tag.Trim() -ne "") {
        Write-Info "Creating tag: $Tag"
        git tag -a $Tag -m "Tag $Tag"
    }

    if ($Push.IsPresent) {
        Write-Info "Pushing to origin/$Branch"
        git push origin $Branch
        if ($Tag -and $Tag.Trim() -ne "") {
            Write-Info "Pushing tags"
            git push origin --tags
        }
        Write-Info "Push complete."
    }
    else {
        Write-Info "Local commit created. Use -Push to push to remote."
    }
}
catch {
    Write-Err $_.Exception.Message
    exit 1
}
