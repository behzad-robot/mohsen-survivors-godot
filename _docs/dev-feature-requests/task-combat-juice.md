# Combat feel: designer-approved developer task

**Designer approval:** 2026-10-05. The designer selected sound, short impact pauses, and an orange-boss firing telegraph from the proposed polish list. This request authorizes the developer to implement these three features. The other proposed polish ideas are outside this task.

## 1. Sound feedback

Add distinct sound cues for player weapon shots, enemy hits, enemy defeats, coin pickups, shop purchases, wave starts, and Mohsen taking damage. Give the orange boss's incoming volley a clear warning cue if it helps the visual telegraph. There are no current project audio assets, so use simple replaceable temporary sounds and keep the sound paths and volume/pitch parameters easy to change later. Avoid a loud overlapping stack when SMG shots or several coins occur rapidly. Do not ask the artist for audio or block on new art.

## 2. Brief impact pauses

Add a very short impact pause for high-impact events: a knife critical hit, a hit on the orange boss, and an enemy defeat. Ordinary SMG hits should remain light and should not repeatedly freeze the game. Use provisional, editable durations and avoid adding durations together when several impacts happen at once. The pause must not break wave timers, spawn warnings, the boss firing sequence, movement input, or transitions to the shop and finish screens. The designer will tune the feel after a manual run.

## 3. Orange-boss volley telegraph

Shortly before the **first** radial volley of each stopped firing phase, make the orange boss visibly flash red as a warning. The red warning is temporary and ends when firing begins. Preserve the existing safe gaps and multi-volley pattern; the first volley must wait until the warning has been visible. Use the existing boss sprite and a code or shader tint, with an editable provisional warning duration and flash rate. Keep the warning distinguishable from the existing white damage flash. No new art asset is required.

## Developer handoff and verification

- Work in C# by default and record provisional tuning and any technical decisions in `_docs/tech-features.md`.
- Update `_docs/developer-progress.md` with code-written and designer-verified status separately.
- Do not run, build, compile, launch, or test project code. Give the designer a short manual test checklist after implementation.
