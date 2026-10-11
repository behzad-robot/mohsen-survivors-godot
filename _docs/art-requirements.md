# Art requirements and approved asset contract

**Owner:** artist for asset status and output details; designer for visual direction and approval. **Readers:** artist and developer. This is both the known art backlog and the file handoff contract based on `prd.md`. A row marked **Deferred** is a future need, not an instruction to start it.

## File workflow

1. Artist creates candidates under `_art/work/`, preserving the relative path shown below where practical.
2. Designer reviews a specific candidate and explicitly approves it or asks for a revision.
3. Artist places the approved export at the exact `assets/art/approved/` path and changes its status to **Approved**. Record the approved candidate or decision in the log below.
4. Developer may then integrate that approved path. Until then, the developer uses `assets/placeholders/` and does not reference `_art/work/`.
5. Artist begins the next asset task only when the designer asks for it. The order is Mohsen first, then floor/background and boundary wall after Mohsen's approval, then individual later requests.

Use transparent PNG for characters, weapons, enemies, pickups, and icons unless the designer and developer agree on another format. For animation, use one image per named state with horizontal, equal-size frames and record frame size, count, FPS, and pivot before approval. The first character contract is `mohsen_idle.png` and `mohsen_walk.png`; a static idle frame is acceptable as a candidate while the designer reviews his look.

## Initial asset list

| ID | Asset and purpose | Candidate path | Approved path expected by developer | Status | Missing specification |
| --- | --- | --- | --- | --- | --- |
| ART-001 | Mohsen without shoes: static idle now; walk later | `_art/work/characters/mohsen_idle.png`; original `_art/work/characters/mohsen_no_shoes.png` | `assets/art/approved/characters/mohsen_idle.png`; walk path reserved at `assets/art/approved/characters/mohsen_walk.png` | **Approved static idle; walk pending** | Walk frame count/FPS and animation details |
| ART-002 | Initiative enemy sprite | `_art/work/enemies/initiative.png` | `assets/art/approved/enemies/initiative.png` | Deferred | Designer will describe this character later |
| ART-003 | Jira ticket melee enemy sprite | `_art/work/enemies/jira_ticket_2x.png` | `assets/art/approved/enemies/jira_ticket.png` | **Approved; 2× export 2026-10-04** | — |
| ART-004 | Simple office floor/background | `_art/work/environment/office_floor.png` | `assets/art/approved/environment/office_floor.png` | **Approved** | — |
| ART-006 | Generic weapon icon | `_art/work/ui/weapon_icon.png` | `assets/art/approved/ui/weapon_icon.png` | Deferred | Default Godot UI or placeholders suffice now |
| ART-007 | Passive item icon, initially for the +5% damage sample item | `_art/work/ui/passive_icon.png` | `assets/art/approved/ui/passive_icon.png` | Deferred | Designer may define later item appearance; developer uses a placeholder now |
| ART-008 | Floating knife art | `_art/work/weapons/knife.png` | `assets/art/approved/weapons/knife.png` | **Approved** | — |
| ART-009 | Floating pistol art | `_art/work/weapons/pistol_v2.png` | `assets/art/approved/weapons/pistol.png` | **Approved V2 redesign 2026-10-04** | Developer display-scale integration |
| ART-010 | Floating SMG art | `_art/work/weapons/smg_uzi_v2.png` | `assets/art/approved/weapons/smg.png` | **Approved Uzi-style V2 2026-10-04** | Developer display-scale integration |
| ART-011 | Coin drop | `_art/work/pickups/coin.png` | `assets/art/approved/pickups/coin.png` | Deferred | Size and pickup presentation |
| ART-012 | Ranged enemy mail-envelope projectile | `_art/work/projectiles/enemy_projectile.png` | `assets/art/approved/projectiles/enemy_projectile.png` | **Approved** | ART-018 shoots it from its mouth |
| ART-013 | Rectangular arena boundary wall | `_art/work/environment/arena_wall.png` | `assets/art/approved/environment/arena_wall.png` | **Approved** | — |
| ART-014 | Victory-screen illustration | `_art/work/ui/victory.png` | `assets/art/approved/ui/victory.png` | Deferred | Designer will specify after player and enemy art |
| ART-015 | Game-over illustration: Mohsen buried in Jira tickets; Maryam announcing more tracking boards | `_art/work/ui/defeat_v2.png` | `assets/art/approved/ui/defeat.png` | **Approved V2 2026-10-10; developer handoff requested** | Final title/copy TBD; centered Play Again added in Godot |
| ART-016 | Shop lock-state icon | `_art/work/ui/shop_lock.png` | `assets/art/approved/ui/shop_lock.png` | Deferred | Default Godot UI suffices now |
| ART-017 | Red cross warning shown at each upcoming enemy spawn position | `_art/work/effects/spawn_warning.png` | `assets/art/approved/effects/spawn_warning.png` | Deferred | Final size, opacity, pulse/animation |
| ART-018 | Ranged mail slime enemy sprite | `_art/work/enemies/mail_slime_2x.png` | `assets/art/approved/enemies/mail_slime.png` | **Approved; 2× export 2026-10-04** | — |
| ART-019 | Shared player-bullet animation frame 1 | Designer-provided 2026-10-05 reference, top-left circular sprite | `assets/art/approved/projectiles/pistol_bullet.png` | **Designer-directed replacement 2026-10-05** | 64×64 transparent frame; used by both pistol and SMG bullet scenes |
| ART-020 | Shared player-bullet animation frame 2 | Designer-provided 2026-10-05 reference, top-right elongated sprite | `assets/art/approved/projectiles/smg_bullet.png` | **Designer-directed replacement 2026-10-05** | 64×64 transparent frame; used by both pistol and SMG bullet scenes |
| ART-021 | Enemy death explosion VFX | `_art/work/effects/enemy_explosion.png` | `assets/art/approved/effects/enemy_explosion.png` | **Approved 2026-10-04** | Developer integration and designer in-game verification |
| ART-022 | Large orange multi-mouth boss enemy | `_art/work/enemies/orange_boss.png` | `assets/art/approved/enemies/orange_boss.png` | **Approved 2026-10-04** | Developer integration; animation approach |
| ART-023 | Hamidreza shopkeeper portrait | `_art/work/characters/hamidreza_shopkeeper_v2.png` | `assets/art/approved/characters/hamidreza_shopkeeper.png` | **Approved V2 2026-10-04** | Developer shop UI integration |
| ART-024 | Shared pistol/SMG muzzle flash sprite | `_art/work/effects/muzzle_flash.png` | `assets/art/approved/effects/muzzle_flash.png` | **Approved 2026-10-04** | Developer integration; final in-game display scale and flash timing |
| ART-025 | Amir-Hossein teammate item portrait | `_art/work/items/teammates/amir-hossein.png` | `assets/art/approved/items/teammates/amir-hossein.png` | **Approved 2026-10-04** | Developer item integration; values from designer handoff |
| ART-026 | Arsalan teammate item portrait | `_art/work/items/teammates/arsalan.png` | `assets/art/approved/items/teammates/arsalan.png` | **Approved 2026-10-04** | Developer item integration; values from designer handoff |
| ART-027 | Hossein teammate item portrait | `_art/work/items/teammates/hossein.png` | `assets/art/approved/items/teammates/hossein.png` | **Approved 2026-10-04** | Developer item integration; values from designer handoff |
| ART-028 | Maryam teammate item portrait | `_art/work/items/teammates/maryam.png` | `assets/art/approved/items/teammates/maryam.png` | **Approved 2026-10-04** | Developer item integration; values from designer handoff |
| ART-029 | Milad teammate item portrait | `_art/work/items/teammates/milad.png` | `assets/art/approved/items/teammates/milad.png` | **Approved 2026-10-04** | Developer item integration; values from designer handoff |
| ART-030 | Mohammad-Hossein teammate item portrait | `_art/work/items/teammates/mohammad-hossein.png` | `assets/art/approved/items/teammates/mohammad-hossein.png` | **Approved 2026-10-04** | Developer item integration; values from designer handoff |
| ART-031 | Omid teammate item portrait | `_art/work/items/teammates/omid.png` | `assets/art/approved/items/teammates/omid.png` | **Approved 2026-10-04** | Developer item integration; values from designer handoff |
| ART-032 | Pooria teammate item portrait | `_art/work/items/teammates/pooria.png` | `assets/art/approved/items/teammates/pooria.png` | **Approved 2026-10-04** | Developer item integration; values from designer handoff |
| ART-033 | Sahar teammate item portrait | `_art/work/items/teammates/sahar.png` | `assets/art/approved/items/teammates/sahar.png` | **Approved 2026-10-04** | Developer item integration; values from designer handoff |
| ART-034 | Sara teammate item portrait | `_art/work/items/teammates/sara.png` | `assets/art/approved/items/teammates/sara.png` | **Approved 2026-10-04** | Developer item integration; values from designer handoff |
| ART-035 | Player bullet hit effect, four frames | Designer-provided 2026-10-05 reference, bottom row | `assets/art/approved/effects/player_bullet_hit.png` | **Designer-directed extraction 2026-10-05** | 256×64 transparent sheet, four 64×64 frames; developer timing provisional |
| ART-036 | Floating shotgun weapon sprite | `_art/work/weapons/shotgun_v2.png` | `assets/art/approved/weapons/shotgun.png` | **Approved short V2 2026-10-05** | Developer integration; gameplay tuning |
| ART-037 | Slim female Jira melee slime variant | `_art/work/enemies/jira_ticket_female.png` | `assets/art/approved/enemies/jira_ticket_female.png` | **Approved 2026-10-05** | Same logic as existing melee enemy |
| ART-038 | Five-eyed blue slime parent | `_art/work/enemies/many_eyes.png` | `assets/art/approved/enemies/many_eyes.png` | **Approved 2026-10-05** | Wanders; radial eight-bullet attack; spawns offspring on death |
| ART-039 | One-eyed blue offspring slime | `_art/work/enemies/one_eye_minion.png` | `assets/art/approved/enemies/one_eye_minion.png` | **Approved 2026-10-05** | Developer integration; stats and movement provisional |
| ART-040 | Behzad teammate item portrait | `_art/work/items/teammates/behzad.png` | `assets/art/approved/items/teammates/behzad.png` | **Approved V1 2026-10-05** | Developer item integration |
| ART-041 | Narges teammate item portrait | `_art/work/items/teammates/narges.png` | `assets/art/approved/items/teammates/narges.png` | **Approved V1 2026-10-05** | Developer item integration |
| ART-042 | Sahel teammate item portrait | `_art/work/items/teammates/sahel.png` | `assets/art/approved/items/teammates/sahel.png` | **Approved V1 2026-10-05** | Developer item integration |
| ART-043 | Saman teammate item portrait | `_art/work/items/teammates/saman.png` | `assets/art/approved/items/teammates/saman.png` | **Approved V1 2026-10-05** | Developer item integration |
| ART-044 | Animal-bone-helmet Jira ticket melee variant | `_art/work/enemies/tank_variants/jira_ticket_tank_v2.png` | `assets/art/approved/enemies/jira_ticket_tank.png` | **Approved V2 2026-10-06** | Chance to appear from wave 3; tank tuning provisional |
| ART-045 | Straw-hat female Jira melee variant | `_art/work/enemies/tank_variants/jira_ticket_female_tank_v2.png` | `assets/art/approved/enemies/jira_ticket_female_tank.png` | **Approved V2 2026-10-06** | Chance to appear from wave 3; tank tuning provisional |
| ART-046 | Helmeted ranged mail slime variant | `_art/work/enemies/tank_variants/mail_slime_tank.png` | `assets/art/approved/enemies/mail_slime_tank.png` | **Approved V1 2026-10-06** | Chance to appear from wave 3; tank tuning provisional |
| ART-047 | Helmeted many-eyes slime variant | `_art/work/enemies/tank_variants/many_eyes_tank.png` | `assets/art/approved/enemies/many_eyes_tank.png` | **Approved V1 2026-10-06** | Chance to appear from wave 3; tank tuning provisional |
| ART-048 | Helmeted one-eye offspring variant | `_art/work/enemies/tank_variants/one_eye_minion_tank.png` | `assets/art/approved/enemies/one_eye_minion_tank.png` | **Approved V1 2026-10-06** | Chance to appear from wave 3; tank tuning provisional |
| ART-049 | Helmeted orange boss variant | `_art/work/enemies/tank_variants/orange_boss_tank.png` | `assets/art/approved/enemies/orange_boss_tank.png` | **Approved V1 2026-10-06** | Eligible only from wave 3 when a boss spawn occurs |
| ART-050 | Office water dispenser sprite | `_art/work/objects/water_dispenser.png` | `assets/art/approved/objects/water_dispenser.png` | **Approved V1 2026-10-10; developer handoff requested** | 1–3 per wave except wave 1; water every 10 seconds |
| ART-051 | Coffee cup sprite | `_art/work/objects/coffee_cup.png` | `assets/art/approved/objects/coffee_cup.png` | **Approved V1 2026-10-10; developer handoff requested** | 10% enemy drop; +50% current speed for 5 seconds |
| ART-052 | Glass of water sprite | `_art/work/objects/glass_of_water.png` | `assets/art/approved/objects/glass_of_water.png` | **Approved V1 2026-10-10; developer handoff requested** | Heals 3 HP on collection |
| ART-053 | Strawberry milkshake sprite matching coffee cup style and size | `_art/work/objects/strawberry_milkshake.png` | `assets/art/approved/objects/strawberry_milkshake.png` | **Approved V1 2026-10-10; developer handoff requested** | Uniqueness TBD |
| ART-054 | Charging paper-shredder enemy visual | `_art/work/enemies/charge_enemy.png` | `assets/art/approved/enemies/charge_enemy.png` | **Approved V1 2026-10-10; developer handoff requested** | Display name and animation method TBD |
| ART-055 | Home screen illustration with title and space for a centered Play button | `_art/work/ui/home_screen_v6.png` | `assets/art/approved/ui/home_screen.png` | **Approved V6 2026-10-10; developer handoff requested** | Developer places live Play button over open center; designer in-game verification pending |
| ART-056 | Pizza item icon | `_art/work/items/pizza_v1.png` | `assets/art/approved/items/pizza.png` | **Approved V1 2026-10-11; developer handoff requested** | Uniqueness TBD |
| ART-057 | Hamburger item icon | `_art/work/items/hamburger_v1.png` | `assets/art/approved/items/hamburger.png` | **Approved V1 2026-10-11; developer handoff requested** | Uniqueness TBD |
| ART-058 | Team Building clasped-hands item icon | `_art/work/items/team_building_v1.png` | `assets/art/approved/items/team_building.png` | **Approved V1 2026-10-11; developer handoff requested** | Uniqueness TBD |
| ART-059 | Cup of tea item icon | `_art/work/items/tea_v1.png` | `assets/art/approved/items/tea.png` | **Approved V1 2026-10-11; developer handoff requested** | Uniqueness TBD |
| ART-060 | Beach straw hat item icon | `_art/work/items/beach_straw_hat_v1.png` | `assets/art/approved/items/beach_straw_hat.png` | **Approved V1 2026-10-11; developer handoff requested** | Uniqueness TBD |
| ART-061 | Apple item icon | `_art/work/items/apple_v1.png` | `assets/art/approved/items/apple.png` | **Approved V1 2026-10-11; developer handoff requested** | Uniqueness TBD |

## Art task backlog

### Six item icons — approved developer handoff (2026-10-11)

- ART-056–061 are six separate transparent 128×128 candidate PNGs: pizza, hamburger, Team Building clasped hands, tea, beach straw hat, and apple. Full-resolution source images are `_art/work/items/{slug}_concept_v1.png`; the unified preview is `_art/work/items/six_items_review_v1.png` and the prompts are saved in `_art/work/items/six_items_v1_prompts.md`.
- The designer approved all six V1 icons for `assets/art/approved/items/{pizza,hamburger,team_building,tea,beach_straw_hat,apple}.png`. Each item has rarity 2 and costs 20 coins. Effects: Pizza +20 max HP; Hamburger +10 movement speed and +10 max HP; Team Building −5 max HP and +2 HP regeneration/s; Tea +15 movement speed and −2 HP regeneration/s; Beach straw hat +5 percentage points critical chance; Apple +0 max HP and +0.05 HP regeneration/s. `_docs/items-db.md` records these exact values and the existing stat semantics. Uniqueness remains TBD.

### Sahar-2, Sepideh, Mania and Hojat teammate icons — approved developer handoff (2026-10-11)

- Designer requested portraits from the matching PNGs in `_art/team-mates-refrence-images/`. Candidates under `_art/work/items/teammates/`: `sahar-2.png`, `sepideh.png`, `mania.png`, and `hojat.png`. All use the existing teammate illustration style and transparent 128×128 square canvas. Full-resolution sources are `{slug}_concept_v1.png`; prompts are in `four_teammates_v1_prompts.md`.
- **Approved V1 2026-10-11.** Exports are `assets/art/approved/items/teammates/{sahar-2,sepideh,mania,hojat}.png`. Existing item effects remain unchanged. Sahar-2 is distinct from the existing Sahar portrait/item.

### Saeed and Arezoo teammate icons — approved developer handoff (2026-10-11)

- Designer requested both portraits from `_art/team-mates-refrence-images/saeed.png` and `arezoo.png`. Candidates: `_art/work/items/teammates/saeed.png` and `_art/work/items/teammates/arezoo.png`, transparent 128×128 square exports matching the existing teammate icon format. Each full-resolution source is named `{slug}_concept_v1.png`; prompts are in `saeed_arezoo_v1_prompts.md` alongside them.
- **Approved V1 2026-10-11.** Exports are `assets/art/approved/items/teammates/saeed.png` and `assets/art/approved/items/teammates/arezoo.png`. Existing item effects are unchanged. Saeed's source photo was also used in the approved home-screen illustration.

### Bluetooth headset item icon — approved developer handoff (2026-10-11)

- Designer requested artwork for a Bluetooth headset item. Candidate: `_art/work/items/bluetooth_headset_v1.png`, transparent 128×128 static icon. Full-resolution source: `_art/work/items/bluetooth_headset_concept_v1.png`; generation prompt saved alongside them. Charcoal-and-teal wireless over-ear office headset with padded earcups, microphone boom and blue indicator, matching existing bold cartoon item rendering.
- **Approved V1 2026-10-11.** Export: `assets/art/approved/items/bluetooth_headset.png`. The designer specified +5 percentage points critical chance, rarity 2, and cost 25 coins; `_docs/items-db.md` records the item. Uniqueness remains TBD.

### Game-over screen — approved developer handoff (2026-10-10)

- ART-015 approved export `assets/art/approved/ui/defeat.png` is the designer-approved V2, a 1280×720 opaque background illustration: Jira ticket piles fill the bottom and sides of a blue corporate office, with Mohsen's tired face and one reaching hand emerging on the right. Maryam is safely above the tickets on the left. Ticket labels use `SFIT-` followed by four digits from 1000–9999. The designer explicitly requested the hand for this illustration; gameplay sprites retain their existing presentation.
- Keep a centered Play Again control separate in Godot. Candidate leaves the middle clear of focal artwork, with a suggested button-safe rectangle around x=486–794, y=281–439. No title or button is baked into the image; final title/copy remains TBD.
- Maryam's upper speech bubble says: “We're going to add 3 more boards for tracking our progress of tracker board that's base on strategy tracker tracking..” This speech is part of the illustration; the centered Play Again control remains separate.
- Full-resolution V2 source: `_art/work/ui/defeat_concept_v2.png`; prompt: `_art/work/ui/defeat_v2_prompt.md`. V1 is preserved at `_art/work/ui/defeat.png` and `_art/work/ui/defeat_concept_v1.png`. Designer approved V2 and requested developer integration on 2026-10-10. No scene/script changes are included in the artist handoff; designer in-game verification remains pending.

### Home screen illustration — approved developer handoff (2026-10-10)

- ART-055 candidate V6 is an opaque 1280×720 image. It retains the exact title “Mohsen Survivors,” Hamidreza in a blue coat, and two neutral-faced Mohsens with hands passing a small pink motorbike/key between them; its rear box bears a white `!`. Sara is a ghost; happy Sahel and Sahar are the QA pair; Pooria and Narges exchange flying Jira tickets; Arsalan holds cash while writing a ledger; poker-faced Behzad and Saeed smoke together. It uses `_art/team-mates-refrence-images/saeed.png` for Saeed, includes Maryam at lower left raising a finger and Milad beside the upper-left group, and shows Hossein looking right with a normal-tone poker face. V5's tired Hossein and V4's simmering Hossein remain available for comparison.
- `assets/art/approved/ui/home_screen.png` is the approved opaque 1280×720 export. `_art/work/ui/home_screen_v6.png` is the approved candidate, `_art/work/ui/home_screen_concept_v6.png` is the high-resolution source, and `_art/work/ui/home_screen_cast_reference_v2.png` records the earlier portrait references. Earlier versions remain under `_art/work/ui/` for comparison. `_art/work/ui/home_screen_button_guide_v6.png` is a placement guide only, illustrating a 280×76 button centered at (640, 308) in the open area between the title and Mohsens; the approved image contains no button. The developer adds the live Play button in Godot. The designer also requested icons plus text on the starting weapon select screen; use the already approved knife, pistol, and SMG art paths, and include the approved shotgun art if it is currently selectable.

### Charge enemy — approved developer handoff (2026-10-10)

- ART-054 V1 is the approved compact runaway office paper shredder on two caster wheels. Its forward wedge points right, paper strips trail behind it, the slot forms an angry mouth, and a small red lamp and caution stripes mark the front. The transparent approved export is `assets/art/approved/enemies/charge_enemy.png`, 160×112 with about 120×95 visible pixels. `_art/work/enemies/charge_enemy_concept_v1.png` preserves the full-resolution source.
- `_art/work/enemies/charge_enemy_review_v1.png` shows the enlarged concept; `charge_enemy_game_scale_v1.png` presents illustrative approach, charge, and recovery poses in the 1280×720 office arena. The preview uses simple scaling and a direction arrow; these are not animation exports. The designer has now authorized the developer to implement `_docs/dev-feature-requests/task-charge-enemy.md`: pursue Mohsen, commit to a straight charge toward his captured position, stop for 3 seconds, then pursue again. Final display name and animation method remain TBD; gameplay tuning values are editable provisional choices.

### Strawberry milkshake — approved developer handoff (2026-10-10)

- ART-053 is a transparent 48×48 static PNG matching the coffee cup canvas and bold black-outlined cartoon style: pink milkshake in a clear rounded glass, whipped cream, strawberry garnish, and striped straw. Approved export: `assets/art/approved/objects/strawberry_milkshake.png`; full-resolution source: `_art/work/objects/strawberry_milkshake_concept_v1.png`. Generation prompt is saved alongside the candidate. Renumbered from ART-052 to resolve the concurrent glass-of-water entry's ID.
- The designer approved promotion and requested developer handoff as a new passive item: name `Strawberry milkshake`, description exactly `item`, +10 HP (max HP in the passive catalog), rarity 1, cost 20 coins. `_docs/items-db.md` records this addition. Uniqueness remains TBD. Earlier object candidates retain their existing status.

### Water dispenser, coffee cup, and glass of water — approved developer handoff (2026-10-10)

- ART-050 is a transparent front-facing white office water cooler with a blue jug, blue and red taps, and a dark drip tray. Its canvas is 160×104; the visible dispenser is about 35×90 pixels, matching the approved female Jira slime's approximately 90-pixel visible height. ART-051 is a transparent white ceramic coffee mug with brown coffee and a small steam curl on a 48×48 canvas; its visible pixels span about 33×38. ART-052 is a clear tumbler filled two-thirds with blue water on a 48×48 canvas; its visible pixels span about 31×36.
- V1 candidates and full-resolution `*_concept_v1.png` sources are under `_art/work/objects/`. `objects_review_v2.png` compares all three with the female slime at enlarged size, and `objects_game_scale_v2.png` shows native size in the 1280×720 arena.
- Designer approved all three and requested implementation on 2026-10-10. Approved PNGs are at the exact paths in ART-050–052. Full behavior handoff is `_docs/dev-feature-requests/task-water-and-coffee.md`: 1–3 dispensers per wave except wave 1, animated water drops every 10 seconds, water heals 3 HP, green player-healing numbers, and 10% enemy coffee drops granting +50% current movement speed for 5 seconds. No additional animation frames are required; the developer owns code-driven pickup animation.

### Helmeted tank enemy variants — approved developer handoff (2026-10-06)

- ART-044–049 cover all six currently approved enemy sprites: green Jira melee, coral female Jira melee, purple mail ranged, five-eyed blue parent, one-eyed blue offspring, and orange boss. The designer revised the green Jira variant to wear an ivory animal-skull-and-bone helmet with small horns. The female variant now wears a woven beach straw hat with a teal band and a turquoise crop top; her original white collar and black tie are removed. The other four retain their V1 metal knight or biker helmets. The designer approved all six and requested that they have a chance of appearing starting in wave 3. The chance and tank durability are provisional editable developer values; behavior should follow each corresponding base enemy unless the designer later specifies otherwise. The armored boss is eligible only if a boss spawn occurs at wave 3 or later; the wave-1 test boss remains its original visual.
- Current transparent candidates, full-resolution concept sources, and previous V1 drafts are under `_art/work/enemies/tank_variants/`. `tank_variants_comparison_v2.png` shows the current six against their originals; `tank_variants_game_scale_v2.png` shows them at native pixel size in the 1280×720 office arena. Export sizes: 160×112 for the four ordinary-sized enemies, 64×56 for the one-eyed minion, and 288×208 for the boss. The female hat is wider than her original head; body and overall display height remain close to the original.
- All six transparent runtime sprites are at the exact `assets/art/approved/enemies/*_tank.png` paths above and have been handed to the developer.

### Teammate portrait additions — approved (2026-10-05)

- Four new source photos were found: Behzad, Narges, Sahel, and Saman. The previous ten source hashes are unchanged. Narges and Sahel's tall photos were cropped to square face-and-upper-body references before stylization; all four 200×200 normalized reference crops are under `_art/work/items/teammates/reference-squares/`. Original reference photos were not changed.
- The designer approved ART-040–043 V1 and requested developer handoff. Their 128×128 transparent runtime exports are at the exact `assets/art/approved/items/teammates/{name}.png` paths in the table. The 384×384 candidates, full-resolution `*_concept_v1.png` sources, and `new_teammates_review_sheet_v1.png` remain in `_art/work/items/teammates/`. Source hashes and approval status are in `portrait_ledger.md` for the next folder update.

### Item icon batch — approved developer handoff (2026-10-05)

- The designer approved all four PNGs in this conversation and requested developer handoff with `_docs/items-db.md`. The earlier hold is lifted for this batch only; the four teammate candidates above are separate work and remain awaiting review.
- **Approved Hookah:** `_art/work/items/hookah_v1.png` copied to `assets/art/approved/items/hookah.png`. Teal glass base, brass stem and tray, clay bowl, curled dark hose. Role **Item**; +5 movement speed, −5 max HP, +2 knockback.
- **Approved Propranolol pills:** `_art/work/items/propranolol_pills_v1.png` copied to `assets/art/approved/items/propranolol_pills.png`. Medicine bottle and scored tablets. Role **Item**; −15 movement speed, +10 HP regeneration/s.
- **Approved Heavy Smoker V2:** `_art/work/items/heavy_smoker_v2.png` copied to `assets/art/approved/items/heavy_smoker.png`. Square cropped Mohsen portrait with five cigars, red bloodshot eyes, a comic frazzled expression, and curling smoke. V1 and the designer's comedy reference remain in `_art/work/` only. +27% bonus damage, −15 max HP; role remains TBD.
- All four item PNGs, including YAML below, are 1254×1254 transparent static images. Developer should fit them to the existing inventory/shop icon footprint. No animation is required.

### DevOps YAML item icon — approved developer handoff (2026-10-05)

- The designer assigned a shared `.yml` document icon for items with the DevOps role in this artist conversation. Candidate: `_art/work/items/devops/yml_file_v1.png`, transparent PNG, bold document outline with a folded corner, `.yml` lettering, and teal indented code marks.
- **Designer approved V1 and requested developer handoff.** Approved export: `assets/art/approved/items/devops/yml_file.png`. Use this shared icon for all items with role **DevOps**, preserving existing item modifiers.
- YAML is complete for review. The current assignment is the four teammate portrait additions above. Earlier candidates and approvals retain their existing status.

The list stays visible so the developer knows what approved files may arrive, but the artist starts only the currently assigned task.

1. **Latest handoff:** ART-037–039 are approved at their `assets/art/approved/enemies/` paths. The female Jira slime uses the existing melee logic with different visuals. The blue five-eyed slime wanders rather than following Mohsen, shoots eight bullets in a circle, and spawns many one-eyed small enemies on death. Exact timing, offspring count, and offspring tuning remain editable provisional values. The designer also requested wave 1 ordinary melee HP near 10 and ranged HP near 15.
2. **Latest handoff:** ART-036 shorter shotgun V2 is approved at `assets/art/approved/weapons/shotgun.png`. The designer requested high damage, short range, limited bullet lifetime, and a shotgun-like spread. The developer owns implementation and editable tuning values.
3. **Earlier handoff:** ART-025–034 are ten approved teammate item portraits at `assets/art/approved/items/teammates/{name}.png`. The designer explicitly requested implementation using `_docs/items-db.md`; use a placeholder for any item without an approved icon. Source-to-output status and SHA-256 hashes remain in `_art/work/items/teammates/portrait_ledger.md` so later additions to the reference folder can be identified without repeating finished work.
4. **Latest handoff:** ART-024 is one approved pistol/SMG muzzle flash at `assets/art/approved/effects/muzzle_flash.png`. Use the same static sprite for both guns when they fire.
5. **Latest handoff:** ART-023 Hamidreza's approved blue-coat portrait is at `assets/art/approved/characters/hamidreza_shopkeeper.png`. The designer asked the developer to show him in a shop UI corner and replace the shop's current `messageView` with a simple speech bubble above his head. On the player's first entry to the shop, the bubble says exactly: “welcome to VX the Greatest function of Snappfood”.
6. **Earlier handoff:** ART-022 orange boss slime is approved at `assets/art/approved/enemies/orange_boss.png`. The designer requested a bullet-hell boss that moves slightly slower than Mohsen, periodically stops for circular bullet volleys with safe gaps, and spawns at the start of wave 1 for testing. The designer also changed wave duration to 45 seconds. The developer owns gameplay implementation and tuning.
7. **Earlier handoff:** ART-021 enemy death explosion VFX was approved by the designer and is in `assets/art/approved/effects/enemy_explosion.png`. It is an eight-frame one-shot effect, not a gameplay projectile.
8. **Gun redesign handed over:** ART-009 pistol and ART-010 Uzi-style SMG V2 are approved at their existing developer paths. The developer was sent the larger texture dimensions and should keep their display footprint close to the previous guns.
9. **Earlier handoff completed by designer request on 2026-10-03:** Mohsen's no-shoes static idle, both slime enemies, office floor and wall, three floating weapons, red envelope projectile, and pistol/SMG bullets are in `assets/art/approved/` at the paths above. The developer should use the approved files, not `_art/work/` candidates.
10. **Mohsen animation still open:** `mohsen_idle.png` is the no-shoes 64×64 static export with 60-pixel visible height. No `mohsen_walk.png` exists yet. The developer may show the static idle during movement or add code-driven bobbing until the designer requests walk art. The cutout sheet and separate shoe art remain only in `_art/work/` and are not part of this handoff.
11. **Later, only when assigned by the designer:** ART-002 initiative enemy; ART-011 coin; ART-017 spawn warning; ART-006/007/016 optional UI icons; ART-014/015 finish illustrations. The first playable shop's sample passive item continues using a developer placeholder.

### Approved new enemy set (2026-10-05)

- `assets/art/approved/enemies/jira_ticket_female.png` is a transparent 160×104 coral slime with a notably narrower 65×90 visible silhouette, glasses, white collar, tie, and ticket. It is an alternate visual for the existing Jira melee logic.
- `assets/art/approved/enemies/many_eyes.png` is a transparent 160×104 blue parent slime with five distinct eyes and about 118×83 visible art. `assets/art/approved/enemies/one_eye_minion.png` is a transparent 64×48 matching blue offspring with one eye and about 44×33 visible art. The parent wanders, fires an eight-bullet circular volley, and spawns many offspring on death; timing and count are provisional developer choices until the designer tests them.
- No many-eyes projectile art is approved. The developer uses `assets/placeholders/many_eyes_bullet.svg`, a simple blue 20×20 orb. If the designer later requests final art, a transparent, centered projectile PNG at `assets/art/approved/projectiles/many_eyes_bullet.png` should fit the existing small circular visual and pivot; exact style and dimensions remain for designer approval.
- Matching full-resolution sources are `jira_ticket_female_concept_v1.png`, `many_eyes_concept_v1.png`, and `one_eye_minion_concept_v1.png` under `_art/work/enemies/`. `new_enemies_lineup_v1.png` shows enlarged comparison with the approved green melee enemy, while `new_enemies_game_scale_v1.png` shows all four at native 1280×720 scale.

### Approved shotgun V2 (2026-10-05)

- `assets/art/approved/weapons/shotgun.png` is a transparent 108×48 single sprite, facing right with centered framing. Its stock and barrel are shorter than V1 while the pump, receiver, and wood remain readable. The visible pixels occupy about 100×30 within the frame.
- `_art/work/weapons/shotgun_concept_v2.png` is the high-resolution source. `_art/work/weapons/shotgun_short_comparison_preview.png` compares V2 with V1 and the approved pistol and SMG. V1 remains under `_art/work/weapons/shotgun.png`. No separate shotgun projectile sprite has been approved; the developer may use existing approved bullet art or a placeholder while implementing the designer's short-range spread behavior.

### Approved teammate item portraits (2026-10-04)

- The ten transparent static upper-body PNGs under `assets/art/approved/items/teammates/` match the designer-provided photos while using the established Hamidreza cartoon rendering style. They were approved as 384×384 exports and resized in place to 128×128 on the designer's 2026-10-04 request; their exact filenames remain the slugs in ART-025–034 above. The artist does not set item effects or balance.
- `_art/work/items/teammates/teammates_review_sheet.png` presents all ten approved designs together. `_art/work/items/teammates/portrait_ledger.md` records the original delivery hashes; approved runtime PNG hashes changed after the 128×128 resize. For newly added photos, compare source filenames and hashes against the ledger, then create candidates only for new or changed sources.
- Each `*_concept_v1.png` alongside the candidates is its full-resolution source. Items in `_docs/items-db.md` without an approved portrait should use a developer placeholder until their art is requested and approved. `Sahar-2` is a separate item from `Sahar` and currently has no approved portrait.

### Approved shared muzzle flash (2026-10-04)

- `assets/art/approved/effects/muzzle_flash.png` is one transparent 32×28 sprite pointing right, with its ignition point on the left center. It uses a white core, bright yellow body, and orange tips for both pistol and SMG. No animation sheet is needed.
- `_art/work/effects/muzzle_flash_concept_v1.png` is the high-resolution source. `_art/work/effects/muzzle_flash_gun_preview.png` shows the sprite beside both approved gun sprites at the same scale, enlarged for review. The developer chooses the muzzle anchor and on-screen duration during integration.

### Approved Hamidreza shopkeeper (2026-10-04)

- `assets/art/approved/characters/hamidreza_shopkeeper.png` is the approved static transparent 384×351 upper-body portrait for a shop UI corner. It retains the photo's short dark hair, goatee, relaxed smile, and folded-arm crop, with the designer-requested white shirt and blue coat.
- `_art/work/characters/hamidreza_shopkeeper_concept_v2.png` is the 1312×1199 full-resolution source. The prior dark-outfit candidate and source remain as `hamidreza_shopkeeper.png` and `hamidreza_shopkeeper_concept_v1.png`.

### Approved orange boss enemy (2026-10-04)

- `assets/art/approved/enemies/orange_boss.png` is a transparent 288×192 sprite with about 249×180 pixels of visible art. It is roughly twice the height of the enlarged ordinary slime sprites in the 1280×720 design view.
- The three mouths, white hair, orange body, and blue striped long sleeves are intentional visual distinctions. `_art/work/enemies/orange_boss_concept_v1.png` is the high-resolution source, and `orange_boss_scale_preview_v1.png` compares it with Mohsen and both approved slimes.
- Designer gameplay direction: move around the arena slightly slower than Mohsen; periodically stop and fire multiple bullets in circular patterns with gaps a player can stand in; spawn at the beginning of wave 1 for testing. A separate boss bullet asset and animation frames have not been approved; the developer may use approved projectile art or a placeholder while implementing the behavior.

### Enemy resolution update (2026-10-04)

- The designer reported visible pixelation after enlarging the enemies and authorized direct developer handoff. The approved enemy PNGs were re-exported from their original high-resolution concepts at twice the previous frame dimensions, preserving the same designs and transparent margins.
- `enemies/jira_ticket.png`: 160×96 frame, about 99×84 visible art. `enemies/mail_slime.png`: 160×104 frame, about 121×92 visible art. Previous 1× exports remain in `_art/work/enemies/jira_ticket_1x.png` and `mail_slime_1x.png` for comparison.
- Developer integration: the larger source pixels should replace scene enlargement rather than multiply it. Current scene settings include `VisualScale = 2.0` for melee and `VisualScale = 1.12` for ranged; adjust the runtime visual scale so each approved 2× texture displays at the intended size. Keep collision geometry separately editable. The designer will verify the result in game.

### Approved combat asset layout

- The weapon sizes are knife 36×16, pistol 72×48, and SMG 88×56 pixels. Pistol and SMG are 2× source textures intended to retain roughly their former on-screen footprint after developer scale adjustment. The current player bullet PNGs are 64×64 transparent frames from the designer's later reference. The mail-envelope enemy projectile is 26×19 pixels, bright red with a pale fold and dark outline.
- These are one base visual per confirmed weapon type. Separate artwork for tiers I–IV has not been requested; the developer can reuse each type's PNG for its tiers until the designer decides otherwise.
- `_art/work/create_combat_art.py` is the editable source of the earlier weapon and projectile exports. `_art/work/combat_assets_sheet_v1.png` shows enlarged silhouettes and `_art/work/combat_scale_preview_v1.png` shows an illustrative game-scale composition. These previews are not gameplay placements or attack timing specifications.

### Designer-provided player bullet reference (2026-10-05)

- The designer asked the developer to extract and use the two top bullet sprites and four bottom hit frames from the attached pixel-art reference. The 64×64 blue cells were made transparent without redrawing their foreground pixels. The circular and elongated top sprites remain at `assets/art/approved/projectiles/pistol_bullet.png` and `assets/art/approved/projectiles/smg_bullet.png`. The designer clarified that these are consecutive animation frames for **both** player gun bullets; the filenames are retained as stable asset paths, not a weapon-specific visual mapping.
- The bottom four cells are combined left to right into `assets/art/approved/effects/player_bullet_hit.png`, a 256×64 transparent sheet. It is used only on player gun impacts; the enemy red-envelope art and thrown knife art are unchanged.

### Approved gun redesigns (2026-10-04)

- The designer approved a cooler pistol and an Uzi-style SMG. Their 2× exports are `assets/art/approved/weapons/pistol.png` (72×48) and `assets/art/approved/weapons/smg.png` (88×56). High-resolution concepts, earlier work files, and game-scale previews remain under `_art/work/weapons/`.
- Both face right and use center-oriented framing. The developer was asked to keep their display footprint close to the previous guns by adjusting the weapon visual node scale rather than letting the 2× texture display at 2× size.

### Approved enemy death VFX (2026-10-04)

- `assets/art/approved/effects/enemy_explosion.png` is a transparent 1024×128 PNG: eight horizontal equal-size 128×128 frames, centered pivot, suggested 12 FPS, one-shot with no loop. It is a generic orange-and-cream cartoon explosion for both slime enemy types.
- `_art/work/effects/enemy_explosion_storyboard.png` and `enemy_explosion_preview.gif` are review previews. `_art/work/effects/create_enemy_explosion.py` is the editable art source. Godot can play the PNG as frame animation; no particle system is required for this effect.

### Approved office environment layout

- `office_floor.png` is a seamless 256×256 opaque carpet repeat with quiet 128×128 square divisions.
- `arena_wall.png` is a transparent 3×3 atlas of 96×96 cells (288×288 total). Rows are top-left/top/top-right, left/empty/right, bottom-left/bottom/bottom-right. The empty middle cell reveals the floor. Boundary pieces are dark office partitions with an inner bevel and light floor-side shadow.
- `_art/work/environment/create_office_art.py` is the editable art source. The preview places all four boundaries in one viewport for visual review; it does not set the gameplay arena size.

The developer uses a simple red cross placeholder for ART-017 until approved art is delivered. At the configured burst time, crosses mark the randomly chosen spawn positions; they disappear after the editable warning period, 1 second by default, when enemies appear at those positions.

The three named starting weapons and one +5% bonus-damage sample item formed the earlier initial shop pool. The designer has now handed `_docs/items-db.md` to the developer for implementation, along with fourteen approved teammate item portraits. The sample item's art remains deferred. Default Godot controls and developer placeholders are sufficient for UI and any item without an approved export. Additional asset rows should be added as features are defined, with stable IDs and exact approved paths. ART-005 was retired because the first arena has no prop requirement.

The design view is 1280×720 native desktop. The intended style is close to Brotato and The Binding of Isaac: bold dark outlines, simple readable silhouettes, expressive character design, and a quiet floor that leaves enemies and pickups legible. Mohsen is human, not a potato. The gameplay reference screenshots and approximate display sizes are documented in `_art/reference-images/README.md`. The first draft target is Mohsen 55–65 display pixels tall and ordinary enemies 45–60; review final scale in context.

The initial game uses one rectangular arena larger than the camera view. It needs only a floor/background and boundary walls, without extra props. Mohsen flashes during post-hit invulnerability and shrinks to zero scale at death; the developer implements both without extra character frames. The shop needs four offer slots and a reroll control, but can use default Godot visuals for now. The designer has confirmed melee and ranged enemy behavior but will provide each enemy's appearance when its individual art task begins.

Mohsen should be drawn without hands. He needs idle and walk only, without attack, hit, or death frame sets. Weapons float separately; the knife moves toward an enemy while attacking. Enemy walk may use frame animation or code-driven scaling when those tasks arise. The designer will specify distinct victory and defeat illustrations and titles later; the developer renders title text and Play Again separately so copy can change without re-exporting art.

## Approval log

2026-10-10: Designer approved ART-015 game-over V2 and requested developer handoff. Approved export: `assets/art/approved/ui/defeat.png` (1280×720). Centered Play Again remains a Godot control; Maryam's speech is baked into the artwork.

2026-10-10: Designer approved ART-050–052 water dispenser, coffee cup, and glass of water V1 and requested developer implementation with the water/healing/coffee behavior in `_docs/dev-feature-requests/task-water-and-coffee.md`. Promoted to their `assets/art/approved/objects/` paths; designer gameplay verification remains pending.

2026-10-10: Designer approved ART-053 Strawberry milkshake V1 and requested developer handoff as a new item with description `item`, +10 HP, rarity 1, cost 20 coins. Approved export: `assets/art/approved/objects/strawberry_milkshake.png` (48×48 transparent PNG).

| Date | Asset ID | Candidate or version | Designer decision | Final path or requested change |
| --- | --- | --- | --- | --- |
| 2026-10-03 | ART-001 idle only | Mohsen no-shoes design; 64×64 static export | Designer requested handoff and specified no-shoes version | `assets/art/approved/characters/mohsen_idle.png`; walk pending |
| 2026-10-03 | ART-003, ART-018 | Green melee and purple ranged slime | Designer requested handoff | `assets/art/approved/enemies/jira_ticket.png`, `assets/art/approved/enemies/mail_slime.png` |
| 2026-10-03 | ART-004, ART-013 | Carpet tile and wall atlas | Designer requested handoff | `assets/art/approved/environment/office_floor.png`, `assets/art/approved/environment/arena_wall.png` |
| 2026-10-03 | ART-008, ART-009, ART-010 | Knife, pistol, SMG | Designer requested handoff | `assets/art/approved/weapons/knife.png`, `assets/art/approved/weapons/pistol.png`, `assets/art/approved/weapons/smg.png` |
| 2026-10-03 | ART-012, ART-019, ART-020 | Red mail envelope, pistol bullet, SMG bullet | Designer requested handoff | `assets/art/approved/projectiles/enemy_projectile.png`, `assets/art/approved/projectiles/pistol_bullet.png`, `assets/art/approved/projectiles/smg_bullet.png` |
| 2026-10-04 | ART-003, ART-018 | 2× enemy exports from original concepts | Designer requested resolution update and direct developer handoff | Same approved enemy paths; 160×96 melee and 160×104 ranged |
| 2026-10-04 | ART-009, ART-010 | Pistol V2 and Uzi-style SMG V2 | Designer approved and requested developer handoff | Same approved paths; 72×48 pistol and 88×56 SMG |
| 2026-10-04 | ART-021 | Eight-frame enemy death explosion | Designer approved and requested developer handoff | `assets/art/approved/effects/enemy_explosion.png` |
| 2026-10-04 | ART-022 | Orange, three-mouth boss with white hair and blue striped shirt | Designer requested developer handoff and specified boss behavior | `assets/art/approved/enemies/orange_boss.png` |
| 2026-10-04 | ART-023 | Hamidreza blue-coat V2 upper-body portrait | Designer approved and requested shop UI handoff | `assets/art/approved/characters/hamidreza_shopkeeper.png` |
| 2026-10-04 | ART-024 | Single shared pistol/SMG muzzle flash | Designer approved and requested developer handoff | `assets/art/approved/effects/muzzle_flash.png` |
| 2026-10-04 | ART-025–034 | Ten teammate item portraits, v1 | Designer approved the set and requested developer handoff with `_docs/items-db.md` | `assets/art/approved/items/teammates/{name}.png` for the ten listed slugs |
| 2026-10-05 | ART-036 | Shorter shotgun V2 | Designer requested handoff and specified high-damage short-range spread shots | `assets/art/approved/weapons/shotgun.png` |
| 2026-10-05 | ART-019, ART-020, ART-035 | Two top bullet sprites and four bottom hit frames from designer's reference image | Designer asked the developer to extract and use them | Replaced the two player bullet PNGs and added `assets/art/approved/effects/player_bullet_hit.png` |
| 2026-10-05 | ART-019, ART-020 | Clarified that both top bullet sprites are animation frames for both guns | Designer correction | Developer shares the two frames between separate pistol and SMG bullet scenes; no new art required |
| 2026-10-05 | ART-037–039 | Female Jira slime, five-eyed parent, one-eyed offspring | Designer approved and requested developer handoff with behavior notes | `assets/art/approved/enemies/jira_ticket_female.png`, `many_eyes.png`, `one_eye_minion.png` |
| 2026-10-06 | ART-044–049 | Six tank variants; green bone helmet and female straw-hat V2, other four V1 | Designer approved all and requested developer handoff with wave-3-or-later chance | `assets/art/approved/enemies/{jira_ticket,jira_ticket_female,mail_slime,many_eyes,one_eye_minion,orange_boss}_tank.png` |
| 2026-10-05 | ART-040–043 | Behzad, Narges, Sahel, Saman teammate portraits V1 | Designer requested developer handoff; approved 128×128 exports | `assets/art/approved/items/teammates/{behzad,narges,sahel,saman}.png` |
| 2026-10-05 | Item icon batch | YAML V1, Hookah V1, Propranolol pills V1, Heavy Smoker V2 | Designer approved all four PNGs and lifted the hold for developer handoff with database changes | `assets/art/approved/items/devops/yml_file.png`, `assets/art/approved/items/hookah.png`, `assets/art/approved/items/propranolol_pills.png`, `assets/art/approved/items/heavy_smoker.png` |
| 2026-10-10 | ART-054 | Charging paper-shredder enemy V1 | Designer approved and asked developer to read and implement `_docs/dev-feature-requests/task-charge-enemy.md` | `assets/art/approved/enemies/charge_enemy.png` |
| 2026-10-10 | ART-055 | Home screen illustration V6 | Designer approved and requested developer handoff, plus weapon-select icons with text | `assets/art/approved/ui/home_screen.png` |
| 2026-10-11 | Six teammate portraits | Saeed, Arezoo, Sahar-2, Sepideh, Mania, Hojat V1 | Designer approved and requested developer handoff | `assets/art/approved/items/teammates/{saeed,arezoo,sahar-2,sepideh,mania,hojat}.png` |
| 2026-10-11 | Bluetooth headset | 128×128 transparent V1 | Designer approved and specified +5 percentage points critical chance, rarity 2, cost 25; requested developer handoff | `assets/art/approved/items/bluetooth_headset.png` |
| 2026-10-11 | ART-056–061 | Six transparent 128×128 item icons V1 | Designer approved all; set rarity 2 and cost 20 for each; specified effects in `_docs/items-db.md` and requested developer handoff | `assets/art/approved/items/{pizza,hamburger,team_building,tea,beach_straw_hat,apple}.png` |

Developer technical note (2026-10-03): ART-008 knife art now needs to read clearly both beside Mohsen and as a rotating thrown projectile for up to 0.5 seconds. Keep pivot and transparent margins suitable for both uses; final visual design remains with the artist and designer.
