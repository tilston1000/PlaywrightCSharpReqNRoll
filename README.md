# PlaywrightReqnroll UI Test Project

This project contains automated UI tests using .NET, Playwright, Reqnroll, and Allure for reporting.

## Prerequisites

- .NET 9 SDK
- Node.js (for Allure CLI)
- A `.env` file (or environment variables) with test credentials:
  - `TEST_USERNAME`
  - `TEST_PASSWORD`
    Notes:
- Playwright CLI is managed as a local .NET tool via [.config/dotnet-tools.json](.config/dotnet-tools.json).
- Allure CLI is installed by the local script/workflow when needed.

## Project Structure

- `Features/` - Gherkin feature files
- `StepDefinitions/` - Step definition classes
- `Pages/` - Page Object Model classes
- `Hooks/PlaywrightHooks.cs` - Reqnroll lifecycle orchestration (scenario setup/teardown)
- `Hooks/AllureHook.cs` - Attaches screenshots and videos to Allure reports
- `Helpers/TestArtifactHelper.cs` - Artifact naming/path and attachment coordination
- `Helpers/TestStartupHelper.cs` - Startup tasks (env loading and video cleanup)
- `run-tests.local.ps1` - PowerShell script for local builds, tests, and report viewing
- `.github/workflows/playwright-tests.yml` - CI workflow for GitHub Actions

## Running Tests Locally

1. Install prerequisites (see above).
2. Ensure `.env` exists (or export env vars) with `TEST_USERNAME` and `TEST_PASSWORD`.
3. Open a PowerShell terminal in the project root.
4. Run:
   ```
   ./run-tests.local.ps1
   ```
   To run a specific browser:

```
./run-tests.local.ps1 -Browser firefox
```

To run all supported browsers locally:

```
./run-tests.local.ps1 -Browser all
```

To run a specific test subset locally:

```
./run-tests.local.ps1 -Browser chromium -TestFilter "TestCategory=smoke"
```

5. The script will:

- build the project,
- install Playwright browsers if needed,
- run smoke tests on the selected browser(s),
- generate browser-specific Allure reports (`allure-report-chromium`, `allure-report-firefox`, `allure-report-webkit` as applicable),
- generate and open one merged report in `allure-report`.

## Running Tests in GitHub Actions

Tests run in CI via [playwright-tests.yml](.github/workflows/playwright-tests.yml) in three trigger modes:

1. Push to `main`.
2. Pull request targeting `main`.
3. Manual run (`workflow_dispatch`) with browser input: `all`, `chromium`, `firefox`, or `webkit`.

CI behavior:

- Runs per-browser matrix jobs (or single browser for manual runs).
- Restores/builds the project and installs Playwright browser dependencies (`--with-deps`).
- Runs tests headless (`HEADLESS=true`, `SlowMo=0`).
- Produces per-browser Allure results and per-browser Allure reports.
- Merges available browser results into one merged Allure report.
- Deploys merged report to GitHub Pages on `main` for both push and manual runs, even if one browser leg fails, as long as a merged report exists.
- Restores and saves Allure history between runs so trend widgets continue over time.

How to run a specific browser manually in CI:

1. Open Actions in GitHub.
2. Select Playwright UI Tests.
3. Click Run workflow.
4. Choose browser input (`chromium`, `firefox`, `webkit`, or `all`).

After completion, view the report via GitHub Pages or download run artifacts from the workflow summary.

## Allure Reporting

- Screenshots and videos are attached to failed scenarios via the `AllureHook`.
- The Reqnroll Allure plugin and results directory are configured in [reqnroll.json](reqnroll.json).
- Additional Allure runtime behavior flags (`cleanResultsDirectory`, `enable`, `loggingLevel`) are configured in [allureConfig.json](allureConfig.json).
- Local runs can produce browser-specific reports and a merged report. Use `allure open allure-report` for the merged view or `allure open allure-report-<browser>` for a browser-specific view.
- CI preserves trend history by restoring/saving the Allure history artifact between runs.

## Security

- Keep dependencies up to date for best security.
