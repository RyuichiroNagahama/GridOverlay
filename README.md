# FpsMeter — UIManager Registered UI (JP/EN)

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

