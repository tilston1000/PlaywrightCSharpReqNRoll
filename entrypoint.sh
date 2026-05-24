#!/bin/sh
set -e
dotnet test --verbosity normal
allure generate allure-results --clean -o allure-report
