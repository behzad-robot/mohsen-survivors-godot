# Gameplay reference images

These screenshots are for internal visual scale and style reference. They are not game assets and should not be referenced from Godot scenes or copied into `assets/art/approved/`.

| Local file | Source | Image size | Use |
| --- | --- | --- | --- |
| `brotato-gameplay-steam.jpg` | [Brotato Steam store screenshot](https://shared.fastly.steamstatic.com/store_item_assets/steam/apps/1942280/ss_61ad3d242282311207828c1a7d87c1c9d7b4d8bf.1920x1080.jpg?t=1771429106) | 1920×1080 | Mid-wave style, character/enemy proportions, on-screen density |
| `brotato-wave-1-wiki.jpg` | [Brotato Wiki wave-one screenshot](https://brotato.wiki.spellsandguns.com/File:20221125161202_1.jpg) | 1200×675 | Early-wave character and enemy scale |

## Approximate size targets at our 1280×720 design view

- Main character visual body: approximately **55–65 pixels tall**, around **45–55 pixels wide**, excluding floating weapons.
- Ordinary enemy: approximately **45–60 pixels tall**. Larger special enemies can be sized separately when designed.
- These are visual estimates from the screenshots after scaling their 16:9 images to 1280×720. They are starting proportions, not measured source sprite dimensions or collider specifications. Review Mohsen's first candidate in a 1280×720 mockup before locking scale.

The saved Brotato screenshots suggest simple silhouettes, thick dark outlines, and clear contrast between characters and a quiet floor. The Binding of Isaac is an additional style reference from the designer's brief. Mohsen should remain an original human character using the designer's photos in `_art/mohsen-refrence-images/` as appearance reference.

The **red crosses** visible on the floor in the Brotato screenshots warn where enemies will appear. In Mohsen Survivors, each burst chooses its positions when triggered, displays crosses for 1 second by default, then removes them and spawns enemies there. That duration is editable per burst. ART-017 in `_docs/art-requirements.md` tracks future art; the developer uses a red-cross placeholder first.
