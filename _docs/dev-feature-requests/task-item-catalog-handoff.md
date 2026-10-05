# Item catalog implementation and audit

**Designer authorization:** 2026-10-06. The designer explicitly requested that the developer implement the current `_docs/items-db.md` and verify that every active item resource matches it. This task may start in the Developer conversation. `_docs/items-db.md` is the authoritative item contract for this work; the shared PRD summarizes the same decisions.

## Resource audit

- Audit every one of the **26 active passive items** against the database table: display name, role description, all stat modifiers and amounts, coin cost, rarity, uniqueness, and approved icon or placeholder path. Fix mismatches in resource files and shop pool references.
- Rename Arezoon to **Arezoo** and Mana to **Mania** in displayed names and resource paths/references. Arezoo costs 45 coins, has rarity 3, is unique, and keeps +6 knockback and +2 max HP while gaining Spread +3.
- Remove the temporary `+5% damage` sample item from the active shop pool. It is no longer in the database table. Remove or retain its unused resource according to normal project cleanup; it must not be offered.
- Keep existing designer-approved art paths. Use placeholders for items without approved icons.

## Shop rules

- Each of the four shop slots independently chooses a weapon with **40%** probability or a passive item with **60%** probability. A weapon is never guaranteed. If one category has no eligible offers, use the other category.
- All rarities 1–3 are eligible from the first shop. Among eligible passive items, use the exact per-item weights in `_docs/items-db.md`, based on the completed wave number. Preserve the existing weapon tier rules unless they conflict with this task.
- `Unique=true` allows one copy per run. Once owned, exclude it from new offers and invalidate any other visible or locked copy. Every `Unique=false` item remains repeatable, with additive stat stacking.
- Preserve the existing rule that an HP-reducing item cannot be offered or bought when it would reduce current HP to zero or below.
- Keep the signed HP-regeneration total stored, but use `max(0, stored regeneration)` for actual healing ticks. Negative regeneration never damages Mohsen; later positive modifiers can offset it.

## Arezoo Spread +3

- A player bullet or shotgun pellet that hits an enemy emits **three** evenly spaced bullets from the hit location.
- Each child copies the original projectile's type and gameplay/visual traits, including resolved damage and critical hit, knockback, speed, lifetime/range, collision behavior, and sprite. Its travel direction is one of the three radial directions.
- The enemy struck by the original projectile is ignored by those three children for their entire lifetime. Children may hit other enemies.
- Child bullets do **not** emit further spread bullets on their own hits. Knife throws and hostile bullets are not player bullets for this rule.

## Handoff record

Update `_docs/tech-features.md` with the implementation details and `_docs/developer-progress.md` with code-written status, open checks, and a short designer manual-test checklist. The developer does not run, build, compile, launch, or test project code. The designer verifies the implementation by running the game.
