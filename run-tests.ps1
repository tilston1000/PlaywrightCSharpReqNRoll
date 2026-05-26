# Remove TestResults folder if it exists
if (Test-Path "TestResults") {
    Remove-Item -Recurse -Force "TestResults"
}

# Ensure HEADLESS env var does not override appsettings.json
if ($env:HEADLESS) { Remove-Item Env:HEADLESS }

# Clean previous results and report
if (Test-Path "allure-results") { Remove-Item "allure-results" -Recurse -Force }
if (Test-Path "bin\\Debug\\net9.0\\allure-results") { Remove-Item "bin\\Debug\\net9.0\\allure-results" -Recurse -Force }

# Restore Allure history for trend tracking
if (Test-Path "allure-report/history") {
    if (-not (Test-Path "allure-results")) {
        New-Item -ItemType Directory -Path "allure-results" | Out-Null
    }
    Copy-Item "allure-report/history" "allure-results/history" -Recurse -Force
}

# Set Allure output directory
$env:ALLURE_RESULTS_DIRECTORY = "allure-results"


# Find the correct bin output path (handles win-x64, linux-x64, etc.)
$binRoot = "bin\Debug\net9.0"
$binPath = $binRoot
if (Test-Path "$binRoot\win-x64") { $binPath = "$binRoot\win-x64" }
elseif (Test-Path "$binRoot\linux-x64") { $binPath = "$binRoot\linux-x64" }
elseif (Test-Path "$binRoot\osx-x64") { $binPath = "$binRoot\osx-x64" }

# Build and Run tests
dotnet build

# Install Playwright browsers
if (Test-Path "$binPath/playwright.ps1") {
    powershell -ExecutionPolicy Bypass -File "$binPath/playwright.ps1" install
} else {
    Write-Host "playwright.ps1 not found in $binPath, skipping browser install."
}

dotnet test --filter "TestCategory=smoke"

# If results exist in bin, copy them to project root
if (Test-Path "$binPath\allure-results") {
    if (-not (Test-Path "allure-results")) {
        New-Item -ItemType Directory -Path "allure-results" | Out-Null
    }
    Copy-Item "$binPath\allure-results\*" "allure-results" -Force -Recurse

    # Generate the Allure report (with --clean)
    allure generate allure-results --clean -o allure-report

    # Open the report in browser
    allure open allure-report
}
else {
    Write-Host "No Allure results found"
}

# If the folder is recreated and is empty, remove it again
if (Test-Path "TestResults") {
    $files = Get-ChildItem "TestResults"
    if ($files.Count -eq 0) {
        Remove-Item "TestResults" -Force
    }
}