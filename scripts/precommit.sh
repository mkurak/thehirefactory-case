#!/usr/bin/env bash
set -euo pipefail

echo "→ dotnet format (auto-fix)"
dotnet format ./source/TheHireFactory.ECommerce.sln

echo "→ git add (formatted files)"
git add -A

echo "→ dotnet build (sanity check)"
dotnet build ./source/TheHireFactory.ECommerce.sln -c Release --nologo