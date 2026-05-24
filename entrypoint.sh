#!/bin/sh
# IMPORTANT: Ensure this file has executable permissions (chmod +x entrypoint.sh) on the host before running in Docker with a volume mount.
set -e

# Ensure allure-results directory always exists
mkdir -p allure-results
dotnet test --verbosity normal
allure generate allure-results --clean -o allure-report
