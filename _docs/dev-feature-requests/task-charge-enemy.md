# Charge enemy: queued developer feature

**Status:** Designer approved the paper-shredder ART-054 sprite and explicitly authorized developer implementation on 2026-10-10. Approved art: `assets/art/approved/enemies/charge_enemy.png`.

## Behavior

This is a new enemy type with a straight-line, boar-like charge. Its approved appearance is a wheeled office paper shredder; its final display name is TBD.

1. **Approach:** Move toward Mohsen like a normal melee enemy, updating the pursuit direction as he moves.
2. **Charge trigger:** When Mohsen is within an editable `chargeRadius`, capture his position once as `targetDestination`. Calculate the direction from the enemy to that position and begin charging.
3. **Committed charge:** Move along that fixed direction toward `targetDestination`. Do not update the destination or direction when Mohsen moves away. Mohsen can dodge by moving after the charge begins. Stop the charge when the enemy reaches the captured destination; clamp the last movement step to avoid overshooting it.
4. **Recovery:** Stay in place for `stopDuration`, initially **3 seconds** and editable. After the pause, return to the approach state. A later charge again captures Mohsen's then-current position when he is within `chargeRadius`.

Keep the existing melee contact-damage rule unless the designer gives a different rule. Existing enemy HP, speed, knockback resistance, hit feedback, drops, and wave burst systems should remain reusable. `chargeRadius`, charge movement speed, and arrival tolerance need editable provisional values when development begins; only the 3-second recovery default is specified here. The designer will tune the values after running the game.

## Art first

The artist's V1 charging paper-shredder visual is approved at `assets/art/approved/enemies/charge_enemy.png` (160×112 transparent PNG). `_art/work/enemies/charge_enemy_game_scale_v1.png` is an illustrative movement preview, not an animation sheet. The developer may use code-driven lean/squash during charge and recovery; final animation method remains TBD.

## Developer handoff gate

The designer authorized the developer to implement this feature on 2026-10-10 after approving ART-054. The developer owns scenes, scripts, editable tuning, and technical/progress tracking. Do not run, build, compile, launch, or test project code; the designer handles game testing.
