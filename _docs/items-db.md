# Passive items database

**Status:** Designer-approved developer handoff, 2026-10-06. This file is the source of truth for the current passive-item catalog, costs, rarity, uniqueness, stat effects, and shop-item selection rules. The designer requested a resource-by-resource audit against this file. Artist approval remains separate.

This catalog contains teammate and themed passive items. Descriptions state each person's role for now; the designer will provide further art references later. Cost is in whole coins. The designer increased every item cost to **1.75× its earlier catalog price**, rounding fractional results **up**; later explicit changes set Behzad to 80, Arsalan to 95, and Mania to 75 coins. Rarity is **1 (common), 2 (uncommon), or 3 (rare)**; higher values mean less frequent offers. **Unique** means the player may own at most one copy in a run; owned unique items are excluded from later offers, and any other visible or locked copy becomes unavailable after purchase. Non-unique items may be bought repeatedly, with their modifiers stacking additively. Movement speed is flat px/s, HP regeneration is HP/s, knockback is in pixels, damage bonuses and critical chance are percentage points, and bullet pierce is a count of additional enemies hit. The current player starts at 10 max HP, 240 px/s movement speed, and 0 HP/s regeneration. Hossein and Saeed include the designer's latest changes.

## Item catalog

| Item | Description | Proposed stat changes | Cost | Rarity | Unique |
| --- | --- | --- | ---: | ---: | --- |
| Pooria | Developer. | +12% bonus damage | 14 | 2 | false |
| Narges | Developer. | +6% bonus damage; +0.30 HP regeneration/s | 9 | 1 | false |
| Behzad | Developer. | +200% bonus damage; −10 max HP | 80 | 3 | false |
| Sara | Developer. | +10% bonus damage; +1 knockback | 13 | 1 | false |
| Milad | Developer. | +14% bonus damage | 14 | 2 | false |
| Saman | Developer. | +10% bonus damage; +2 knockback | 13 | 2 | false |
| Arsalan | Developer. | +25 percentage points critical chance; −10 max HP | 95 | 3 | false |
| Mohammad Hossein | Developer. | +12% bonus damage | 14 | 1 | false |
| Amir Hossein | Developer. | +14% bonus damage; +1 knockback | 20 | 2 | false |
| Hojat | Developer. | +10% bonus damage; +1 knockback; +5 percentage points critical chance | 16 | 2 | false |
| Omid | Developer. | +12% bonus damage | 14 | 1 | false |
| Hossein | Product manager. | +85 movement speed; +5 max HP; −3 HP regeneration/s; −15% bonus damage | 70 | 3 | true |
| Saeed | Product manager. | Bullet pierce +2 | 70 | 3 | true |
| Arezoo | Product manager. | +6 knockback; +2 max HP; Spread +3 | 79 | 3 | true |
| Mania | Product manager. | +100% projectile size; +100 knockback; you cant shoot while moving | 75 | 3 | false |
| Sahar | QA. | +3 max HP; −8 movement speed | 16 | 1 | false |
| Sahel | QA. | +4 max HP; −12 movement speed | 20 | 1 | false |
| Sepideh | QA. | +5 max HP; −16 movement speed | 23 | 1 | false |
| Sahar-2 | QA. | +25 movement speed; −0.60 HP regeneration/s | 28 | 1 | false |
| Maryam | Agile manager. | +9 max HP; +0.35 HP regeneration/s; −35 movement speed | 39 | 2 | false |
| `devops-buddy-1` | DevOps teammate. | +12% ranged damage | 14 | 1 | false |
| `devops-buddy-2` | DevOps teammate. | +16% ranged damage | 20 | 2 | false |
| `devops-buddy-3` | DevOps teammate. | +20% ranged damage | 25 | 3 | false |
| Hookah | Item. | +5 movement speed; −5 max HP; +2 knockback | 13 | 2 | false |
| Propranolol pills | Item. | −15 movement speed; +10 HP regeneration/s | 56 | 3 | false |
| Heavy Smoker | Trait. | +27% bonus damage; −15 max HP | 42 | 3 | false |
| Bluetooth headset | Item. | +5 percentage points critical chance | 25 | 2 | TBD |
| Strawberry milkshake | item | +10 max HP | 20 | 1 | TBD |
| Pizza | Item. | +20 max HP | 20 | 2 | TBD |
| Hamburger | Item. | +10 movement speed; +10 max HP | 20 | 2 | TBD |
| Team Building | Item. | −5 max HP; +2 HP regeneration/s | 20 | 2 | TBD |
| Tea | Item. | +15 movement speed; −2 HP regeneration/s | 20 | 2 | TBD |
| Beach straw hat | Item. | +5 percentage points critical chance | 20 | 2 | TBD |
| Apple | Item. | +0 max HP; +0.05 HP regeneration/s | 20 | 2 | TBD |

**Developer damage revision (2026-10-11):** Direct bonus-damage modifiers on Developer items and ranged-damage modifiers on the three DevOps buddies are doubled from their prior values. Critical chance, knockback, HP, regeneration, prices, rarity, and uniqueness are unchanged. Arsalan has no direct damage modifier, so his +25 percentage-point critical chance is unchanged. Hojat's +5 percentage-point critical chance is also unchanged.

**Six item icons handoff (2026-10-11):** Designer approved Pizza, Hamburger, Team Building, Tea, Beach straw hat, and Apple V1 icons at `assets/art/approved/items/{pizza,hamburger,team_building,tea,beach_straw_hat,apple}.png`. Each has rarity 2 and a final cost of 20 coins; this explicit price is not multiplied by the earlier 1.75× adjustment. HP bonuses and penalties map to max HP, movement speed uses flat px/s, and HP regeneration is HP/s. The apple's +0 HP is recorded literally and has no HP effect. The beach straw hat's +5% critical chance follows the existing percentage-point stacking rule. Uniqueness was not specified and remains TBD; implementation defaults should be marked provisional.

**Bluetooth headset handoff (2026-10-11):** Designer approved the icon and specified +5% critical chance, rarity 2, and cost 25 coins. Interpret the bonus as +5 percentage points to the equipped weapon's critical-hit chance, following the existing Arsalan critical-chance rule. Approved icon: `assets/art/approved/items/bluetooth_headset.png`, transparent 128×128 PNG. Uniqueness was not specified and remains TBD; any implementation default should be recorded as provisional.

**Mania revision (2026-10-11):** Replaces the former +8 max HP and −1 knockback. +100% projectile size doubles the visible size and hit radius of player knife throws, bullets, and shotgun pellets; repeated non-unique copies add another 100 percentage points each. +100 knockback stacks normally. The shop card displays the movement restriction as red text: `You cant shoot while moving.` It blocks all equipped weapon attacks while a movement direction is pressed, and firing resumes when movement stops. A later designer change raised Mania's knockback from +5 to +100 and price from 44 to 75; rarity 3 and non-unique status stay unchanged.

**Strawberry milkshake handoff (2026-10-10):** Designer requested a new passive item with description exactly `item`, +10 HP (mapped to max HP in this passive catalog), rarity 1, and final cost 20 coins. This later explicit price is not subject to the earlier 1.75× increase. Approved icon: `assets/art/approved/objects/strawberry_milkshake.png`, transparent 48×48 PNG. Uniqueness was not specified; record any implementation default as provisional.

**Developer rename note:** Rename **Arezoon → Arezoo** and **Mana → Mania** in item display names and corresponding resource names/references. The old names must not remain in the shop pool.

**Spread +3 behavior:** When a player bullet **or shotgun pellet** hits an enemy, spawn three additional bullets from the hit point in evenly spaced directions around a circle. Each spawned bullet inherits all projectile traits of the bullet or pellet that hit, including its type, damage, critical-hit result, knockback, speed, lifetime/range, collision behavior, and visual. The enemy struck by the original bullet or pellet must be immune to those three new bullets for their entire lifetime. **Spread bullets do not trigger further Spread** when they hit another enemy.

**Bullet pierce +2 behavior:** Saeed replaces his previous movement and knockback modifiers and is unique, so only one copy can be purchased per run. A pistol bullet, SMG bullet, or shotgun pellet hits its first enemy and continues through **two more distinct enemies** before it is destroyed. Each bullet damages a given enemy at most once. Spread children inherit the player's pierce capacity; knives and hostile red envelopes do not gain this effect.

**Critical chance behavior:** Arsalan adds 25 percentage points to the equipped weapon's own critical-hit chance. Repeat purchases stack additively; actual hit probability is clamped to 100%. Knife, pistol, SMG, and each shotgun pellet use the resulting chance and retain their weapon-tier critical-damage multipliers. Critical damage numbers are yellow with a black outline; normal damage numbers remain white.

**Negative HP regeneration:** Store the full signed regeneration stat, including negative values. When healing is calculated during a wave, use `max(0, stored HP regeneration)` so a negative value produces no healing and never drains HP. Later positive modifiers still offset the stored negative value.

**Unsafe HP purchases:** An item with a max-HP penalty can still appear in the shop when buying it would reduce Mohsen's current or maximum HP to zero or below. The Buy button remains available, but the purchase is refused before coins are deducted and Hamidreza explains the HP risk. The item remains offered so it can be bought later if Mohsen gains enough HP.

**Names and art:** Arezoo and Mania are the current display spellings. Twenty teammate portraits are approved in `assets/art/approved/items/teammates/`, including the newly approved Saeed, Arezoo, Sahar-2, Sepideh, Mania, and Hojat; items without approved portraits use the developer placeholder. The three DevOps buddies share `assets/art/approved/items/devops/yml_file.png`. Hookah, Propranolol pills, Heavy Smoker, Bluetooth headset, Pizza, Hamburger, Team Building, Tea, Beach straw hat, and Apple have approved icons in `assets/art/approved/items/`. Heavy Smoker's role is **Trait**. The temporary +5% damage sample is removed from the shop pool and is no longer part of this catalog.

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
