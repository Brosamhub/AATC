# SIGNATURE_MOMENTS.md

## Purpose

Three deliberately shareable moments. Each answers the Dream State question *"if a player shows a friend one screenshot or one 5-second clip, what is it?"* These are the viral surface of the game — without them, AATC is a quiet indie in a crowded category. With them, it has a discoverability engine.

**Design rule:** these moments exist for the audience in `AUDIENCE.md` (medical-adjacent). Each leans on *recognition humor* — "the game knows" — over gross-out comedy. All three are shippable within the existing engineering scope; no new systems required, just targeted polish + content.

---

## Moment 1: The Cecal Photo

**The moment a GI resident forwards to their whole program.**

### What triggers it
Successfully reaching the cecum on a case. Real GI docs take a photo of the ileocecal valve as proof of completion — it's called the "cecal photo" and it's a required part of the procedure note. The game ships this ritual as a first-class, collectible artifact.

### What the player sees
A brief celebratory beat: the scope pauses, the UI dims, a cartoon "photo" snaps into frame with a vintage Polaroid effect. The photo shows a stylized ileocecal valve — cartoon, not realistic. A caption auto-generates based on case tags:

- "Cecum, 14 min 22 sec. Redundant sigmoid negotiated." (Anatomy: Redundant Colon)
- "Cecum, 9 min 03 sec. Prep was, generously, fair." (Prep: Fair)
- "Cecum, 11 min 47 sec. Two polyps, one almost hiding." (Lesion: Multiple Polyps)
- "Cecum, 8 min 12 sec. Textbook." (Anatomy: Easy, Prep: Excellent)

The photo auto-saves to a permanent in-game "Case Album" the player can scroll through.

### Why they'd share it
- **Medical audience:** recognition hit. The game performs a ritual they perform daily. The caption reads like the note they just dictated.
- **Non-medical audience:** still funny — the Polaroid aesthetic + dry caption carries on its own. But lower hit rate.
- **Screenshot surface:** the photo is a single rectangular frame designed to fit Instagram/iMessage/group chat. Native share sheet from the Case Album.

### Why it works
- Tied directly to skill (you earned it).
- Combinatorial (every case generates a unique photo + caption).
- Collectible (the Case Album rewards play longevity without monetization).
- Technically plausible (real procedure ritual, cartoonified).

### Implementation notes (scoped, not a plan)
- New: a "Cecal Photo" generator triggered on cecum reached.
- New: Case Album UI screen.
- Existing: case tags already drive the caption variability (`CaseGenerator.cs` has anatomy/prep/lesion tags).
- Art: ~6-10 cartoon ileocecal valve illustrations (or a layered system that combines a base valve + prep-quality overlay + lesion markers). Ballpark: ~1 week of illustrator time, or generative-composed from a small set of parts.

---

## Moment 2: The Patient Archetype You've Seen

**The moment a GI attending texts it to a colleague with "lol this."**

### What triggers it
Specific patient archetypes trigger during case generation. Each archetype is a recognizable character — the kind of patient every GI has *actually* seen. When one appears, the case intro screen features a brief dossier on the patient that leans into recognition humor.

### What the player sees
The case intro card shows the patient's name, a cartoon portrait, and a short dossier. The dossier is written as if by the pre-op nurse who just interviewed them. Archetypes to start with:

- **"The Prep Optimist"** — *"Pt states they 'definitely did' the prep. Stool in rectum on exam suggests otherwise."* (Forces Prep: Poor)
- **"The Coffee Defender"** — *"Pt had 'just one coffee' at 6 AM. Pt had approximately four coffees."* (Mild sedation resistance)
- **"The Stoic"** — *"Pt reports being 'totally fine' with no sedation. Spoiler: they are not."* (Comfort drops faster)
- **"The Over-Preparer"** — *"Pt brought a printout of their family polyp history, color-coded."* (Grants a bonus to polyp detection; they were right to bring it)
- **"The Golfer"** — *"Pt asked if they can make an 11 AM tee time. It is currently 10:47."* (Rushing penalty if you slow down)
- **"The Returning Patient"** — *"Pt was here three years ago. Remembers your name. You do not remember theirs."* (Reputation multiplier; no gameplay penalty)

Each archetype is a single card you see for 2-3 seconds before the case begins. It's small. It's deliberate. It's shareable on its own even without playing.

### Why they'd share it
- **Medical audience:** every single one of these is a real patient type. The Prep Optimist gets passed around GI Twitter/X within a week of launch.
- **Non-medical audience:** the writing can carry these on its own — they read like a deadpan character comedy.
- **Screenshot surface:** the intro card is designed as a standalone shareable unit. Portrait + dossier, phone-sized, readable zoomed out.

### Why it works
- Zero-effort share — the joke is already visible without playing.
- Character-driven — each archetype has an identity the player remembers.
- Mechanically meaningful — each archetype is a gameplay modifier, so the joke and the challenge reinforce.
- Extensible — new archetypes are content, not new systems. Perfect for post-launch expansion.

### Implementation notes (scoped, not a plan)
- New: patient archetype table in `GameBalance.cs` (or a new `PatientArchetypes.cs`) — name, portrait reference, dossier text, gameplay modifier.
- New: case intro card UI (brief, pre-case).
- Existing: case generation already handles tag-based modifiers (`CaseGenerator.cs` + `ProcedureManager.cs`); archetype modifiers plug into the same path.
- Art: ~6-10 cartoon patient portraits to start. Distinct silhouettes, readable at phone size.
- Writing: ~1 day of dialogue polish per 6 archetypes. This is the single highest-leverage writing task in the game — get a good hand on it.

---

## Moment 3: The End-of-Day Stat Card

**The moment a player actually posts to social media.**

### What triggers it
End of each in-game day. Always. Non-negotiable.

### What the player sees
A single portrait-oriented card, phone-sized, designed as a standalone social post. Elements:

- **Header:** the date in-game + the clinic name.
- **Stats:** cases completed, polyps detected, cecal completion rate, average withdrawal quality, money earned, reputation delta.
- **The "Superpower" line:** one generated descriptor based on the day's performance — specific, funny, occasionally flattering. Examples:
  - *"Today's superpower: Cecum in under 10, every single time."* (fast completion streak)
  - *"Today's superpower: Found the polyp everyone else would've missed."* (rare detection)
  - *"Today's diagnosis: You need a coffee."* (high loop tension average)
  - *"Today's diagnosis: Patient comfort expert. They said 'that was fine.'"* (high comfort scores)
  - *"Today's reminder: Prep matters. Next time, bigger jug."* (multiple poor-prep cases)
- **Call-to-share:** a subtle "Share" button with native share sheet. Native platform share — don't overdesign it.

### Why they'd share it
- **Medical audience:** the Superpower line is the payoff. It's a personality test, it's a humble brag, it's a recognizable joke — all three at once. A good one gets posted.
- **Non-medical audience:** the card looks like Spotify Wrapped meets a GI report. Weird enough to be shareable as a curiosity.
- **Screenshot surface:** the card *is* the share. No framing needed. It is designed as the post.

### Why it works
- Daily cadence = daily share opportunity. Unlike the Cecal Photo (every case) or Archetype (some cases), this fires every session.
- Personalized — the Superpower line reflects *your* play, not a generic screen.
- Habit loop — the end-of-day card is a satisfying close to the session, making the session feel complete.
- Viral compounding — each shared card is a mini-ad for the game. The Superpower line + the clinic name + the stylized design = recognizable brand asset.

### Implementation notes (scoped, not a plan)
- New: end-of-day summary screen with card layout.
- New: Superpower line generator — table of conditions + generated lines.
- Existing: all the underlying stats (`GameManager.cs` tracks completed cases, cecal completion, withdrawal quality, money, reputation).
- Art: 1 high-quality card layout template, plus 4-6 variant "stamps" (clinic insignia, rare-performance badges).
- Writing: ~30-50 Superpower lines to start, triggered by stat thresholds. Keep adding over time.

---

## Priority order

If only one ships first, ship **Moment 3 (End-of-Day Stat Card)**. Reasoning:

- Highest frequency (every session vs. some cases vs. every case).
- Lowest implementation lift (no new gameplay systems, just a summary screen).
- Highest viral compounding rate (designed as a social post from the start).

If two ship, add **Moment 2 (Patient Archetypes)**. Reasoning:

- Character work differentiates the game more than any mechanic will.
- Natural extension point for post-launch content drops.

**Moment 1 (Cecal Photo)** is third because it requires the most art and the narrowest audience recognition payoff (it lands hardest with practicing GI specifically, less so with the medical-adjacent crowd at large).

All three should exist before v1 launch. They are the viral surface.

---

## What these moments are NOT

- **Not retention hooks.** Per `MONETIZATION.md`, there are no retention funnels in this game. The moments exist because they're fun to share, not because they gate play.
- **Not ads.** The share buttons are opt-in, native platform share. No forced share walls, no "share for reward" mechanics.
- **Not randomized loot.** The Cecal Photo collection is gated by skill (reach the cecum), not randomness. Archetypes are drawn from a weighted pool based on case difficulty, not gambling.
- **Not achievements.** Achievement systems are external scaffolding. These moments are *in* the game, *of* the game.

## Validation

A signature moment is working if:

- **Cecal Photo:** a playtester voluntarily scrolls back through the Case Album to look at old ones.
- **Patient Archetype:** a playtester reads the dossier out loud without being prompted.
- **End-of-Day Card:** a playtester screenshots it without being asked to share.

If none of these happen in a playtest, the signature moment is failing and needs rework. If one happens but the player doesn't *actually share it outside the room,* the moment is decorative and needs sharpening.

## What could change this

- If playtest signal says the audience wants gameplay depth over shareable moments, these get de-emphasized. Unlikely but possible — hold this priority until tested.
- If a genuinely better signature moment emerges during development (e.g., an animation of the scope navigating a redundant sigmoid turns out to be unexpectedly beautiful and captivating), promote it and demote the weakest of these three.
- Do not keep adding signature moments without cutting weaker ones. Three is the ceiling. More than three dilutes the hit rate.
