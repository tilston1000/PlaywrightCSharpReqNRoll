#!/bin/sh
# IMPORTANT: Ensure this file has executable permissions (chmod +x entrypoint.sh) on the host before running in Docker with a volume mount.
set -e

# Ensure allure-results directory always exists
mkdir -p allure-results
echo "Listing files in /app before test run:"
ls -l /app
echo "Listing files in /app/bin/Debug/net9.0 before test run:"
ls -l /app/bin/Debug/net9.0 || echo "/app/bin/Debug/net9.0 not found"
echo "Printing /app/reqnroll.json:"
cat /app/reqnroll.json || echo "/app/reqnroll.json not found"
echo "Running reqnroll test with diagnostic logging..."
reqnroll test --verbosity normal --diag:log.txt
echo "reqnroll test diagnostic log (first 100 lines):"
head -100 log.txt || echo "log.txt not found"
echo "Listing /app/allure-results after test run:"
ls -l /app/allure-results || echo "/app/allure-results not found"
allure generate allure-results --clean -o allure-report
