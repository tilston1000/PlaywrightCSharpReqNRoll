# PlaywrightReqnroll UI Test Project

This project contains automated UI tests using .NET, Playwright, Reqnroll, and Allure for reporting.

## Prerequisites

- .NET 9 SDK
- Node.js (for Allure CLI)
- Playwright CLI (`dotnet tool install Microsoft.Playwright.CLI`)
- Allure CLI (installed locally by the scripts/workflow)
- `reqnroll.json` in the project root to enable the Allure plugin:
  ```json
  {
    "plugins": ["Allure.ReqnrollPlugin"]
  }
  ```

## Project Structure

- `Features/` - Gherkin feature files
- `StepDefinitions/` - Step definition classes
- `Pages/` - Page Object Model classes
- `Hooks/AllureHook.cs` - Attaches Playwright screenshots and videos to Allure reports
- `run-tests.local.ps1` - PowerShell script for local builds, tests, and report viewing
- `.github/workflows/playwright-tests.yml` - CI workflow for GitHub Actions

## Running Tests Locally

1. Install prerequisites (see above).
2. Ensure `reqnroll.json` is present in the project root (see above).
3. Open a PowerShell terminal in the project root.
4. Run:
   ```
   ./run-tests.local.ps1
   ```
5. The script will build, run smoke tests, install any missing browser dependencies, and open the Allure report in your browser.

## Running Tests in GitHub Actions

Tests are automatically run in CI using GitHub Actions. Here’s what happens:

1. On every push or pull request to the `main` branch, the workflow in `.github/workflows/playwright-tests.yml` is triggered.
2. The workflow steps:
   - Checks out the code.

- Sets up Node.js for the Allure CLI.
- Installs the local Playwright CLI tool.
- Restores, builds, and installs Playwright browsers on the runner.
- Runs the tests directly on `ubuntu-latest`.
- Collects Allure results, generates the report, uploads the raw results and HTML report as artifacts, and deploys the HTML report to GitHub Pages.

3. After the workflow completes, you can view the report from the GitHub Pages URL or download the artifacts from the run summary.

No manual action is needed—just push your code or open a pull request, and the tests will run automatically in CI.

If GitHub Pages is enabled for the repository, the report is published at the Pages URL shown in the workflow run.

## Allure Reporting

- Screenshots and videos are attached to failed scenarios via the `AllureHook`.
- The Allure plugin is enabled via `reqnroll.json` (not `allureConfig.json`).
- To view the report locally, run `allure open allure-report` after test execution, or let `run-tests.local.ps1` open it for you.

## Security

- Keep dependencies up to date for best security.
