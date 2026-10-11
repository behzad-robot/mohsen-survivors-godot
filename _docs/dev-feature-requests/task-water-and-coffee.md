# Water dispensers, healing feedback, and coffee pickups

Designer requested developer implementation and approved ART-050–052 on 2026-10-10. Implementation and designer verification are pending.

## Approved assets

| Asset | Approved path | Canvas |
| --- | --- | --- |
| Water dispenser | `assets/art/approved/objects/water_dispenser.png` | 160×104 transparent PNG |
| Coffee cup | `assets/art/approved/objects/coffee_cup.png` | 48×48 transparent PNG |
| Glass of water | `assets/art/approved/objects/glass_of_water.png` | 48×48 transparent PNG |

## Designer requirements

- Spawn 1–3 water dispensers at the start of every wave except wave 1.
- Each dispenser drops one glass of water every 10 seconds at a random location around itself. Give the drop a pleasing animation; the developer can implement a short outward arc and landing bounce with the static approved sprite.
- Collecting a glass of water heals the player by 3 HP.
- Whenever the player heals, including HP regeneration and glass-of-water collection, show a green floating text with the heal value around Mohsen, similar to enemy damage-number feedback.
- Each defeated enemy has a 10% chance to drop a coffee cup. This is an added pickup drop and does not replace the existing coin rules.
- Collecting coffee grants a temporary movement-speed bonus of +50% of current movement speed for 5 seconds.

## Engineering details to record as provisional

The designer has not specified dispenser placement distribution, drop radius, dispenser collision/destructibility, pickup attraction/lifetime, wave cleanup, regeneration text batching, or repeated coffee pickup stacking/refresh rules. Choose compatible implementation details where needed and document them in the technical plan without treating them as designer-approved tuning. Apply healing through the existing health model; display actual healing received and avoid text for zero healing. Avoid repeated coffee multiplication or leaving a permanent bonus when its timer ends.

Keep tunable values editable. Follow the standing no-run/no-build/no-compile/no-launch/no-test rule; the designer runs and verifies the game.
