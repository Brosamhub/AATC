# AUDIENCE.md

## Who this game is for

**Medical-adjacent players** — gastroenterologists, GI fellows, residents, nurses, med students on their GI rotation, endoscopy techs, and curious patients who've had (or are about to have) a colonoscopy.

## One-sentence statement

*"A game for people who recognize the difference between a fixed sigmoid and a redundant colon, and who will show it to the person sitting next to them the moment they see it."*

## Demographics

- **Age:** 25-55, skewing 28-45.
- **Region:** US and English-speaking first; medical humor travels but vocabulary doesn't.
- **Role split (rough):** ~30% practicing GI / endoscopy staff, ~30% other medical (surgeons, IM, family med, med students), ~40% medical-curious (patients, caregivers, partners of medical professionals).
- **Device:** iPhone primary. iPad as a strong secondary (residents and fellows use iPads constantly). Android is a 3rd priority, not 2nd.

## Reference games they already play

- **Two Point Hospital** — the aspirational tone reference. They know it. Several have played it *because* they're in medicine.
- **Theme Hospital** (older cohort) — nostalgia.
- **Trauma Center** (DS) — closest procedural-surgical minigame ancestor. Niche but beloved in the crowd.
- **Surgeon Simulator** — played as a joke, often in med school. Reference for "the dumb version."
- **Wordle / NYT Games** — short daily-habit games. This is the session pattern to respect.
- **Pocket Plants / Unpacking** — craft-forward indie mobile. Premium-comfortable.

## Session pattern

- **Primary:** 5-15 min slot between patients, on rounds, during breaks, on the commute.
- **Secondary:** 20-30 min post-call wind-down.
- **Frequency:** 1-3 sessions/day for engaged players; bursty around shifts.
- **Implication:** day-loop granularity (1 day = 1 session) is the right frame. Don't require a 30+ min sitting to complete anything.

## What they want from this game

1. **Recognition humor.** "Oh my god, they know about the redundant sigmoid." The joke is *that the game knows*.
2. **Competence fantasy.** They are professionals. They want to feel skillful, not parodied. The game rewards technique and punishes sloppy work the way their actual job does.
3. **Respectful absurdity.** Patients can be weird (the guy with the coin, the over-sedated golfer), but never degraded. The humor is about the *job*, not the patients.
4. **Shareability with colleagues.** Screenshots and short clips that land in a group chat with three gastroenterologists and they all laugh. That's the viral surface.
5. **A little bit of pride.** They can say *"this is the only game that gets it"* and mean it.

## What they don't want

- Crass gross-out humor. Alienates the professional audience immediately. *(This aligns with `CLAUDE.md`: "Never graphic. Never mean-spirited. Never overly clinical in presentation.")*
- Fake medicine. If the anatomy naming, prep terminology, sedation protocols, or case tags are wrong, they'll notice in 30 seconds and quietly delete.
- Pay-to-win. This crowd has disposable income but pattern-matches F2P aggression as disrespectful to the craft.
- Patronizing tutorials. They already know what a colonoscopy is.
- Ads. Deal-breaker.

## Design filter (use on every decision)

Before committing to any design choice, ask:

1. **Would a GI attending roll their eyes at this?** If yes, fix it or cut it.
2. **Would a resident screenshot this for the group chat?** If yes, keep it. If nothing in the last 10 decisions would, the shareability muscle is atrophying.
3. **Is this technically plausible enough that an expert wouldn't wince?** "Plausible" is the bar, not "accurate." The game is a cartoon — but a cartoon drawn by someone who's been in the room.
4. **Does this respect the 5-15 min session?** If a new feature requires 30+ min of uninterrupted play, it's the wrong feature for this audience.

## Ceiling and floor

- **Ceiling (optimistic):** 50-200K lifetime installs. Strong word-of-mouth through medical social media, residency programs, conference booth demos. Premium priced correctly.
- **Floor:** 5-10K installs if marketing is organic and the game doesn't spread past the seed community.
- **LTV shape:** higher than casual-tycoon average because the audience has disposable income and low price sensitivity on niche cultural objects.
- **Not a mass-market hit.** That's a feature. Targeting casual mobile broad would dilute the only thing this game can be great at.

## Implications for downstream decisions

- **Monetization:** niche premium ($3-5) is the default. F2P-cosmetic is *possibly* viable if cosmetics are tastefully themed (different scope models, cartoon character customization for the doc and staff). F2P-ads is disqualified.
- **Signature moments:** lean toward *"detail they'll recognize"* over *"dumb thing they'll screenshot."* Both can exist, but the recognition moments are the primary viral surface.
- **Art direction:** Two Point Hospital family — stylized cartoon, distinctive silhouettes, readable on phone, technically literate (correct-ish anatomy, plausible equipment). Not Sim Hospital BuildIt generic.
- **Localization:** English-first. Spanish and Japanese are the next two candidates (large medical markets, comedy-friendly cultures). German and French later.
- **App Store positioning:** category is Simulation or Strategy, not Casual. Screenshots should emphasize the procedure minigame *and* the recognition humor, not the management layer.

## What could change this

If a playtest with ≥5 people from outside the medical-adjacent audience shows strong engagement and retention, the audience definition is too narrow and should be revisited. Default assumption until then: hold this audience.
