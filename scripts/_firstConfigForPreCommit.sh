#!/usr/bin/env bash

# eğer .git dizini yoksa çık
if [ ! -d ".git" ]; then
  echo "Bu betik, projenin kök dizininde çalıştırılmalıdır."
  exit 1
fi

# kök dizini bul ve .git/hooks dizinini oluştur
cd $(git rev-parse --show-toplevel)
mkdir -p .git/hooks

cat > .git/hooks/pre-commit << 'SH'
#!/usr/bin/env bash
set -euo pipefail
repo_root="$(git rev-parse --show-toplevel)"
bash "$repo_root/scripts/precommit.sh"
SH

chmod +x .git/hooks/pre-commit

dotnet format ./source/TheHireFactory.ECommerce.sln

echo "→ Ön yapılandırma tamamlandı. Artık 'git commit' yapabilirsiniz."