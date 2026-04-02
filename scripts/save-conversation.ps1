<<<<<<< HEAD
$ErrorActionPreference = "Stop"

$source = ".chat/latest.md"
$target = "lab-1/conversation-log.md"

if (-not (Test-Path $source)) {
    exit 0
}

$content = Get-Content $source -Raw
if ([string]::IsNullOrWhiteSpace($content)) {
    exit 0
}

$targetDir = Split-Path $target -Parent
if (-not (Test-Path $targetDir)) {
    New-Item -ItemType Directory -Path $targetDir -Force | Out-Null
}

$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss K"
$entry = @"
## Conversation saved: $timestamp

$content

---
"@

Add-Content -Path $target -Value $entry

git add $target | Out-Null

Set-Content -Path $source -Value ""
=======
$ErrorActionPreference = "Stop"

$source = ".chat/latest.md"
$target = "lab-1/conversation-log.md"

if (-not (Test-Path $source)) {
    exit 0
}

$content = Get-Content $source -Raw
if ([string]::IsNullOrWhiteSpace($content)) {
    exit 0
}

$targetDir = Split-Path $target -Parent
if (-not (Test-Path $targetDir)) {
    New-Item -ItemType Directory -Path $targetDir -Force | Out-Null
}

$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss K"
$entry = @"
## Conversation saved: $timestamp

$content

---
"@

Add-Content -Path $target -Value $entry

git add $target | Out-Null

Set-Content -Path $source -Value ""
>>>>>>> ed8937926970a6d087a5c69af7505e0bfa96e32a
