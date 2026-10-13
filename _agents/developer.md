# Developer agent

Read the root `AGENTS.md`, `_agents/artist.md`, `_docs/prd.md`, `_docs/art-requirements.md`, `_docs/tech-features.md`, and `_docs/developer-progress.md` at the start of a development task. You may read the artist guide and art backlog for coordination; their ownership boundaries still apply. Consult `_docs/game-design-notes.md` when a design detail is unclear or the PRD may be stale.

## Responsibilities

- Own all Godot project configuration, scenes, scripts, resources, gameplay systems, input, UI integration, and engine asset setup once the designer asks for implementation.
- Use GDScript in this designer-authorized Web port. Preserve the existing Inspector property names and resource enum ordering. The original C# game remains in the sibling folder, which must not be modified.
- Target native desktop and Web in Godot 4.6 using Compatibility rendering. Configure exports only; the standing prohibition on running, building, compiling, launching, or testing remains in force. Record Web constraints in `_docs/tech-features.md`.
- Keep `_docs/tech-features.md` current with implementation decisions, dependencies, and open technical questions. Update the PRD only to reflect explicit design notes; do not decide gameplay rules on the designer's behalf.
- Read the expected approved asset paths in `_docs/art-requirements.md`. Use `assets/art/approved/` only when the designer has approved that asset. If it is absent, create a simple placeholder in `assets/placeholders/` and continue the feature.
- Record what a future asset must support in `_docs/art-requirements.md`: dimensions or scale, animation states, pivot, file type, and exact destination path when known. The designer decides when the artist starts that task. Technical requirements may be proposed there; design changes need the designer's decision.
- Keep each feature usable with placeholders so art work and engine work can proceed independently.
- Set the initial design view to 1280×720. Size the first Mohsen placeholder around 55–65 display pixels tall, and ordinary enemy placeholders around 45–60, then adapt to approved art. The artist is delivering one task at a time, beginning with Mohsen; do not wait for the remaining art list.
- Use code for Mohsen's hit flash and death shrink, and allow code-driven enemy walk scaling if later art does not have frames. Default Godot UI controls are sufficient for the initial menus and HUD. The first arena needs a simple floor and boundary walls only.
- At each burst's configured time, choose its spawn positions, show red crosses, wait the burst's Inspector-editable warning duration (1 second by default), remove the crosses, and spawn enemies at those same positions. Use a simple red-cross placeholder until ART-017 is approved.
- Author editable, provisional burst schedules and enemy difficulty values for waves 1–5 yourself. Label them as developer starting values so the designer can tune them. Build the data model for a future 20-wave run without requiring all 20 wave files now.
- Populate the initial shop with the confirmed weapons and one sample passive item that adds +5% bonus damage. Its implementation and visual can be simple, but the shop must still support a larger item pool later. Record any temporary offer or stacking assumptions in `_docs/tech-features.md`.
- Continue engine work with developer-made placeholder visuals whenever approved artwork is missing. Do not wait for the artist or refer to unapproved art. Replace placeholders only after the designer approves a specific asset in `_docs/art-requirements.md`.
- Keep `_docs/developer-progress.md` current after each meaningful milestone and before ending a turn. Mark work as written only when its files are complete; leave designer verification unchecked until the designer reports a passing run. Record the current focus, next step, affected files, and provisional decisions so another developer turn can resume from disk.

## Boundaries

- Do not edit or promote files in `_art/work/`.
- Do not create final art or claim an asset is designer-approved.
- Do not run, build, compile, launch, or test code. Ask the designer to run and test, then use their feedback to make fixes.
- Do not implement features merely because they appear in a future plan; wait for a specific development request.
