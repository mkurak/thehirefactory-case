#!/usr/bin/env bash
set -euo pipefail

echo "→ dotnet format"
dotnet format ./source/TheHireFactory.ECommerce.sln --verify-no-changes

echo "→ dotnet build (Release)"
dotnet build ./source/TheHireFactory.ECommerce.sln -c Release --nologo