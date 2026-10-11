# Wave damage and coin estimate

**Static estimate, updated 2026-10-11.** This is a balance sketch from the current weapon, wave, enemy, and coin-drop data. It is not a playtest result. Enemy HP uses each scene's `base + gain × (wave − 1)`, multiplied by that wave's editable danger HP multiplier (1.0 in waves 1–4; 1.2/1.2/1.5/2/2/3 in waves 5–10). Tank scenes have 2.5× both base and gain.

## Assumed loadout

Start with a tier I pistol. Buy **one weapon after every wave**: SMG I, shotgun I, knife I, then matching pistol I, SMG I, shotgun I, knife I, pistol II, and SMG II. Matching copies merge, leaving at most four equipped weapons. This assumes the wanted offers appear and that coins are available; actual shops only offer a weapon in about 40% of each slot before eligibility rules. No passive items, rerolls, pierce, spread, or multi-target knife hits are included. Tier I/II/III weapon offers cost 18/30/42 coins.

The DPS column uses the current base weapon damage, attack intervals, and average critical hits. It assumes every pistol, SMG, and knife attack hits, and **half of shotgun pellets hit**. Effective damage during a wave will vary with range, aim, movement, and target availability. The new Developer and DevOps damage bonuses are excluded so this is a weapon-only baseline. For example, one +12% all-damage Developer item raises this baseline by about 12% before other bonuses; Behzad's +200% bonus makes base weapon damage about 3× if his HP cost is survivable.

| Wave | Equipped tiers (pistol / SMG / shotgun / knife) | Approx. DPS | Jira HP | Mail HP | Tank Jira HP | Rough coins earned | Coins if scheduled foes all die |
| ---: | --- | ---: | ---: | ---: | ---: | ---: | ---: |
| 1 | I / — / — / — | 11 | 10 | 15 | — | 47 | 75 |
| 2 | I / I / — / — | 29 | 15 | 17 | — | 85 | 103 |
| 3 | I / I / I / — | 50 | 20 | 19 | 50 | 90 | 114 |
| 4 | I / I / I / I | 61 | 25 | 21 | 62.5 | 86 | 158 |
| 5 | II / I / I / I | 71 | 36 | 27.6 | 90 | 65 | 215 |
| 6 | II / II / I / I | 77 | 42 | 30 | 105 | 59 | 242 |
| 7 | II / II / II / I | 92 | 60 | 40.5 | 150 | 47 | 256 |
| 8 | II / II / II / II | 103 | 90 | 58 | 225 | 34 | 280 |
| 9 | III / II / II / II | 117 | 100 | 62 | 250 | 33 | 296 |
| 10 | III / III / II / II | 123 | 165 | 99 | 412.5 | 21 | 320 |

**Coin method:** each defeated enemy drops an average of 1.75 coins from the current 0/1/2/3 distribution. The last column is the scheduled burst count × 1.75, assuming every scheduled enemy dies and every coin is picked up. The rough earned column assumes 70% of the listed DPS is useful over the 45-second wave, estimates kills from the wave's spawn-weighted average HP including its tank chance, and assumes 85% of dropped coins are collected. Fractional values are expectations over many runs. Bosses and many-eyes offspring are excluded from both coin columns; they can add drops, while fighting them also consumes damage that would otherwise kill scheduled foes. Wave 1 has a guaranteed 120-HP boss; its provisional linear gain is 60 HP per wave. Mail and Jira now have distinct gains, while tank variants remain 2.5× their normal counterpart's HP.

**Balance read:** Before wave danger is applied, Jira gains a flat 5 HP per wave and mail slime gains 2. With the requested danger values, their displayed HP reaches 165 and 99 by wave 10. This example loadout grows from 11 to 123 DPS, while rough coin income falls to about 21 by wave 10 under the same simple assumptions. That is below a tier III weapon's 42-coin price; savings, item bonuses, and actual combat outcomes will matter. Real results may differ substantially, especially with bosses, offspring, item bonuses, and multi-target hits.
