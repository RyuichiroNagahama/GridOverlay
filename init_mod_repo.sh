#!/usr/bin/env bash
set -euo pipefail

# === ここを変えて実行してください ===
MOD_NAME="GridOverlay"                                    # フォルダ名 = リポジトリ名に使う
LOCAL_DIR="$HOME/Documents/cs2_mods/$MOD_NAME"         # ローカルのMODフォルダ
GITHUB_USER="RyuichiroNagahama"                        # あなたのGitHubユーザー名
VISIBILITY="public"                                   # public or private
# =====================================

cd "$LOCAL_DIR"

# 0) すでにGit管理ならスキップ
if [ -d .git ]; then
  echo "Repo already initialized: $(pwd)"
else
  git init
fi

# 1) .gitignore（Unity/CS2/ビルド物を除外）
cat > .gitignore <<'EOF'
# Build artifacts
bin/
obj/
Library/

# Local config / pid
*.pid

# Temp & backup
*.off
*.bak*
*.tmp

# Rider/VSCode/VS
.idea/
.vscode/
*.user
*.suo
*.userprefs
EOF

# 2) README（日本語/英語 併記の雛形）
if [ ! -f README.md ]; then
cat > README.md <<'EOF'
# GridOverlay — UIManager Registered UI (JP/EN)

## 方針 / Policy
- 反射や set_url 乗っ取りで既存 UIView を差し替えない。
- UIサービス（UIManager / Gameface）で自前のビューを登録・開閉する。
- HUD/シーン再生成に追従できる堅牢な設計。

## Hotkeys
- Shift+F2 … Toggle（表示/非表示）
- Shift+F12 / ESC … Revert（既定へ戻す）
- Shift+F1 … Debug Dump

---

## Policy (English)
- No reflection hacks / no overriding existing UIView via `set_url`.
- Register and control own view via the official/semi-official UI service.
- Robust against HUD/scene re-creation.

EOF
fi

# 3) 最初のコミット（JP/EN）
git add .
git commit -m "chore(init): 初期コミット / initial commit (repo bootstrap)"

# 4) GitHubに空のリポジトリを作る
#   ① GitHub CLI(gh) がある場合は自動作成
#   ② 無い場合は、Webで空リポジトリを作成してURLを貼る
if command -v gh >/dev/null 2>&1; then
  gh repo create "${GITHUB_USER}/${MOD_NAME}" --${VISIBILITY} --source=. --remote=origin --push
else
  # Webで作成する場合の案内だけ出して止める
  echo
  echo ">>> GitHub CLI(gh) が見つかりません。"
  echo "    先に GitHub 上で空のリポジトリを作成してください："
  echo "    https://github.com/new  （Repository name: ${MOD_NAME}, ${VISIBILITY})"
  echo "    作成後、続けて以下を実行："
  echo "      git remote add origin https://github.com/${GITHUB_USER}/${MOD_NAME}.git"
  echo "      git branch -M main"
  echo "      git push -u origin main"
  exit 0
fi

echo "✅ Done: https://github.com/${GITHUB_USER}/${MOD_NAME}"
