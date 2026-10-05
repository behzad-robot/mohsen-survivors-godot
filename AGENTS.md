# Mohsen Survivors: agent instructions

This is a Godot survivors game with Brotato as the main gameplay reference and Vampire Survivors as a secondary reference. Mohsen is a tech lead fighting office work made into enemies. The game designer is the user.

## Standing rules

Do not run, build, compile, launch, or test project code. The game designer runs and tests the game and reports results. Agents may inspect files and write files for their assigned work. Do not start feature implementation from this setup request; wait for a task in the relevant agent conversation.

The developer should write most scripts in C#. Use GDScript only when a required feature cannot be implemented through Godot's C# API, and document the specific limitation and why GDScript is needed in `_docs/tech-features.md`. Do not assume editor tools require GDScript; check C# support first.

The immediate target is the current native desktop Godot 4.6 project. Web export is a deferred goal and does not block C# gameplay implementation. If web export becomes a requirement later, revisit the engine/language compatibility before promising a web build.

## Choose a role in each Codex conversation

- **Game designer (user):** writes and approves decisions in `_docs/game-design-notes.md`, approves artwork, and provides test feedback.
- **Developer agent:** read `_agents/developer.md` and `_agents/artist.md` before engine work, along with the shared handoff docs. Owns Godot scenes, scripts, project setup, gameplay integration, and temporary placeholders. Reading the artist guide is for coordination; the developer does not take over art ownership.
- **Artist agent:** read `_agents/artist.md` before art work. Owns original visual assets and their work in progress. Artwork enters the developer's asset tree only after the game designer approves it.

If a role is not explicit, ask which role applies before changing game or art files. Follow the root instructions and the role guide together. Work in separate Codex conversations on this same project folder. Do not overwrite another role's in progress files.

## Source of truth and handoff

1. The designer writes `_docs/game-design-notes.md`. It is the source for game design decisions.
2. `_docs/prd.md` is the structured output of those notes and is shared by developer and artist. An agent may bring it into line with explicit notes, but must not invent or silently change design decisions. Mark unresolved details as **TBD** and ask the designer in that role's conversation.
3. `_docs/art-requirements.md` is the art handoff contract. Both agents read it. The artist uses it to create candidates; the developer uses approved asset paths listed there.
4. `_docs/tech-features.md` is the developer's implementation plan and technical decision log. It can fill in engineering detail that the PRD does not specify, but cannot override the PRD.
5. `_docs/developer-progress.md` is the persistent developer work tracker. The developer updates implementation checkboxes, current focus, provisional decisions, and handoff notes after each milestone. A task is marked designer-verified only after the designer reports a passing run.

When notes change, check whether the PRD and the two downstream docs need updating before implementing the affected work. Record open questions in the relevant doc without treating a proposed answer as approved.

## Project paths

| Path | Purpose | Primary owner |
| --- | --- | --- |
| `_docs/` | Design notes and shared handoff documents | Designer for notes; role owners for their handoff docs |
| `_agents/` | Role instructions for separate Codex conversations | Shared instructions |
| `_art/work/` | Art drafts and unapproved exports; never referenced by gameplay | Artist |
| `_art/reference-images/` | Saved gameplay style and scale references; do not use in game | Shared reference |
| `_art/mohsen-refrence-images/` | Designer-provided photos for Mohsen's character design | Designer reference |
| `assets/art/approved/` | Designer-approved, integration-ready art | Artist promotes after explicit designer approval |
| `assets/placeholders/` | Temporary visuals used while approved art is unavailable | Developer |
| `scenes/`, `scripts/`, `resources/` | Planned Godot implementation locations | Developer |

Only `assets/art/approved/` is the art contract for the developer. A file existing in `_art/work/` does not imply approval. The developer may use simple placeholders for any missing or unapproved asset. Keep references to final art paths in the art requirements document so approved replacements are straightforward.

The artist keeps a visible backlog in `_docs/art-requirements.md`, works one asset task at a time, and waits for the designer's approval before starting the next. The first art task is Mohsen's character. The developer may continue engine features with placeholders independently. `_art/reference-images/` contains gameplay references and `_art/mohsen-refrence-images/` contains designer-provided character references; neither is gameplay art.

Each spawn burst at time t selects its spawn positions, shows red crosses there, waits 1 second by default (editable per burst in the Godot Inspector), removes the crosses, and spawns enemies at those positions. The artist backlog includes a final warning-sign asset; use a developer placeholder until it is approved.

For the first playable version, the developer chooses and documents provisional difficulty values for waves 1–5 in editable wave resources. The longer target remains 20 waves; the designer will expand and tune them later. The initial shop pool uses the three confirmed weapon types plus one sample passive item that grants +5% bonus damage. Use a developer placeholder for that item's visual until the designer requests art.
