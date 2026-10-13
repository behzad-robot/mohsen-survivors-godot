# GDScript / Web port handoff

Written 2026-10-13 at the designer's request in `mohsen-survivors-godot-gdscript`, branch `feat/gdscript-port`. No files in the sibling C# checkout were changed. No project code was run, built, compiled, launched, or tested.

## What changed

- Converted all 28 scripts under `scripts/data/` and `scripts/gameplay/` to `.gd`, retaining class names, Inspector property names, numeric enum values, tuning defaults, and script UIDs.
- Updated all script references in scenes, wave resources, item resources, and shotgun profiles. The resource content and approved art remain unchanged.
- Preserved the complete flow: menu, four starting weapons, ten authored waves, spawn warnings, bosses and tank variants, many-eyes offspring, charging enemies, coins/water/coffee, regeneration, combat feedback, shop locks/rerolls, stacking and unique items, weapon chain merges, victory/defeat, and editor-only cheat keys.
- Replaced C# records/tuples and LINQ with GDScript dictionaries, arrays, filtering and loops. Shotgun profiles retain their lazy-load fallback.
- Removed .NET project/solution files, C# scripts and sidecars, the empty `TestScript.cs` template, and .NET configuration in this checkout.
- Selected Compatibility rendering and added a Web preset with threads and extensions disabled. Existing desktop exports now use branch-local output paths. Exports are ignored and excluded from export inputs.

## Designer verification

1. Open **this folder's** `project.godot` with Godot 4.6. Allow the editor to scan/import the new scripts. The standard editor can now open the project without .NET. Report any parser/resource errors with their file and line before checking gameplay.
2. Run from the editor and try each starting weapon. Check movement, aiming, fire cadence, piercing, shotgun spread, hit feedback, HP/invulnerability/regeneration, death, and restart.
3. Check spawn crosses and their warning delays, boss volleys and gaps, charging targets, many-eyes volleys/offspring, and wave-3-or-later tank behavior. Use editor-only `C` for coins, `O` to reach the shop, `P` for Product Manager offers, and `R` to restart.
4. Check item prices/modifiers/tooltips, unique items, unsafe max-HP purchases, stacking, spread/pierce, locks across rerolls and waves, four-weapon limits, and chain merges. Check coffee refresh/expiration and water drops after wave 1.
5. Check victory and defeat artwork, Play Again, UI resizing, wave transitions, sounds, and later-wave difficulty.
6. Install export templates matching Godot 4.6 if needed. In **Project → Export**, select **Web** and export to `build/web/index.html`. Create `build/web/` if it is absent in a fresh checkout. The default HTML shell, single-threaded template, and Compatibility renderer are configured.
7. Serve the complete exported directory through Godot's Web run option or a local HTTP server; use HTTPS for hosting. Do not open the HTML directly through `file://`. Check keyboard focus, audio after clicking Play, all screens, browser console errors, resizing, and later-wave performance. Desktop browser controls remain keyboard-based; touch controls were not added.

The Web export configuration follows [Godot's official Web export documentation](https://docs.godotengine.org/en/stable/tutorials/export/exporting_for_web.html) and [Web exporter property reference](https://docs.godotengine.org/en/stable/classes/class_editorexportplatformweb.html). Export and runtime compatibility are **pending designer verification**, not claimed as tested.
