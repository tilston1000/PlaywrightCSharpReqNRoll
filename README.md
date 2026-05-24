# PlaywrightReqnroll UI Test Project

This project contains automated UI tests using .NET, Playwright, Reqnroll, and Allure for reporting.

## Prerequisites

- .NET 9 SDK
- Node.js (for Playwright)
- Playwright CLI (`dotnet tool install --global Microsoft.Playwright.CLI`)
- Allure CLI (see https://docs.qameta.io/allure/)
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
- `run-tests.ps1` - PowerShell script to build, test, and generate Allure reports
- `.github/workflows/playwright-tests.yml` - CI workflow for GitHub Actions

## Running Tests Locally

1. Install prerequisites (see above).
2. Ensure `reqnroll.json` is present in the project root (see above).
3. Open a PowerShell terminal in the project root.
4. Run:
   ```
   ./run-tests.ps1
   ```
5. The script will build, run smoke tests, and open the Allure report in your browser.

## Running Tests in GitHub Actions

Tests are automatically run in CI using GitHub Actions. Here’s what happens:

1. On every push or pull request to the `main` branch, the workflow in `.github/workflows/playwright-tests.yml` is triggered.
2. The workflow steps:
   - Checks out the code.
   - Sets up Docker Buildx for building images.
   - Builds the Docker image for the test environment.
     - **Note:** The Dockerfile now copies all source and config files (including `appsettings.json`) before running `dotnet build` and `playwright install`. This ensures all required files are present for a successful build and browser installation.
     - `dotnet build` must run before `playwright install` to satisfy Playwright CLI requirements.
   - Scans the Docker image for vulnerabilities using Trivy.
   - Runs the tests inside the Docker container.
   - Uploads the Allure report as a workflow artifact.
3. After the workflow completes, you can download the Allure report artifact from the GitHub Actions run summary.

No manual action is needed—just push your code or open a pull request, and the tests will run automatically in CI.

## Allure Reporting

- Screenshots and videos are attached to failed scenarios via the `AllureHook`.
- The Allure plugin is enabled via `reqnroll.json` (not `allureConfig.json`).
- To view the report locally, run `allure open allure-report` after test execution.

## Security

- The CI pipeline scans the Docker image for vulnerabilities using Trivy.
- Keep dependencies up to date for best security.
