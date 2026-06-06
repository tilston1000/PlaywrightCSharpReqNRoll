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
5. The script will:

- build the project,
- install Playwright browsers if needed,
- run smoke tests,
- generate and open the Allure report.

## Running Tests in GitHub Actions

Tests are automatically run in CI using GitHub Actions. Here’s what happens:

1. On every push or pull request to the `main` branch, the workflow in `.github/workflows/playwright-tests.yml` is triggered.
2. The workflow then:

- checks out the code,
- restores local .NET tools (`dotnet tool restore`),
- restores/builds the project,
- caches Playwright browsers and installs them on cache miss,
- runs tests in headless mode (`HEADLESS=true`, `SlowMo=0`),
- collects Allure results,
- generates and uploads Allure artifacts,
- deploys report to GitHub Pages on pushes to `main`.

3. After the workflow completes, you can view the report from the GitHub Pages URL or download the artifacts from the run summary.

No manual action is needed—just push your code or open a pull request, and the tests will run automatically in CI.

If GitHub Pages is enabled for the repository, the report is published at the Pages URL shown in the workflow run.

## Allure Reporting

- Screenshots and videos are attached to failed scenarios via the `AllureHook`.
- The Reqnroll Allure plugin and results directory are configured in [reqnroll.json](reqnroll.json).
- Additional Allure runtime behavior flags (`cleanResultsDirectory`, `enable`, `loggingLevel`) are configured in [allureConfig.json](allureConfig.json).
- To view the report locally, run `allure open allure-report` after test execution, or let `run-tests.local.ps1` open it for you.

## Security

- Keep dependencies up to date for best security.
