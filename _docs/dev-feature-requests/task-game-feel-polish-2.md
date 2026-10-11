# Game feel polish: designer-selected backlog

**Designer handoff:** 2026-10-11. The designer selected items 1–6 from the current project polish review. Item 7, replacing the placeholder sound palette, is excluded. These requests are queued for the developer; the designer will say when to start work on items in this folder, per this folder's `AGENTS.md`.

Use the existing systems and approved assets where possible. Keep provisional timings and intensities editable. Do not change wave duration, combat balance, economy, or the designer's approved art without a separate decision. Record completed work in `_docs/developer-progress.md` and technical tuning in `_docs/tech-features.md`. The designer runs and verifies the game; agents must not run, build, compile, launch, or test it.

## 1. Spawn warning countdown

Animate each existing red-cross marker through its configured warning duration so the imminent spawn is easy to read. A pulse and/or contracting ring should clearly end at the actual spawn time. Preserve the selected spawn position and per-burst warning duration. Keep it readable when many warnings overlap and use a developer visual effect or placeholder until approved art is available.

## 2. Ordinary ranged-enemy firing tell

Give the standard ranged enemy a short, visible anticipation before its projectile appears, such as a windup tint or muzzle flash. The projectile should launch only after the tell. Keep the existing boss and charger telegraphs distinct. Make timing configurable and preserve the existing range/retreat behavior and attack cadence as closely as possible; document any timing change.

## 3. Mohsen damage feedback

When Mohsen actually loses HP, add a brief punch to the HUD HP bar and a subtle red tint at the screen edges. Coordinate these with the existing sprite flash and hit sound. Do not trigger them on blocked hits during invulnerability, healing, or shop stat changes. Keep the effect short, readable, and easy to reduce or disable in the Inspector.

## 4. Weapon-merge feedback in shop

When a weapon purchase automatically upgrades an equipped weapon, show a distinct tier-up animation on the purchased offer and the equipped weapon slot. Make the new tier immediately legible. Ordinary purchases that do not merge should keep their current feedback. The effect must not interrupt additional purchases, rerolls, locks, or Next Wave.

## 5. Wave opening and clear beats

Add a brief, clear wave-start banner and a compact wave-cleared moment before opening the shop or completion screen. Keep the configured active wave duration unchanged; decide and document whether the opening beat freezes play or overlays active play. Existing warnings, enemies, pickups, and temporary effects must still be cleared at the correct transition. Keep the treatment concise enough for repeated ten-wave play.

## 6. Coin-collection rhythm

On actual coin collection, make the HUD coin count bounce briefly. Quick consecutive collections may raise the coin cue's pitch slightly, then reset after a short gap. Work with the existing spring/magnet movement and audio throttling so large coin clusters remain pleasant and legible. Do not change coin value, pickup radius, or drop rates.

## Visual review note

The earlier static-review suspicion that `Enemy._Process` hides the charger's squash is **not a confirmed bug**: `ChargeEnemy._Process` calls the base method first and then applies its own scale for that frame. The designer can report if the charge/recovery silhouette still looks too weak in play; no fix is requested on that suspicion alone.
