# Passive items database

**Status:** Designer-approved developer handoff, 2026-10-06. This file is the source of truth for the current passive-item catalog, costs, rarity, uniqueness, stat effects, and shop-item selection rules. The designer requested a resource-by-resource audit against this file. Artist approval remains separate.

This catalog contains teammate and themed passive items. Descriptions state each person's role for now; the designer will provide further art references later. Cost is in coins. Rarity is **1 (common), 2 (uncommon), or 3 (rare)**; higher values mean less frequent offers. **Unique** means the player may own at most one copy in a run; owned unique items are excluded from later offers, and any other visible or locked copy becomes unavailable after purchase. Non-unique items may be bought repeatedly, with their modifiers stacking additively. Movement speed is flat px/s, HP regeneration is HP/s, knockback is in pixels, damage bonuses are percentage points, and bullet pierce is a count of additional enemies hit. The current player starts at 10 max HP, 240 px/s movement speed, and 0 HP/s regeneration. Hossein and Saeed include the designer's latest changes.

## Item catalog

| Item | Description | Proposed stat changes | Cost | Rarity | Unique |
| --- | --- | --- | ---: | ---: | --- |
| Pooria | Developer. | +6% bonus damage | 8 | 2 | false |
| Narges | Developer. | +3% bonus damage; +0.30 HP regeneration/s | 5 | 1 | false |
| Behzad | Developer. | +15% bonus damage; −4 max HP | 13 | 3 | false |
| Sara | Developer. | +5% bonus damage; +1 knockback | 7 | 1 | false |
| Milad | Developer. | +7% bonus damage | 8 | 2 | false |
| Saman | Developer. | +5% bonus damage; +2 knockback | 7 | 2 | false |
| Arsalan | Developer. | +13% bonus damage; −3 max HP | 14 | 3 | false |
| Mohammad Hossein | Developer. | +6% bonus damage | 8 | 1 | false |
| Amir Hossein | Developer. | +7% bonus damage; +1 knockback | 11 | 2 | false |
| Hojat | Developer. | +5% bonus damage; +1 knockback | 9 | 2 | false |
| Omid | Developer. | +6% bonus damage | 8 | 1 | false |
| Hossein | Product manager. | +85 movement speed; +5 max HP; −3 HP regeneration/s; −15% bonus damage | 40 | 3 | true |
| Saeed | Product manager. | Bullet pierce +2 | 40 | 3 | true |
| Arezoo | Product manager. | +6 knockback; +2 max HP; Spread +3 | 45 | 3 | true |
| Mania | Product manager. | +8 max HP; −1 knockback | 25 | 3 | false |
| Sahar | QA. | +3 max HP; −8 movement speed | 9 | 1 | false |
| Sahel | QA. | +4 max HP; −12 movement speed | 11 | 1 | false |
| Sepideh | QA. | +5 max HP; −16 movement speed | 13 | 1 | false |
| Sahar-2 | QA. | +25 movement speed; −0.60 HP regeneration/s | 16 | 1 | false |
| Maryam | Agile manager. | +9 max HP; +0.35 HP regeneration/s; −35 movement speed | 22 | 2 | false |
| `devops-buddy-1` | DevOps teammate. | +6% ranged damage | 8 | 1 | false |
| `devops-buddy-2` | DevOps teammate. | +8% ranged damage | 11 | 2 | false |
| `devops-buddy-3` | DevOps teammate. | +10% ranged damage | 14 | 3 | false |
| Hookah | Item. | +5 movement speed; −5 max HP; +2 knockback | 7 | 2 | false |
| Propranolol pills | Item. | −15 movement speed; +10 HP regeneration/s | 32 | 3 | false |
| Heavy Smoker | Trait. | +27% bonus damage; −15 max HP | 24 | 3 | false |

**Developer rename note:** Rename **Arezoon → Arezoo** and **Mana → Mania** in item display names and corresponding resource names/references. The old names must not remain in the shop pool.

**Spread +3 behavior:** When a player bullet **or shotgun pellet** hits an enemy, spawn three additional bullets from the hit point in evenly spaced directions around a circle. Each spawned bullet inherits all projectile traits of the bullet or pellet that hit, including its type, damage, critical-hit result, knockback, speed, lifetime/range, collision behavior, and visual. The enemy struck by the original bullet or pellet must be immune to those three new bullets for their entire lifetime. **Spread bullets do not trigger further Spread** when they hit another enemy.

**Bullet pierce +2 behavior:** Saeed replaces his previous movement and knockback modifiers and is unique, so only one copy can be purchased per run. A pistol bullet, SMG bullet, or shotgun pellet hits its first enemy and continues through **two more distinct enemies** before it is destroyed. Each bullet damages a given enemy at most once. Spread children inherit the player's pierce capacity; knives and hostile red envelopes do not gain this effect.

**Negative HP regeneration:** Store the full signed regeneration stat, including negative values. When healing is calculated during a wave, use `max(0, stored HP regeneration)` so a negative value produces no healing and never drains HP. Later positive modifiers still offset the stored negative value.

**Names and art:** Arezoo and Mania are the current display spellings. Fourteen teammate portraits are approved in `assets/art/approved/items/teammates/`; items without approved portraits use the developer placeholder. The three DevOps buddies share `assets/art/approved/items/devops/yml_file.png`. Hookah, Propranolol pills, and Heavy Smoker have approved icons at `assets/art/approved/items/hookah.png`, `assets/art/approved/items/propranolol_pills.png`, and `assets/art/approved/items/heavy_smoker.png`. Heavy Smoker's role is **Trait**. The temporary +5% damage sample is removed from the shop pool and is no longer part of this catalog.

## Shop offer selection and wave-based rarity weighting

Each of the four shop slots independently chooses **weapon (40%)** or **passive item (60%)**. No shop is guaranteed a weapon. All three item rarities are eligible starting with the first shop after wave 1. The player gets a better chance of seeing rarer passive items as waves progress. For an eligible item of rarity `r` offered after wave `w` (`w` from 1 to 20), use these relative weights:

- Rarity 1: `weight(1, w) = 12`
- Rarity 2: `weight(2, w) = 4 + 0.25 × (w − 1)`
- Rarity 3: `weight(3, w) = 1 + 0.25 × (w − 1)`

Choose an eligible passive item with probability `its weight / sum of weights of all eligible passive items`. At wave 1, per-item weights are **12 : 4 : 1** for rarities 1 : 2 : 3; at wave 5 they are **12 : 5 : 2**; at wave 20 they are **12 : 8.75 : 5.75**. Rarity 3 stays less likely than rarity 2, which stays less likely than rarity 1, while both rarer groups become more available over time. If a chosen category has no eligible offers, use the other category rather than creating an empty slot. Existing weapon tier rules remain in effect.

## Remaining design follow-ups

- Balance these approved starting values after designer playtests. Do not treat a future tuning request as part of this handoff.
- Confirm additional future passive items and art as individual designer tasks.

**Handoff:** The designer explicitly authorized this revision for developer implementation on 2026-10-06. The developer should audit every item resource against the table, remove the sample from the active pool, update offer selection, and record implementation progress separately from designer verification.
