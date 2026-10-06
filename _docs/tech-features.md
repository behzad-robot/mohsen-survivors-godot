# Technical features and implementation notes

**Owner:** developer agent. **Source:** `prd.md` and the designer's notes. This document records implementation plans and technical details that the PRD does not need to specify. It does not authorize starting implementation or making gameplay design choices.

Track implementation status and designer verification separately in `developer-progress.md`. Update that file after each milestone so later developer turns know what was written, what remains, and what the designer has actually tested.

## Existing project context

- A Godot project already exists at the repository root, with a C# project file and solution. Inspect actual project files before choosing scene or script conventions.
- The designer runs and tests the project. The developer writes code and supplies concise manual test steps for the designer, without running, building, compiling, or launching it.
- C# is the default scripting language. Use GDScript only after identifying a required capability unavailable in the Godot C# API; document that limitation here before adding a GDScript file. Editor tooling should also use C# where supported.

## Platform direction

The current target is native desktop using the existing Godot 4.6 project and predominantly C# scripts. Web export is deferred by the designer and does not block implementation. If the web goal returns, revisit compatibility: [Godot 4.6's feature list](https://docs.godotengine.org/en/4.6/about/list_of_features.html) says Godot 4 C# projects cannot export to web, and [its renderer documentation](https://docs.godotengine.org/en/4.6/tutorials/rendering/renderers.html) says web requires the Compatibility renderer. The current project uses the Mobile renderer. Do not change the engine, language, or renderer solely for a deferred web goal.

## Planned project layout

| Path | Use |
| --- | --- |
| `scenes/` | Godot scenes, grouped by gameplay area or UI once implementation begins |
| `scripts/` | C# scripts, grouped by feature once implementation begins |
| `resources/` | Godot data resources when needed |
| `assets/placeholders/` | Developer-made temporary visuals for missing art |
| `assets/art/approved/` | Designer-approved art listed in `art-requirements.md` |

Create feature subfolders only when there is a feature task. Keep asset references centralized enough to replace a placeholder with the approved path without changing unrelated logic.

## Feature planning queue

| Feature | Current state | Technical detail to settle before implementation |
| --- | --- | --- |
| Player movement and combat | Code written; designer verification pending | Top-down movement, WASD/arrows, nearest-target auto-fire, 10 HP, 1 damage per hit, configurable 1 s default invulnerability and flash |
| Enemies and spawning | Code written; designer verification pending | Reusable melee/ranged scenes; timed bursts begin red-cross warnings, then spawn after per-burst warning duration |
| Weapons | Code written; designer verification pending | Initial knife/pistol/SMG choice, four slots, four tiers, nearest-target auto-fire; scaling formulas TBD |
| Passive items | Code written for the 26-item `_docs/items-db.md` catalog; designer verification pending | Prices, rarity, uniqueness, and modifiers are editable; approved icons are used where available, placeholders for the rest |
| Waves and progression | Code written for waves 1–10; designer verification pending | Ten editable 45-second waves; 2.5× enemy HP per wave from wave 4, tank base HP 2.5× normal; full target 20 waves |
| UI and art integration | Placeholder code written; designer verification pending | Play button, weapon selection, shop, shared finish-screen layout, 1280×720 design view; exact copy and window scaling TBD |

## Confirmed input and presentation requirements

- Native desktop is the initial target; web export is deferred and mobile support is out of scope.
- Movement accepts both WASD and arrow keys. Weapons fire automatically at the nearest enemy.
- Provide a pre-play selection of exactly one starting weapon: knife, pistol, or SMG.
- The initial knife, pistol, and SMG values come from the four-tier wiki table below; their shared damage scaling formula remains TBD.
- Use a 1280×720 design view on desktop. Window scaling behavior remains to be selected.
- Use a top-down 2D rectangular arena larger than the camera view, with walls that keep Mohsen inside.
- Weapons fire automatically. The player can equip up to four weapons.
- Each wave lasts 45 seconds under the designer's 2026-10-04 update. Spawn and difficulty parameters should be configurable rather than hardcoded into one wave.
- After a wave, show four random weapon/item offers and a reroll button. An offer set may contain no weapon. Coins from enemy drops pay for purchases.
- Mohsen starts at 10 HP and takes 1 damage per enemy hit, then enters an editor-configurable invulnerable period, default 1 second, with a flashing sprite.

## Wave and enemy authoring plan

Prefer inspector-editable C# custom `Resource` types saved as `.tres` files rather than raw JSON, so the designer can author and inspect waves in Godot. Godot supports [C# global custom Resources](https://docs.godotengine.org/en/4.6/tutorials/scripting/c_sharp/c_sharp_global_classes.html) and [exported typed Resource arrays](https://docs.godotengine.org/en/4.6/tutorials/scripting/c_sharp/c_sharp_exports.html). Proposed structure:

| Resource | Exported data | Purpose |
| --- | --- | --- |
| `WaveDefinition` | Wave number, optional wave name, duration (45 s by default), enemy HP multiplier, ordered list of `SpawnBurstDefinition` | One file per wave; waves 1–10 currently authored, with capacity for 20 later |
| `SpawnBurstDefinition` | Trigger time in seconds since wave start, warning duration (default 1 s), list of `EnemySpawnGroupDefinition` | At t=x, choose positions and show warnings; spawn after the duration |
| `EnemySpawnGroupDefinition` | Enemy scene/variant, count, HP multiplier, speed multiplier, knockback resistance adjustment | Allows different enemy groups within one burst |

Example: at t=0, one melee group with count 2 and one ranged group with count 3 begins warning; they spawn after that burst's warning duration. At t=1, two groups of count 5 each begin their own warning. Each enemy instance receives the resolved values at spawn so one burst cannot mutate shared scene defaults. The developer should validate nonnegative counts, sensible stat limits, and trigger time plus warning duration within the 45-second wave when authoring tools are implemented. Provisional, editable schedules and enemy values now cover waves 1–10; the designer will later tune them and expand to waves 11–20.

At each burst trigger, randomly choose legal positions for all enemies in that burst, show a red cross at each position, and hold those positions through the warning period. After the burst's Inspector-editable duration (default 1 second), remove the crosses and spawn the enemies at those same positions. Use a red-cross developer placeholder until ART-017 is approved. A t=0 burst begins warning at wave start and spawns at t=1 by default.

Use two reusable behavior scenes or scripts, melee and ranged, with inspector-exposed baseline HP, movement speed, and knockback resistance. Ranged enemies also expose fire range, too-close retreat distance, attack interval, and projectile settings. Enemy scene variants may swap sprites and baseline values without duplicating behavior code. A designer-selected scene variant is referenced by each spawn group.

## Player, equipment, and shop plan

- Player scene exports coin pickup radius and invulnerability duration; default invulnerability is 1 second. Pickup radius must be larger than the body collider. Flash the player sprite during invulnerability. HP regeneration is a float amount per second during waves and cannot exceed max HP; refill HP at the start of a new wave.
- Initial player stats: max/current HP (start 10), flat movement speed, knockback value, bonus damage %, ranged bonus damage %, melee bonus damage %, HP regeneration per second, and Spread. Item modifiers can add positive or negative values; current additive stacking follows `_docs/items-db.md`.
- Weapons target the nearest enemy. Knife travels toward that target; its hitbox is active only during the attack and can hit enemies along the path. Pistol/SMG use ranged projectiles. Keep weapon tier data separate from attack code.
- Shop has four independent offer slots. Reroll only unlocked slots; locked offers persist into later shops until unlocked or bought. Buying leaves a slot empty until reroll or the next shop. Successive rerolls cost 5, 10, 15, 20, etc.; whether the cost resets each shop visit is TBD.
- The active shop offer pool consists of knife, pistol, SMG, shotgun, and the 26 passives in `_docs/items-db.md`. The temporary +5% sample is inactive. Approved icons are used where available, with developer placeholders for the rest. The catalog's prices, rarity, uniqueness, and stacking rules are saved in item resources.
- Block purchase of a new weapon when all four slots are full. Permit buying an identical same-tier weapon that immediately merges with an equipped copy to free or preserve a slot. Define handling for duplicates with free slots and for tier IV before implementation. Coins left on the ground at wave end disappear.
- Use a shared finish-screen scene or component with parameterized title and illustration, yielding distinct victory and defeat screens, each with Play Again at the bottom.

## Art integration and size

The during-wave HUD is an editable `scenes/ui/hud.tscn` scene. `GameController` binds HP, coins, wave title, timer, and existing equipment summary to individual controls. Wave names live in `resources/waves/wave_01.tres`–`wave_10.tres` through `WaveDefinition.WaveName`; an empty or whitespace-only name displays `Wave-{X}`. Waves 6–10 have no approved names yet. Timer color uses the displayed, upward-rounded seconds: white at 20 or more, yellow from 10 through 19, and red below 10. The HP bar uses editable flat styles, and the coin icon reuses the developer placeholder until approved art arrives. These layout and color values need a designer run for visual verification.

- Start with placeholders approximately matching the saved gameplay screenshots: Mohsen visual body about 55–65 display pixels tall and ordinary enemies about 45–60 at 1280×720. These are display targets, not fixed source-image dimensions or collider sizes. Keep sprite scale and collision geometry independently editable.
- The approved no-shoes `assets/art/approved/characters/mohsen_idle.png` is a static 64×64 image with 60-pixel visible height. `mohsen_walk.png` is still pending; keep the approved idle visible during movement with the existing subtle code-driven bob.
- Mohsen has idle/walk art only. Implement post-hit flash through sprite modulation or equivalent transparency changes and death by shrinking to zero scale. Weapons are independent floating visuals. Enemy walk can use sprites or code-driven scaling for later visual variants.
- The first arena needs a floor/background and boundary walls, without props. Use default Godot controls for Play, selection, shop, and HUD until the designer requests custom UI. Keep victory and defeat image slots replaceable because their artwork and copy will arrive later.
- `_art/reference-images/` and `_art/mohsen-refrence-images/` are references only and must not be referenced by scenes or included as gameplay assets. The artist delivers one approved task at a time; continue development with placeholders.
- Read `_agents/artist.md` and the full backlog in `art-requirements.md` to anticipate approved file paths. The artist still begins only the task assigned by the designer. ART-017 will replace the spawn-warning placeholder after approval.

## Approved starting weapon tier references

The designer chose the four-tier damage, attack interval, and crit values from the [Knife](https://brotato.wiki.spellsandguns.com/Knife), [Pistol](https://brotato.wiki.spellsandguns.com/Pistol), and [SMG](https://brotato.wiki.spellsandguns.com/SMG) Brotato Wiki pages as the initial balance. Pistol and SMG range still use those values; knife range is a provisional longer value requested later. Knockback values in the table below are provisional developer choices following the designer’s later instruction to make knife stronger and guns weaker. Damage scaling percentages in those pages refer to Brotato's flat melee/ranged damage stats; this game's player bonuses are percentages, so the translation formula needs a designer decision before coding. The exact values may be tuned later.

| Weapon | Tier | Base damage (wiki scaling) | Attack interval | Range | Crit chance / multiplier | Knockback |
| --- | --- | --- | --- | --- | --- | ---: |
| Knife | I | 9 (80% melee) | 1.01 s | 280 | 20% / 2.5× | 22 |
| Knife | II | 13 (80% melee) | 0.93 s | 280 | 30% / 3× | 22 |
| Knife | III | 18 (80% melee) | 0.86 s | 280 | 40% / 3.5× | 22 |
| Knife | IV | 25 (80% melee) | 0.78 s | 280 | 50% / 4× | 22 |
| Pistol | I | 12 (100% ranged) | 1.20 s | 400 | 10% / 2× | 6 |
| Pistol | II | 20 (100% ranged) | 1.12 s | 400 | 15% / 2× | 6 |
| Pistol | III | 30 (100% ranged) | 1.03 s | 400 | 20% / 2× | 6 |
| Pistol | IV | 50 (100% ranged) | 0.87 s | 400 | 25% / 2× | 6 |
| SMG | I | 3 (50% ranged) | 0.17 s | 400 | 1% / 1.5× | 3 |
| SMG | II | 4 (60% ranged) | 0.17 s | 400 | 1% / 1.5× | 3 |
| SMG | III | 5 (70% ranged) | 0.17 s | 400 | 1% / 1.5× | 3 |
| SMG | IV | 8 (80% ranged) | 0.15 s | 400 | 1% / 1.5× | 3 |

The current knife is a finite-lifetime throw that can hit each enemy once. Pistol and SMG bullets disappear on the first enemy hit by default; Saeed's item now adds two further distinct hits. These later designer decisions replace the earlier thrust plan. The wiki tier-I knife has lower base damage than the pistol; review its feel after designer testing. Do not assume reloads or additional tiers without a decision.

## Working rules

- Implement only what a developer task and the PRD support. For unspecified mechanics, record a question rather than silently copying a reference game.
- A missing art asset must not stall engine work: use a simple placeholder, then swap to the exact approved path in `art-requirements.md` after designer approval.
- Record decisions that affect art output in `art-requirements.md` so the artist sees them. Do not alter visual design requirements without the designer's direction.
- After writing code, give the designer a short manual test checklist and ask for observed results. Never claim the project was run or verified by an agent.

## First playable readiness

The initial five-wave target has been expanded by the designer to ten waves. Difficulty values remain provisional and editable. The active shop uses the approved passive catalog in `_docs/items-db.md`; the earlier +5% damage sample is inactive.

Completing the eventual 20-wave content and final shop balance still needs designer feedback on waves 1–10, later wave schedules, item tuning, and several edge-case rules listed below. The designer alone runs and tests the result.

## Open technical questions

- What window scaling behavior should be used for the 1280×720 design view?
- How should the designer tune provisional waves 1–10 after testing, and what should waves 11–20 contain later?
- What are final shop offer prices and the later passive-item catalog? Does reroll cost reset each shop visit?
- How should Brotato's flat melee/ranged damage scaling translate to this game's percentage-based bonuses?
- What happens when buying duplicate weapons while slots remain, or when the matching weapon is already tier IV?
- Which enemy visual identity maps to melee versus ranged, and what are their default health, speed, firing range, and retreat thresholds?
- Do coins persist between shops? Confirm victory after wave 20, defeat at 0 HP, and Play Again behavior.

## Five-wave implementation decisions (developer provisional, 2026-10-03)

The first playable is written in C# under `scripts/data/` and `scripts/gameplay/`. `scenes/main.tscn` is the entry scene and exposes ten wave resources and the item pool in the Inspector. `scenes/enemies/melee.tscn` and `ranged.tscn` expose baseline values. Wave data is in `resources/waves/wave_01.tres` through `wave_10.tres`. Approved character, enemy, environment, weapon, and projectile visuals now load from `assets/art/approved/`; missing art still uses `assets/placeholders/`. No `_art/work/` file is referenced. The floor is tiled and the walls are assembled from the approved atlas in `Arena.cs`. Clearing wave 10 currently shows the temporary completion screen; the 20-wave victory copy is TBD.

All ten authored waves last 45 seconds. Burst warning duration starts at 1 second in every wave file and can be edited per burst. Burst schedules below are `(time: Jira melee + mail ranged + many-eyes parent)`; the Jira count includes both visual variants. Group speed and resistance values remain editable. The group HP multipliers in waves 1–10 now default to 1, with HP growth controlled by the wave-level multiplier described below. The wave-1 boss spawns directly at t=0 as an explicit testing exception to the ordinary burst warnings. Every enemy defeated, including spawned offspring, rolls for 0–3 coins at the probabilities documented in Minor updates 2 below. Enemies still alive and coins on the field disappear when a wave ends.

| Wave | Burst schedule, seconds from wave start | HP multiplier range | Speed multiplier range | Knockback resistance adjustment |
| --- | --- | ---: | ---: | ---: |
| 1 | 0: 10+0; 10: 9+1; 20: 9+1+1; 31: 9+2; boss at t=0 | 1.000000 | 1.00–1.05 | +0.00 |
| 2 | 0: 9+1; 8: 9+2; 16: 9+2+1; 25: 10+2; 35: 10+3 | 1.350000 | 1.05–1.10 | +0.05 |
| 3 | 0: 8+2; 7: 9+2; 15: 10+3+1; 23: 11+3; 32: 12+3 | 1.822500 | 1.10–1.20 | +0.10 |
| 4 | 0: 8+2; 6: 10+3; 13: 11+3; 20: 12+4+1; 28: 13+4; 36: 14+4 | 4.55625 | 1.15–1.25 | +0.15 |
| 5 | 0: 10+3; 6: 11+3; 12: 12+4; 19: 13+4+2; 26: 14+5; 33: 15+5; 40: 16+5 | 11.390625 | 1.20–1.35 | +0.20 |

Additional provisional values: arena 1800×1100; Mohsen speed 240 px/s and pickup radius 78 px; Jira melee enemy 10 HP, 105 px/s; mail ranged enemy 15 HP, 90 px/s, 260 px firing range, 140 px retreat distance, 2.4 s shot interval, 230 px/s projectile speed. Wave 1 uses base HP, with slight late-burst speed increases; later waves use separate editable wave-level HP and group speed multipliers. Enemy touch and projectile each deal 1 HP subject to the same player invulnerability. No initial HP regeneration or other stat bonuses. Spawn positions are random within the walls, with a preferred minimum distance of 390 px from Mohsen and 42 px from other pending signs; a 40-attempt fallback may pick a closer legal point in a crowded burst.

Weapon tier base damage, intervals, crit, and scaling coefficients use the selected wiki values; pistol and SMG ranges remain 400 while knife range is provisionally 280; knockback uses the later designer-requested tuning shown in the table above. Provisional damage formula is `base_damage × (1 + (general_bonus_percent + scaling_coefficient × matching_melee_or_ranged_bonus_percent) / 100)`. Projectiles travel at 580 px/s for pistol and 650 px/s for SMG; the thrown knife starts at 560 px/s, eases down during a 0.65-second lifetime, and has a 280-pixel tier range and can hit each enemy once. Gun bullets disappear on the first enemy hit by default; Saeed's Bullet pierce adds further distinct hits. Duplicate purchases merge immediately when a same-type/same-tier weapon below tier IV exists; chain merges are supported. Otherwise they occupy a free slot. Tier IV duplicates occupy a free slot only and are blocked at four occupied slots.

The active shop rules are specified in the approved item catalog revision below. Weapon tiers in offers are I for shops after waves 1–2 and I–II after waves 3–4. The later 3× weapon-price revision sets tier I–IV costs to 18/30/42/54 coins. `TierOneWeaponPrice` and `WeaponTierPriceStep` are exported as 18 and 12 for Inspector tuning. Coins carry over between shops. Reroll cost resets to 5 each shop and rises by 5 per reroll there. A new shop refreshes each unlocked slot; locked offers retain their slot and price across shops unless a unique purchase makes them ineligible. Bought slots stay empty until reroll or the next shop.

The 1280×720 canvas scales with the window using `canvas_items` stretch. The Play Again button returns to the menu; the next Play begins a fresh run. Defeat occurs at 0 HP after the shrink animation. No code was run, built, compiled, launched, or tested by the developer; designer verification is pending.


## Component-scene refactor and weapon attachment (2026-10-03)

The designer reported missing component scenes, a weapon occasionally staying behind Mohsen, and weapon visuals aiming throughout cooldown. The root cause of the left-behind symptom was that weapon sprites were siblings of Mohsen in world space and their position was refreshed only after a target was found. When Mohsen moved with no target, the sprite stayed at its last world position.

The scene hierarchy now uses `scenes/player/player.tscn` with an editable `Visual`, `WeaponMount`, and `Camera`. Knife, pistol, and SMG each have a scene under `scenes/weapons/` with a visible `Visual` child and `Weapon.cs`. Weapon instances are children of `WeaponMount`, use local slot offsets, and follow Mohsen automatically regardless of target state. `Weapon.Tick` keeps an Inspector-editable idle rotation, turns toward the target when an attack starts, and returns to idle after an Inspector-editable 0.08-second pose. SMG fire interval is 0.17 seconds, so its pose returns to idle between shots when targets remain in range. The weapon's tier and attack stats remain separate from scene presentation.

`scenes/gameplay/arena.tscn`, enemy scenes, separate attack/projectile scenes, coin, spawn warning, and separate UI scenes for menu, starting choice, shop, offer row, finish, and HUD are editable components. `scenes/main.tscn` composes the arena and `GameController` coordinates wave flow. The previous runtime-created child sprite paths were removed. This refactor was inspected statically only; the designer must verify scene import, movement, attachment, and attack poses in Godot.

## Combat polish (designer request, 2026-10-03)

The knife attack scene now exports a 0.65-second lifetime, 35-pixel hit radius, 560 px/s starting speed, 0.25 ending-speed multiplier, and 1.5 speed-ease exponent. Its tier range is provisionally 280 pixels from Mohsen. The nonlinear speed curve starts fast and decelerates during flight; range and lifetime both cap travel. It tracks enemy instance IDs so one throw can damage several enemies but cannot damage the same enemy twice. Gun bullets use the first swept collision along their movement segment and disappear immediately on hit; the prior pistol second-hit behavior is removed. Hostile bullets keep their existing one-hit behavior. These collision checks use the traveled segment, so a fast bullet can still hit an enemy between its frame endpoints.

Provisional knockback per hit is 22 pixels for knife, 6 for pistol, and 3 for SMG before player bonus and enemy resistance. Enemy sprites flash full white through a per-instance shader material for an Inspector-editable 0.07 seconds on every hit, including a brief defeated pose before removal. Mohsen's camera gets a brief Inspector-editable 0.09-second shot shake, with provisional per-weapon strengths of 2.5 pixels for knife, 3 for pistol, and 1.25 for SMG, capped at 5 pixels. The shake affects the world camera, not the HUD. All values await the designer's playtest; no agent build or run was performed.

## Approved art integration (2026-10-03)

`scenes/player/player.tscn` uses the approved **without-shoes** `mohsen_idle.png` at its 64×64 native size; the visible figure is 60 pixels tall. Until an approved walk export arrives, movement keeps this idle sprite with the existing subtle code bob. The gameplay scenes do not reference the shoe or cutout files in `_art/work/`.

`scenes/enemies/melee.tscn` uses `jira_ticket.png`; `scenes/enemies/ranged.tscn` uses `mail_slime.png`. The ranged slime emits the approved red-envelope `enemy_projectile.png` from an Inspector-editable mouth-area offset. The enemy sprite flips horizontally toward Mohsen. Both scenes retain code-driven walk squash. Their current display scales after the 2× art update are documented below.

The knife, pistol, SMG, and shotgun weapon scenes use their matching approved PNGs. The rotating thrown knife scene reuses the same approved knife art. `pistol_bullet.tscn` and `smg_bullet.tscn` remain separate projectile scenes but share one two-frame animated visual from `resources/projectiles/player_bullet_frames.tres`; shop weapon icons still load the approved weapon PNGs.

`scenes/gameplay/arena.tscn` supplies the approved 256×256 repeatable carpet tile and 288×288 wall atlas. `Arena.cs` draws the floor in clipped 256-pixel repeats and selects atlas cells in the documented 3×3 order: corner cells once, edge cells repeated along each side, and the transparent center left unused. The wall's visible band is 36 pixels; Mohsen, enemies, and spawn positions now stay inside that band with visual-size margins. Arena dimensions remain 1800×1100. Partial final edge cells accommodate dimensions that are not multiples of 96; seam appearance needs designer inspection.

Still using developer placeholders: coin, red spawn warning, portraits for items without approved art, and finish-screen illustrations. No approved Mohsen walk sprite exists. This integration was reviewed statically only; the designer will run the project and verify art scale, wall seams, projectile origins, and orientation.

## Facing and animation polish (designer request, 2026-10-03)

Mohsen's approved static idle sprite flips horizontally whenever movement has a negative X component and returns to its normal right-facing orientation for positive X. Pure up/down movement preserves the last horizontal facing. The `WeaponMount` remains unflipped so independent weapon orientation and projectile direction are unaffected.

Enemies now use `shaders/enemy_hit_flash.gdshader` on their scene `Visual` nodes. `Enemy.cs` duplicates the material per enemy and drives a full-white silhouette flash for a 0.07-second hit interval, shortened from the original 0.12 seconds. `WalkSquash` is 0.10; the X/Y scale varies visibly. Display scale is set per enemy scene. Hit geometry and wall margins remain separate from sprite scale, with the wall margin widened to keep the enlarged art inside the arena.

The thrown knife retains one hit per enemy but now starts at 560 px/s and eases toward 25% of that speed over a 0.65-second lifetime using a 1.5 exponent. Tier range is provisionally 280 pixels. Its starting speed and ease parameters live in `scenes/projectiles/knife_attack.tscn`; tier range lives in `WeaponData.cs`. Pistol and SMG recoil only their child visual sprites backward along their local barrel axis: 7 pixels over 0.14 seconds for pistol and 3.5 pixels over 0.11 seconds for SMG. Their root weapon mount positions and projectile origins stay fixed. These values are developer starting points pending designer playtest. No agent run, build, compile, launch, or test was performed.

## Teammate passive catalog (designer handoff, 2026-10-04)

`resources/items/` holds one editable `ShopItemDefinition` resource for each of the 26 entries in `_docs/items-db.md`, plus the now-inactive sample resource. The main scene's Inspector-editable `ItemPool` lists only the 26 approved catalog entries. Fourteen named teammate resources use their exact approved portrait PNG; the three DevOps entries share the approved YAML icon; Hookah, Propranolol pills, and Heavy Smoker use their approved PNGs. Entries without approved art use `assets/placeholders/passive.svg`. The shop displays each item's role, rarity, uniqueness, and per-copy/current modifier values. The earlier work-in-progress SMG texture reference in the reusable inventory entry was removed; code supplies the actual weapon or item icon.

2026-10-05 correction: a static audit found many item `Amount` properties absent from the saved `.tres` files. They therefore loaded the former `StatModifier.Amount` default of `+5`, regardless of the intended database value. This made Maryam's movement modifier `+5` instead of `−35`, with similar errors across the catalog. All 23 database items now explicitly save their stat enum and floating-point amount; their role descriptions are saved too. The sample item explicitly saves `+5%` bonus damage. A missing modifier amount now defaults to zero, preventing an accidental bonus. The values still follow `_docs/items-db.md` as provisional balance inputs and require designer verification in game.

2026-10-05 approved item batch: the three DevOps resource icons now reference `assets/art/approved/items/devops/yml_file.png` without changing their +6/+8/+10 percentage-point ranged modifiers. Hookah saves +5 px/s movement, −5 max HP, and +2 knockback; Propranolol pills saves −15 px/s movement and exactly +10 HP/s regeneration; Heavy Smoker saves +27 percentage-point all-damage bonus and −15 max HP. Each is a separate editable resource with a provisional 5-coin price and the existing additive stacking behavior. Heavy Smoker's `Description` says `Role TBD.` because no role was approved. The sample +5% item was found still lacking an explicit serialized amount and now saves `Stat = BonusDamagePercent` and `Amount = 5.0`. The 1254×1254 approved icons retain their source resolution and use the existing 64×64 offer and inventory slots. No agent run, build, compile, launch, or test was performed.

2026-10-05 teammate portrait handoff: Behzad, Narges, Sahel, and Saman item resources now point to their corresponding approved 128×128 transparent PNGs. A static audit also found Pooria's earlier approved portrait still using the placeholder; his resource now points to the approved PNG. The existing offer and inventory `TextureRect` controls fit them into 64×64 slots. Saman's intended +5% bonus damage had been omitted from the saved resource and was therefore zero with the current neutral default; it now saves `Stat = BonusDamagePercent` and `Amount = 5.0`. Other modifiers and shop rules are unchanged. Designer verification is pending; the developer did not run, build, compile, launch, or test project code.

On the designer's 2026-10-04 resize request, all ten approved item PNGs were reduced from 384×384 to 128×128 pixels in place, keeping their transparent channel and resource paths. The shared passive SVG placeholder now declares a 128×128 canvas. Shop offer and inventory controls still scale these images to their existing UI slots; this task changes source resolution, not layout.

The subsequent shop display request sets the `TextureRect` slots in `scenes/ui/shop_offer.tscn` and `scenes/ui/shop_inventory_entry.tscn` to 64×64 UI pixels. `expand_mode = Ignore Size` prevents each texture's native dimensions from enlarging its container; `stretch_mode = Keep Aspect Centered` preserves proportions within the slot. Offer rows and both inventory scroll rows are taller, and the shop column gaps are smaller to keep the panel within the 1280×720 design view. The scene layout still needs designer runtime verification.

## Editor testing shortcuts (designer request, 2026-10-04)

`GameController._Input` handles non-repeating `C`, `P`, and `R` presses only when `OS.HasFeature("editor")` is true. Godot 4.6 [documents the `editor` feature tag](https://docs.godotengine.org/en/4.6/classes/class_engine.html#class-engine-method-is-editor-hint) as present when running from the editor and absent in exports; `Engine.IsEditorHint()` would be false for ordinary game play launched by F5. `C` adds 100 coins and immediately refreshes the visible shop or HUD count. Coins granted on menu, weapon-choice, or finish screens are carried into the next started run. `P` is now active only while the shop is open: it freely clears all four offer locks and rerolls every slot from eligible items whose role description is `Product manager.`. It does not spend coins, increase the normal reroll price, offer weapons, or skip waves. The Product Manager-only filter stays active for any replacement after a purchase until the normal paid reroll or the next shop. `R` reloads the current scene, returning to the menu with fresh state. `P` presses outside the shop have no effect. No project code was run, built, compiled, launched, or tested by the developer.

The early five-coin passive price and equal-weight offer draw were superseded by `_docs/items-db.md` and later designer revisions. Current prices are 1.75× the prior catalog costs rounded up; the 40/60 offer split and rarity weights remain. Negative HP regeneration offsets positive regeneration, with effective regeneration floored at zero; it does not drain HP. Max-HP reductions cannot lower current HP below one at purchase time. No agent run, build, compile, launch, or test was performed.

## Coin, damage-number, and shop-inventory polish (designer request, 2026-10-03)

Coin pickup uses a circular 78 px trigger radius, enlarged provisionally from 48 px. A triggered coin remains magnetized even if Mohsen runs out of the trigger area. Its movement follows Mohsen's current position each frame, starting at 80 px/s, accelerating at 1800 px/s² to a 900 px/s cap, and collecting within 14 px. These four values are editable on the player or coin scene/script. The trigger radius is slightly beyond Mohsen's visible body. Field coins are still removed at wave end.

`scenes/effects/damage_number.tscn` is a reusable white Label with a 5 px black outline. Every accepted enemy `TakeHit` spawns one number showing the rounded applied attack damage, including lethal hits. Its provisional 0.7 s animation rises 55 px with ease, drifts at most 18 px sideways, scales from 1.3 to 0.9, and fades after the first 35% of its lifetime. The duration and travel distances are editable scene exports.

The shop has bottom grids for equipped weapons and bought passive items. Every equipped weapon has its own entry, including its current merge tier. Repeated purchases of the same passive resource are counted in one entry as `×N`; the existing +5% item still stacks additively. Hover tooltips show effective weapon damage using the same calculation as firing, base damage, attacks per second and interval, range, knockback, critical chance/multiplier, and projectile behavior. Passive tooltips show per-copy and total modifier values, the current global damage bonus, and current damage and attack speed for equipped weapons. Entries use `scenes/ui/shop_inventory_entry.tscn` inside vertically scrollable `GridContainer`s. Purchase counts reset on a new run. These presentation values await designer review.

## Minor combat improvements (designer request, 2026-10-05)

`scenes/player/player.tscn` now has a 56×8 px black-backed red health bar centered about 45 px above Mohsen. `Player.cs` sets its maximum and value from current health and hides it at full HP, zero HP, and during the death shrink. It updates immediately on damage, refill, or max-HP item changes, and during regeneration. The existing top-left HUD health bar remains.

The full-white enemy hit flash lasts an editable 0.07 seconds instead of 0.12. `DamageNumber.cs` adds an editable spawn-origin offset of ±8 px horizontally and ±6 px vertically, separate from its existing side drift. Ordinary melee/ranged movement bases are 105/90 px/s instead of 82/68. A subsequent designer direction set wave-1 Jira melee and mail ranged health near 10/15 HP, superseding the earlier provisional 14/10 HP choice. Even at wave 5's largest 1.35 speed multiplier, melee/ranged speeds are about 142/122 px/s, below Mohsen's initial 240 px/s. The wave-1 test boss retains 120 HP and 210 px/s. Actual movement of all enemies, including the boss, is capped at 95% of Mohsen's current movement speed so speed-penalty items cannot make him slower than a pursuer. All 1–5 wave burst counts in the table above are editable; each totals at least ten enemies. These are starting balance values awaiting a designer run.

## 2× approved enemy art handoff (2026-10-04)

The approved Jira ticket and mail slime PNGs were replaced at the same paths with 2× source-resolution exports. To preserve the previous on-screen sizes, `melee.tscn` halves `VisualScale` from 2.0 to 1.0 and `ranged.tscn` halves it from 1.12 to 0.56. The ranged scene's saved `Visual` child scale was removed because `Enemy.cs` sets its scale in `_Ready` and each frame. The inspector range for `VisualScale` now includes values below 1. Collision and contact radii remain independent of artwork scale.

The visible mouth in the new slime source sits close to the centered sprite pivot. The ranged projectile now starts at a provisional local offset of (0, 2) pixels plus 12 pixels toward Mohsen, keeping the red envelope near that mouth at the preserved display size. Both values are editable in the enemy Inspector. The designer should verify the source and visual size in motion; no agent run, build, compile, launch, or test was performed.

## 2× approved pistol and SMG art handoff (2026-10-04)

The approved pistol and Uzi-style SMG PNGs were replaced at their existing paths with 72×48 and 88×56 exports. `scenes/weapons/pistol.tscn` and `smg.tscn` set only their centered child `Visual` sprites to 0.5 scale, preserving approximately the former 36×24 and 44×28 display frames while retaining the weapon mount positions and shot origins. The knife and bullet sprites are unchanged. The designer should verify gun size and alignment in motion; no agent run, build, compile, launch, or test was performed.

## Ranged red-envelope size (designer request, 2026-10-04)

`scenes/projectiles/enemy_bullet.tscn` now scales its `Visual` sprite to 2×, showing the approved 26×19 red envelope at about 52×38 display pixels. The projectile root, speed, mouth spawn position, and existing 21 px hit radius are unchanged. That radius is already close to the enlarged visible envelope's half-height and remains an Inspector-editable gameplay value. The designer should check the apparent size and hit feel in game; no agent run, build, compile, launch, or test was performed.

## Approved enemy death explosion (2026-10-04)

`scenes/effects/enemy_explosion.tscn` uses the approved 1024×128 sprite sheet as eight centered 128×128 `Sprite2D` frames. `EnemyExplosion.cs` advances the frames once and frees the effect after frame eight. The first integration used 12 FPS (about 0.67 seconds); after designer feedback that this was too slow, the scene now uses an Inspector-editable 30 FPS (about 0.27 seconds). `GameController.EnemyDefeated` spawns one effect at the enemy's global death position for either enemy type. The effect is an independent node above the enemy visual, so it remains visible after the current 0.07-second lethal flash and enemy removal. Coin drops and defeat logic are unchanged. The designer should check the new timing; no agent run, build, compile, launch, or test was performed.

## Longer enemy bullets and thrown-knife trail (designer request, 2026-10-04)

The hostile red envelope now has an Inspector-editable 12-second lifetime in `enemy_bullet.tscn`, up from the shared 1.5-second default, and a 3000 px travel cap when spawned. At 230 px/s it can cover 2760 px before the time cap, beyond the 1800×1100 arena diagonal; the existing arena-boundary cleanup removes it earlier after it exits the arena. Its speed, damage, hit radius, and appearance are unchanged.

The thrown knife scene has a white `Line2D` trail behind its rotating art. `Projectile.cs` samples the knife's world-space flight positions and retains a provisional 0.16-second path with a 10 px stroke and a transparent-to-white gradient from tail to tip. The trail is visual only and disappears with the knife; knife range, easing, hits, and lifetime are unchanged. Trail lifetime and width are editable in the Inspector. The designer should check apparent trail thickness and envelope duration in game; no agent run, build, compile, launch, or test was performed.

## Orange boss and 45-second waves (designer direction, 2026-10-04)

The approved 288×192 `orange_boss.png` is used by `scenes/enemies/orange_boss.tscn`. Its native scene scale keeps its roughly 249×180 visible body about twice as tall as ordinary slimes. `BossEnemy.cs` inherits common health, white hit flash, damage numbers, knockback, contact damage, coin drop, and death explosion behavior from `Enemy.cs`, while overriding only movement and attack. The ordinary melee and ranged behavior was moved into a virtual hook without changing its intended motion or attack rules.

For testing, `GameController.StartWave` spawns one boss immediately at wave 1's start, 240 px above Mohsen. This special test spawn bypasses the ordinary red-cross burst warning; ordinary burst spawns still warn normally. The boss can be damaged by the existing player weapons and is removed with other enemies at wave end. Long-term boss placement and rewards are **TBD**.

Provisional Inspector-editable boss values: 120 HP, 210 px/s movement (Mohsen starts at 240), 70% base knockback resistance, an oval body hurt/contact area with 118×84 px half extents, 270 px preferred distance, 2.8 s moving, 1.2 s stopped, two radial volleys 0.45 s apart per stop, 16 angular slots per volley, and three omitted slots around both the player-facing direction and the opposite direction. The aim angle is fixed for the stopped phase, leaving two stable safe corridors. Each emitted bullet moves at 200 px/s and uses the same approved red-envelope projectile scene as the ranged slime. Movement orbit weight, wall margin, bullet origin distance, speed, volley timing/count, slot count, and gap width are exposed for tuning. The boss visual scale, contact geometry, and wall clearance are separate values.

`WaveDefinition.Duration` now defaults to 45 seconds and all five `.tres` wave resources set that duration explicitly. Existing burst trigger times were compressed proportionally and rounded to whole seconds so every one-second warning still completes before the wave ends; the revised schedules are in the table above. `SpawnBurstDefinition.TriggerTime` now offers a 0–45 s Inspector range. The designer's new 45-second instruction supersedes the older 65-second line in designer-owned `game-design-notes.md`; that file was not changed. No agent run, build, compile, launch, or test was performed.

## Boss hit area and shared red envelopes (designer feedback, 2026-10-04)

Player projectiles previously tested only the boss center against each projectile's radius, making its large visible body hard to hit. `Enemy.HurtboxHalfExtents` now provides optional editable oval dimensions. The orange boss scene sets 118×84 px half extents to follow its roughly 249×180 visible art. Knife and gun swept paths intersect the oval expanded by their projectile radius, and player contact checks use the same oval expanded by the existing 18 px player allowance. Ordinary enemies leave these extents at zero and retain their previous circular collision behavior.

Boss volleys now instantiate `scenes/projectiles/enemy_bullet.tscn`, the same approved red envelope, size, lifetime, and hit radius used by the ranged slime. The boss still controls its own provisional 200 px/s bullet speed. The unused boss-specific bullet scene and orange placeholder were removed. The designer should verify hits across the boss silhouette and the shared envelope appearance; no agent run, build, compile, launch, or test was performed.

## Approved Hamidreza shopkeeper and bubble (2026-10-04)

`scenes/ui/shop.tscn` places the approved static 384×351 Hamidreza portrait below the stats panel on the right. `scenes/ui/shop_speech_bubble.tscn` supplies a reusable cream speech bubble beside him with dark text and a small tail. The former inline `Panel/Box/Message` label was removed; shop purchase, reroll, and error messages use `SpeechBubble/Text`.

`GameController` tracks whether the shop was entered during the current run. On the first visit it shows the exact designer line, `welcome to VX the Greatest function of Snappfood`; buying or rerolling replaces it with the existing status message. Later shop entries begin with an empty, hidden bubble until there is a status message. Starting a new run resets the first-visit greeting. Portrait display size and bubble placement are provisional for designer visual review. No agent run, build, compile, launch, or test was performed.

## Shop screen redesign from reference (designer request, 2026-10-04)

The 1280×720 `shop.tscn` now uses a dark header with next-wave title, centered coin count, and top-right reroll. Four reusable `shop_offer.tscn` cards sit below it in one row, each with a fixed 64×64 icon, visible name/type and relevant modifier or weapon-stat description, buy price, and lock button. A right-side stats panel shows Mohsen's live HP, regeneration, speed, knockback, damage bonuses, pickup radius, weapon count, and bought item count. Hamidreza and the existing speech bubble sit beneath that panel. Bought items and weapons use separate bottom grids of 72×72 entries with fixed 64×64 images, tier or stack badges, and their existing detailed hover tooltips. The next-wave button sits at bottom right. When no next wave exists, the title reads `Shop — Last Wave`, and the continuation button reads `Finish`. The layout uses default Godot controls and existing approved/placeholder art; no new sprite was created. Scene geometry and typography are provisional pending designer review.

## Designer-reference player bullets and hit effect (2026-10-05)

The designer supplied one screenshot with two top bullet sprites and four bottom hit-effect frames. The developer extracted the 64×64 cells pixel-for-pixel, made only the uniform blue cell background transparent, and replaced the existing pistol and SMG bullet PNGs at their stable approved paths. Both top sprites are consecutive frames of one shared animation for both gun bullet scenes. Following the designer's latest direction, `resources/projectiles/player_bullet_frames.tres` plays once and holds its second frame. Its current speed is 90 FPS, preserved from the designer's edit; `Projectile._Ready()` explicitly calls `Play("default")` for an `AnimatedSprite2D` visual. Separate pistol and SMG scenes use it at 0.25 scale; shotgun pellets use it at 0.18 scale, all with nearest filtering. Hit radius, travel speed, damage, and the four-frame impact effect are unchanged. The designer verifies perceived timing in game.

The four bottom cells form `assets/art/approved/effects/player_bullet_hit.png` as a 256×64 horizontal sheet. `scenes/effects/player_bullet_hit.tscn` and `PlayerBulletHit.cs` play its four 64×64 frames once at a provisional, Inspector-editable 24 FPS and 0.5 visual scale, then free the effect. Pistol and SMG swept-hit code spawns the effect at the first collision point, even when the hit defeats the enemy. Knife hits, hostile red envelopes, and non-hit bullet expiry do not spawn it. The designer should verify mapping, visible size, timing, and impact position in game; the developer did not run, build, compile, launch, or test project code.

## Shared approved pistol/SMG muzzle flash (2026-10-04)

`scenes/effects/muzzle_flash.tscn` uses the approved 32×28 static PNG once for both guns. Its root is the ignition point; the child sprite is offset 16 px right, putting the image's left-center edge at the root. The pistol and SMG scenes instance this component at provisional local muzzle points (18, -5) and (21, -6), at 0.75 and 0.65 visual scale respectively. Their weapon roots rotate toward the target for the existing attack pose, so the flash rotates with aim; it remains hidden for the knife.

`Weapon.cs` shows the flash for an Inspector-editable 0.07 s on pistol and 0.05 s on SMG, fading during the latter half. The flash follows each gun's child-sprite recoil displacement and is hidden when returning to idle. Player gun bullets now spawn from that muzzle point; the knife retains its prior origin. These sizes and anchors are provisional pending the designer's visual check. No agent run, build, compile, launch, or test was performed.

## Approved shotgun V2 integration (designer handoff, 2026-10-05)

`scenes/weapons/shotgun.tscn` uses the approved 108×48 right-facing V2 sprite at 0.5 display scale. It has its own muzzle point, flash, recoil, and shot shake. `scenes/projectiles/shotgun_pellet.tscn` reuses the approved two-frame player-bullet animation at 0.18 scale because no shotgun pellet art is approved; each pellet uses the existing one-enemy swept-hit and impact behavior. A blast aims at the nearest enemy in range, fires evenly spaced pellets across the configured angle, rolls critical hits independently per pellet, and applies screen shake once per blast. Each pellet stops at its first enemy, when its lifetime expires, or at the remaining target range measured from the muzzle. This allows multiple pellets to hit one enemy at close range.

Four Inspector-editable `ShotgunTierDefinition` resources in `resources/weapons/shotgun_tier_01.tres`–`shotgun_tier_04.tres` supply **provisional developer starting values**. Damage and knockback are per pellet. All values are for designer review, not approved final balance.

| Tier | Damage/pellet | Interval | Target range | Pellets | Spread | Pellet speed | Pellet life | Crit chance / multiplier | Knockback/pellet |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | --- | ---: |
| I | 12 | 1.50 s | 210 px | 5 | 36° | 520 px/s | 0.34 s | 8% / 1.5× | 8 |
| II | 16 | 1.40 s | 215 px | 6 | 38° | 535 px/s | 0.35 s | 10% / 1.6× | 9 |
| III | 21 | 1.30 s | 220 px | 7 | 40° | 550 px/s | 0.36 s | 12% / 1.7× | 10 |
| IV | 28 | 1.20 s | 230 px | 8 | 42° | 565 px/s | 0.38 s | 15% / 1.8× | 11 |

The provisional ranged-damage scaling coefficient is 100% on all four tiers. The shotgun joins the starting choice and the equal-weight shop pool. It uses the existing four-slot cap, same-tier merging, tier availability, and provisional weapon prices. Shop offers and inventory tooltips label damage and knockback per pellet and show pellet count, spread, speed, and lifetime. Final shotgun balance, pellet art, and whether its tiers should differ from the current merge model remain designer decisions. The developer did not run, build, compile, launch, or test project code.

## Approved female Jira and many-eyes enemies (designer handoff, 2026-10-05)

`scenes/enemies/melee_female.tscn` uses the approved coral Jira PNG with the existing `Enemy.cs` melee behavior, health, speed, contact radius, hit flash, and death path. Only the visual differs. An editable wave group swaps it into one burst in each of waves 1–5: wave 1 at 0 s, wave 2 at 8 s, wave 3 at 7 s, wave 4 at 13 s, and wave 5 at 12 s. The other Jira groups keep the original green visual.

`scenes/enemies/many_eyes.tscn` uses `ManyEyesEnemy.cs`, which overrides movement and attack while retaining the shared hit, contact, knockback, coin, and death effects. It wanders in random directions, turns at an editable interval or near a wall, and emits eight evenly spaced hostile bullets per volley without aiming at Mohsen. The current volley uses the approved red-envelope scene; the earlier blue placeholder projectile scene remains unused. On death, a one-time hook spawns the approved one-eyed minions in a circle around the parent; `scenes/enemies/one_eye_minion.tscn` uses ordinary melee pursuit and has no offspring behavior. Minions use the shared 0–3 coin-drop roll. The parent appears in one warned burst per wave, at 20/16/15/20/19 seconds in waves 1–5, with counts 1/1/1/1/2; group HP and speed multipliers remain editable.

Provisional Inspector values: many-eyes 24 HP, 76 px/s, 10% knockback resistance, 32 px contact radius, 2.2 s wandering turn interval, 2.8 s volley interval after a 1.4 s initial delay, 190 px/s red envelopes originating 28 px from center, six offspring placed 32 px around the parent, and offspring HP/speed multipliers 1.0. The shared red-envelope scene uses a 12 s lifetime and 21 px hit radius. Each one-eyed minion starts at 4 HP, 130 px/s, and 18 px contact radius; all enemy movement remains capped by the existing 95%-of-Mohsen rule. The designer has not fixed the timing, speeds, parent/minion health, offspring count, or wave placement. No agent run, build, compile, launch, or test was performed.

## Minor updates 2 (designer request, 2026-10-05)

Coins now grow from zero to 115% visual scale and settle at normal size over an editable 0.22 s. On entering Mohsen's existing 78 px pickup zone, each coin moves 18 px outward over 0.12 s before its previous accelerating homing phase resumes. The coin remains magnetized throughout, including if Mohsen runs away. Each enemy defeat independently rolls a uniform integer 0–99: 0–9 drops zero coins, 10–39 one, 40–74 two, and 75–99 three. Each drop scatters at most 12 px from the defeated enemy; all existing coin pickup and wave-end cleanup paths remain. These animation distances/times are provisional and exposed in `Coin.cs` and `GameController.cs`.

Every death also spawns `scenes/effects/enemy_death_text.tscn`, which chooses one of the six exact designer phrases and plays a 0.75 s upward scale pop and fade. The new `EnemyDeathText.cs` controls editable timing/drift; the effect is independent of the existing explosion and damage number.

`EnemySpawnGroupDefinition.AlternateEnemyScene` allows each wave 1–5 Jira melee group to save both approved male and female scenes. The group alternates them per enemy with a random initial phase, keeping each group's count, health/speed/resistance multipliers, warned positions, and trigger time. One-enemy groups naturally show one variant. `many_eyes.tscn` now assigns the shared `enemy_bullet.tscn` red-envelope projectile to its eight-way volley; the old blue placeholder bullet scene is unused.

Mohsen's initial max and current HP are 10. The shop draws only items whose combined max-HP modifier leaves both current and maximum HP above zero; it also replaces any now-unsafe visible or locked offers after a purchase and guards the buy action against stale state. This uses the current HP at the shop and does not change existing max-HP application rules. With a 10 HP start, an item such as Heavy Smoker (−15 max HP) can enter the pool only after suitable HP gains. The offer description component is now a Godot `RichTextLabel` with BBCode: positive modifier lines are green (`#76df80`), negative lines red (`#ff7676`), and zero neutral gray. Weapon descriptions remain plain. No project code was run, built, compiled, launched, or tested.

A static item-resource audit during this work found four earlier omitted modifier amounts despite the prior DEV-38 handoff note: Hojat and Sara's +5 percentage-point damage, and Hossein and Sepideh's +5 max HP. These four resources now save explicit `Amount = 5.0` values from `_docs/items-db.md`; Hossein and Sepideh can therefore affect the new HP offer filter as intended. This audit did not run project code.

## Combat feel: sound, impact pauses, boss warning (designer approval, 2026-10-05)

`scenes/effects/game_audio.tscn` is the replaceable sound component. It references eleven developer-generated mono PCM WAV placeholders under `assets/placeholders/audio/`: distinct knife, pistol, SMG, and shotgun shots; enemy hit and defeat; coin pickup; shop purchase; wave start; Mohsen hit; and boss warning. All streams are editable in that scene. `GameAudio.cs` exposes separate shot, hit, defeat, coin, UI, and warning volume controls, plus pitch variation, voice cap, and per-cue throttles. Provisional values are 12 voices, −15/−16/−12/−17/−13/−12 dB respectively, ±4% pitch, and minimum gaps of 80 ms SMG, 70 ms coin, 50 ms enemy hit, and 80 ms defeat. If all voices are occupied a new cue is skipped; active voices are not stolen. Audio continues during the brief impact pauses. These tones are temporary developer placeholders, not approved final sound design.

`GameController` uses a local impact-pause gate instead of pausing the SceneTree or changing global time scale. Its `_Process` advances the short real-time pause countdown while holding the wave clock, pending spawn warnings, and weapon tick. Player, enemies (including the boss), projectiles, coins, and short effects skip their update during the same pause frame. The pause is cleared on new run, wave start/end, and Mohsen defeat, so shop and finish transitions do not remain frozen. A knife critical hit requests 55 ms, a hit on the orange boss 45 ms, and any enemy defeat 40 ms; durations are editable exports. Simultaneous requests take the maximum remaining duration rather than summing, and an editable 110 ms gap prevents rapid ordinary defeats from repeatedly stopping SMG fire. Ordinary gun hits do not request a pause. The designer should tune these provisional values in a manual run.

`BossEnemy` now starts each stopped firing phase with an Inspector-editable 0.42 s warning. The existing sprite pulses red at 7 Hz through the shared enemy shader's `warning_amount` parameter, while the existing white damage flash is applied afterward and remains visually distinct. The boss warning sound plays once at phase start. The first radial volley fires only after the warning; later volleys retain the 0.45 s interval and existing safe-gap calculation. If an edited stop duration would end before all configured volleys, the boss remains stopped just long enough to complete them. No new artwork was used. All three features were reviewed from files only; the developer did not run, build, compile, launch, or test project code.

## Approved tank enemies (designer handoff, 2026-10-06)

Six component scenes under `scenes/enemies/` use the approved `*_tank.png` sprites and the same C# behavior class and non-health attack/movement settings as their counterparts: male Jira, female Jira, mail slime, many-eyes, one-eye offspring, and orange boss. Tank durability is represented by editable base HP only; no new damage, speed, knockback resistance, attack pattern, or loot rule is introduced. Current tank base HP is 25/25/37.5/60/10/300 versus the corresponding 10/10/15/24/4/120, exactly 2.5×. The tank boss's oval half-extents are 118×92 instead of 118×84 to cover its taller 288×208 art; other hit geometry retains counterpart values. Scene sprites retain counterpart `VisualScale` so the helmets add visible height at game scale. These are developer starting values for designer tuning.

`WaveDefinition` now exposes `TankSpawnChance`, `BossSpawnChance`, `BossTankChance`, `BossSpawnTime`, `BossSpawnOffset`, and `BossSpawnWarningSeconds`. `GameController.SelectTankScene` recognizes the six known base scene paths and maps them to Inspector-replaceable tank scenes. It refuses replacement before wave number 3 even if a chance is edited there. Each warned burst first chooses its normal male/female or other base scene, then independently rolls the per-wave tank chance before the red cross is shown; the chosen scene spawns at that cross. Many-eyes offspring use the same per-wave chance for one-eye tank selection. Wave-1's direct t=0 original boss remains unchanged, and wave 2 has no boss opportunity.

For waves 3/4/5, provisional per-enemy tank chances are **18%/24%/30%**. Each wave rolls once at start for an additional boss opportunity with **55%/60%/65%** chance. If selected, a red-cross warning appears at **22/24/20 s** at an Inspector-editable offset from Mohsen, clamped within the boss wall margin, and spawns after **1 s**. That boss independently has **45%/50%/55%** chance to use the tank-boss scene. Offsets are (240,−220), (−240,−220), and (240,220) px. These chances are separate so the normal boss may also appear in later waves; the values and placement are provisional. No project code was run, built, compiled, launched, or tested.

## Approved item catalog revision (2026-10-06)

The active `scenes/main.tscn` pool and the empty-pool fallback now load exactly the 26 items in `_docs/items-db.md`; `bonus_damage.tres` remains on disk for history but cannot be offered. Each item resource explicitly saves the approved display name, role description, modifiers, coin price, rarity 1–3, uniqueness flag, and approved or placeholder icon. `arezoon.tres` and `mana.tres` became `arezoo.tres` and `mania.tres` with their original Godot UIDs retained. Sahar-2's movement bonus is +25 px/s, and Heavy Smoker's role is `Trait.`. A read-only comparison of all 26 resources against the database found no remaining catalog mismatches.

Each shop slot independently rolls 40% weapon and 60% item when both categories have eligible choices. If a category is empty, it draws from the other. Available weapon tiers still follow the completed-wave rule, and full weapon slots admit only mergeable offers. Item rarity is per-item weighted by completed wave `w` (clamped to 1–20): rarity 1 = 12, rarity 2 = `4 + 0.25 × (w − 1)`, rarity 3 = `1 + 0.25 × (w − 1)`. All rarities are eligible after wave 1. Non-unique purchases repeat and stack additively. Arezoo, Hossein, and Saeed are unique: after purchase each is excluded from later draws and any duplicate visible or locked offer is replaced. The existing unsafe-HP offer and purchase guard remains.

Arezoo saves +6 knockback, +2 max HP, and `Spread +3`. The new `PlayerStat.Spread` is an integer count. On each pistol, SMG, or shotgun-pellet hit, three children emerge from the hit point at evenly spaced 120° directions relative to the incoming shot. Each child instances the same bullet/pellet scene and copies resolved damage, critical state, knockback, speed, configured lifetime and range, hit radius, and collision settings. The original target's instance ID is ignored throughout each child's life. Children are marked unable to spawn more spread; knives and hostile red envelopes cannot trigger it. Signed movement speed and HP regeneration modifiers stack without discarding negative amounts. Movement uses a minimum effective 1 px/s; wave healing uses `max(0, regeneration)`, so negative regeneration never drains HP and later positive purchases can offset it. This revision was reviewed from files only. The designer must run it and verify the shop distribution, unique exclusion, spread geometry, and item effects.

The rename initially left generated Godot UID and editor filesystem caches pointing at `arezoon.tres` and `mana.tres`. After the designer reported missing resources, `scenes/main.tscn` switched those two external references to their new paths without stale UID hints. The generated `.godot/uid_cache.bin`, editor filesystem caches, and old folding metadata were removed so Godot can rebuild them on the next editor open. The renamed resource files retain their own UIDs and all gameplay values. This was a static path audit only; designer verification remains pending.

## Shotgun tier load guard (designer wave-3 report, 2026-10-06)

The reported `WeaponData.Get` null exception can occur only on its shotgun branch: `Shotgun(tier)` returned null and the code dereferenced `.Stats`. The four `resources/weapons/shotgun_tier_01.tres`–`04.tres` files and their C# script path exist on disk, so the stack trace does not establish why Godot failed to load one at runtime. Previously each attack tick performed a dynamic path load with no null guard. `scenes/main.tscn` now explicitly references all four editable profiles and passes them through `GameController.ShotgunTiers` to a one-time `WeaponData` cache. If an explicit entry is unavailable, the code attempts the original path once; if that also fails, it logs the tier and exact path and uses a provisional profile with values matching the corresponding `.tres`. This keeps weapon fire, shop descriptions, and tooltips from dereferencing a null resource. The designer should verify the resource loads without a fallback error; no agent run, build, compile, launch, or test was performed.

## Hossein and Saeed item revision (designer request, 2026-10-06)

`resources/items/hossein.tres` originally changed to 40 coins with +85 movement speed, +5 max HP, −3 signed HP regeneration/s, and −15 percentage points of all-weapon bonus damage. Negative regeneration still contributes to the stored total but never drains HP. `resources/items/saeed.tres` originally changed to 40 coins with Bullet pierce +2. Both later became unique; the subsequent 1.75× item-price revision makes each cost 70 coins. Their rarity 3, roles, and icon paths remain unchanged.

`PlayerStat.BulletPierce` was appended to the enum to preserve existing serialized stat numbers. A pistol bullet, SMG bullet, or shotgun pellet begins with the player's additive pierce count; zero retains one-hit destruction. Each successful hit is recorded by enemy instance ID, emits its existing hit effect, and consumes one pierce if the projectile continues. Saeed +2 permits three distinct enemy hits total and is limited to one purchase per run. Hits crossed in one frame are resolved in travel order. A spread child starts with the parent's configured pierce capacity and still ignores the original target for its full life; knife throws and hostile red envelopes are unaffected. Shop stats, offer text, and weapon tooltips show the current pierce count or hit capacity. Values and paths were reviewed statically only; the designer must verify behavior in game.

## Hossein and Saeed uniqueness (designer request, 2026-10-06)

Both item resources now save `Unique = true`. The existing `GameController.IsEligibleItem` and `ReplaceIneligibleOffers` paths exclude an owned unique item from normal rolls, editor-only Product Manager `P` rolls, and duplicate visible or locked offers. The purchase guard rechecks eligibility before charging coins. No stat, price, rarity, or icon values changed. The designer must verify this in game.

## Ten-wave HP and spawn escalation (designer request, 2026-10-06)

`WaveDefinition.EnemyHealthMultiplier` is Inspector-editable and multiplies each enemy scene's base HP at spawn. It also applies to the wave-1 test boss, later warned bosses, and many-eyes offspring. The per-group HP multipliers remain available for later tuning but are reset to their default 1.0 throughout waves 1–10. Waves 1–3 use 1, 1.35, and 1.8225. Beginning at wave 4, each wave's multiplier is exactly 2.5× the preceding value in the saved resources. The Inspector range was widened to 0.1–2000 to cover wave 10. Earlier five-wave HP ranges are superseded.

| Wave | HP multiplier | Approx. normal Jira HP | Provisional burst count | Tank chance | Boss chance / tank-boss chance |
| --- | ---: | ---: | ---: | ---: | ---: |
| 1 | 1.000000 | 10 | 4 | 0% | direct test boss |
| 2 | 1.350000 | 13.5 | 5 | 0% | 0% |
| 3 | 1.822500 | 18.225 | 5 | 18% | 55% / 45% |
| 4 | 4.55625 | 45.563 | 6 | 24% | 60% / 50% |
| 5 | 11.390625 | 113.906 | 7 | 30% | 65% / 55% |
| 6 | 28.4765625 | 284.766 | 7 | 35% | 70% / 60% |
| 7 | 71.19140625 | 711.914 | 7 | 40% | 75% / 65% |
| 8 | 177.978515625 | 1779.785 | 7 | 45% | 80% / 70% |
| 9 | 444.9462890625 | 4449.463 | 7 | 50% | 85% / 75% |
| 10 | 1112.36572265625 | 11123.657 | 7 | 55% | 90% / 80% |

Waves 6–10 reuse wave 5's 0/6/12/19/26/33/40-second warning schedule and existing male/female Jira, ranged slime, and many-eyes groups. Each later wave adds one Jira to every burst, roughly one ranged slime every second wave, and one many-eyes parent every other wave at the 19-second burst; exact counts remain editable in each resource. Speed and knockback resistance retain wave 5's values in waves 6–10. New wave names are blank pending designer naming. This spawn-density, tank-chance, and boss-chance progression is provisional.

Tank scene base HP is now exactly 2.5 times its normal counterpart: Jira 25/10, mail 37.5/15, many-eyes 60/24, one-eye offspring 10/4, and orange boss 300/120. Both Jira visual variants share the same values. Since normal and tank scenes receive the same wave multiplier, that 2.5× ratio holds in every wave where tanks are eligible. `scenes/main.tscn` lists all ten waves, the empty-array fallback loads 1–10, and the temporary finish copy uses the configured wave count. The developer reviewed files only; the designer must verify balance and progression in game.

## Item-price increase (designer request, 2026-10-06)

The 1.75× revision set item prices to `ceil(previous price × 1.75)` because shop prices are whole coins. Later explicit designer revisions set Behzad to 80 and Arsalan to 95; the other 24 active items retain their rounded 1.75× prices. The inactive +5% sample resource changed from its default 5 to 9, but remains outside the offer pool. `ShopItemDefinition.Price` defaults to 9 for future resources that omit an explicit price. Every active resource saves its price explicitly, including Narges, which previously inherited the five-coin default. `_docs/items-db.md` lists current costs. Weapon prices were tripled in a separate revision; reroll costs remain unchanged. The developer compared the saved item prices against the updated catalog without running project code.

## Weapon-price increase (designer request, 2026-10-06)

All weapon types use the same offer-price calculation. Tier I–IV costs changed from 6/10/14/18 to 18/30/42/54 coins, exactly 3× each prior value. `GameController.TierOneWeaponPrice` is exported at 18 and the new `WeaponTierPriceStep` is exported at 12, keeping the tier difference editable. Item prices, reroll costs, offer probabilities, and starting weapon selection are unaffected. The developer reviewed the price path statically; designer verification is pending.

## Behzad and Arsalan revision (designer request, 2026-10-06)

Behzad now costs 80 coins and grants +100 percentage points general bonus damage with −10 max HP. Arsalan costs 95 coins and replaces his former general damage bonus with +25 percentage points critical chance, retaining a revised −10 max HP tradeoff. Both remain rarity 3 and repeatable. The existing shop health-safety filter suppresses either offer when its −10 max HP would leave Mohsen with zero or less current or maximum HP; at the starting 10 HP, a max-HP gain is needed before either can be offered.

`PlayerStat.CriticalChancePercent` was appended to the enum to preserve existing serialized stat numbers; Arsalan saves `Stat = 9`. The player's bonus stacks additively with each weapon tier's base critical chance, and the resolved probability is clamped to 0–100%. Knife, pistol, SMG, and each shotgun pellet use that same resolved chance; the weapon's existing critical damage multiplier is unchanged. A projectile carries its rolled `CriticalHit` result through hits and Arezoo spread children. Enemy hit processing sends that result to the existing damage-number scene, which shows normal numbers white and critical numbers yellow while keeping the black outline and current animation. Shop stats, offer text, inventory item tooltips, and weapon tooltips display the new values. These changes were reviewed from files only; designer verification is pending.
