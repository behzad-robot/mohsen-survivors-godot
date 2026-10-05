# Game design notes

**Owner:** game designer (user). Write new ideas and decisions here in your own words. Agents read this file as the design source and derive the shared PRD from it. A proposed idea is not a requirement until you mark it as a decision.

## Current confirmed decisions

- The game is a survivors-like game. Brotato is the main reference; Vampire Survivors is a secondary reference.
- The playable character is Mohsen, a tech lead in a corporate office world.
- Mohsen will have approximately 3–6 possible weapons and multiple passive items, similar in broad structure to Brotato.
- Enemies personify irritating corporate work, including initiatives and Jira tickets.
- The world and art have a corporate office theme.
- The developer can use simple placeholders while artwork is missing or unapproved.
- The designer approves artwork before the artist puts it in the developer's approved asset folder.
- The immediate target is the current native desktop Godot project. Web export is deferred for later; mobile is out of scope for now.
- Movement uses WASD or the arrow keys, similar to Brotato.
- At the start of a game, the player chooses one initial weapon from knife, pistol, and SMG.
- The knife is a short-range melee weapon intended to feel high damage. The later decision to use the Brotato wiki tier numbers controls its initial balance; the tier-I pistol has higher base damage than the tier-I knife. Review the feel after designer testing. Pistol and SMG projectile details still need definition.
- Use a 1280×720 (16:9) design view on desktop.
- The first playable flow is Play button -> choose one starting weapon -> wave 1 -> shop -> later waves. One environment is enough.
- The level is a simple rectangular, top-down 2D arena, somewhat larger than the visible screen, with walls that keep Mohsen inside.
- Weapons fire automatically. Mohsen may hold at most four weapons total.
- A run has 20 waves. Each wave lasts 65 seconds. Spawn bursts are authored per wave, with exact times and counts by melee/ranged behavior (for example, at t=0 spawn 2 melee and 3 ranged; at t=1 spawn 5 of each). Burst data also controls enemy HP, speed, and knockback resistance.
- The first playable version needs only waves 1–5. The developer chooses sensible, editable starting difficulty values and burst schedules for those five waves. The designer will later expand and tune the run to 20 waves.
- Each burst at t=x randomly chooses its enemy spawn positions and shows red crosses at those exact positions. After a warning period of 1 second by default, remove the crosses and spawn the enemies there. The warning duration is editable per burst in the Godot Inspector. Thus a t=0 burst starts its warning at wave start and spawns after that warning.
- Melee and ranged enemy scenes expose their tuning values in the editor, including ranged firing and retreat distances. Their scripts should support multiple scene variants with different sprites.
- All equipped weapons target the nearest enemy and fire automatically. Mohsen has no hands. The knife flies toward its target and uses a hitbox that can damage enemies along its path only while its attack is active, not during cooldown.
- Use all four tiers of the Brotato wiki's knife, pistol, and SMG numbers as the starting weapon balance, with higher-tier upgrades available during a run.
- For implementation, start with two enemy behavior types: melee enemies follow Mohsen; ranged enemies approach until they are within firing range, shoot, and retreat when Mohsen gets too close. Their visual identities are undecided.
- Every defeated enemy drops a coin. Mohsen must collect it by moving within a configurable pickup area slightly larger than his body. Uncollected coins are lost when the wave ends. Coins are spent in the shop between waves.
- The shop shows four randomly generated offers and a reroll button. Offers can be weapons or passive items; a shop is not guaranteed to contain a weapon. Mohsen can buy multiple offers per visit. Successive rerolls cost 5, then 10, 15, 20, and so on; whether that sequence resets for each shop visit is undecided.
- For the first playable version, the shop pool consists of the confirmed weapons plus one sample passive item that grants +5% bonus damage. A larger item catalog comes later. The developer may use placeholder art for this sample item.
- Each shop offer can be locked. A locked offer stays in its slot through rerolls and into the next shop after a wave until it is unlocked or bought. Bought slots stay empty until reroll or the next shop. A full four-weapon loadout blocks purchases of new weapons, but buying an identical tier I knife while holding one tier I knife automatically combines them into a tier II knife even when all slots are full. Weapon upgrades should behave similarly across tiers.
- Mohsen starts with 20 HP. Each enemy hit removes 1 HP, followed by a configurable invulnerable period (default 1 second) during which Mohsen's sprite flashes. HP regenerates during waves according to the HP regen stat. HP returns to full at the start of each new wave.
- Player stats initially include HP, flat movement speed, knockback value, bonus damage %, ranged bonus damage %, melee bonus damage %, and HP regeneration as a float per second. The stat set should remain extensible. Items usually change stats; some may have positive and negative effects together, and rare items may alter enemy stats or toggle GameManager flags.
- There are two finish screens: victory and defeat. They share a title, image, and Play Again button layout, but have different title text and pictures.
- The visual style should be as close as practical to the bold, simple cartoon look of Brotato and The Binding of Isaac, while Mohsen is a human rather than a potato. Brotato gameplay screenshots are saved in `_art/reference-images/`; three designer-provided Mohsen photos are in `_art/mohsen-refrence-images/`.
- Based on the saved Brotato screenshots, start Mohsen at approximately 55–65 pixels tall on screen in the 1280×720 view and ordinary enemies at approximately 45–60 pixels tall. Final size can change after the first character is reviewed in context.
- Keep a backlog of all known art needs in `_docs/art-requirements.md`, but make and approve art one task at a time. First create Mohsen, then after designer approval create one simple floor/background and a boundary wall for the arena. Enemy appearances, weapons, and other assets are later tasks initiated by the designer.
- Mohsen needs idle and walk animations only. He has no hand or attack animation. Code flashes him with transparency on hit and shrinks him to zero scale on death. Enemy walk animation may use frames or simple scaling, depending on each enemy design.
- Weapons float separately from Mohsen. Weapons, bullets, enemy projectiles, and coins should fit the approved style when their art tasks arrive.
- The arena only needs a simple floor and boundary walls; no extra props are required for now.
- Initial Play, weapon-select, shop, and HUD screens can use default Godot controls. Victory and defeat illustrations and titles will be specified after player and enemy art exist.

## Open design questions

- What exact titles and illustrations should the victory and defeat screens use once player and enemy art are in place?
- Which Mohsen candidate, outfit, palette, and final display dimensions does the designer approve?
- How should shop prices be set, which passive items exist first, and what happens when buying duplicate weapons while free slots remain?
- How are the wiki's flat damage scaling values mapped to this game's percentage-based melee and ranged bonus stats?
- How should the developer's provisional waves 1–5 be tuned after testing, and what should waves 6–20 contain later?
- Which visual enemies use the melee and ranged behavior types, and what does a ranged enemy shoot?
- What happens to coins between shops, and how many rerolls or purchases can occur in one visit?
- What should each enemy look like when its individual art task begins?

## New decisions

Add dated decisions here. If an earlier decision changes, say which one it replaces so the PRD can be updated without ambiguity.
