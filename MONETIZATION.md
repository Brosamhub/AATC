# MONETIZATION.md

## Thesis (one line)

**Niche premium at $4.99, no IAP or ads at launch, optional cosmetic pack as a post-launch expansion.**

This matches the medical-adjacent audience defined in `AUDIENCE.md` — they have disposable income, low price sensitivity on niche cultural objects, and pattern-match F2P aggression as disrespectful. The whole product thesis rests on respecting their taste.

## Options compared

### A. Premium ($3.99–$4.99) — recommended

- **How it works:** one-time purchase. No IAP. No ads. Full game unlocked on install.
- **Economy design implications:**
  - Progression pacing tuned for *satisfaction*, not retention-grinding. Upgrades are unlocked by play, not walls you pay to skip.
  - Staff hire costs and upgrade costs ramp such that the full progression is completable in 4-8 hours of engaged play across ~2-4 weeks of sessions.
  - No "soft currency" and "hard currency" split. One currency (money).
  - Career loop milestones are pacing beats, not monetization prompts.
- **Risk:** lower revenue ceiling than F2P. Premium mobile is a narrow market.
- **Pro:** perfectly audience-fit. The audience will *advocate* for a premium game that respects them.
- **Pro:** design clarity. No compromise between "good for the player" and "good for the funnel."
- **Pro:** ships faster — no A/B infrastructure, no reward video SDK, no IAP receipt validation.
- **Revenue math:** at 50K lifetime installs × $4.99 × ~70% after platform cut ≈ **$175K**. At 10K × $4.99 × 70% ≈ **$35K**. Floor is modest; ceiling is reasonable for a niche.

### B. F2P with cosmetic pack ($0 install + optional $4.99 pack) — viable as a post-launch evolution

- **How it works:** free install, full gameplay unlocked. One optional pack at $4.99 with themed cosmetics (scope colors, doctor coat variants, clinic skins, patient archetype portraits). No gameplay effect.
- **Economy design implications:**
  - Same gameplay economy as premium. Cosmetics are *additive*, not gating.
  - No daily-login mechanics, no "free currency" economy. Just cosmetics.
- **Risk:** a cosmetic-only F2P on mobile usually doesn't hit conversion thresholds because no pressure exists to convert. Works better if the cosmetics are *desirable enough on their own* that players want them.
- **Pro:** wider top-of-funnel. More people try it because it's free.
- **Pro:** cosmetics can become a word-of-mouth vector ("my scope is the gold one").
- **Con:** free install invites audience misfit. Casual players try it, bounce, leave 1-star reviews about the "weird colonoscopy game" that drag discoverability.
- **Verdict:** good *as a post-launch move* after the premium audience has seeded word-of-mouth and App Store positioning is stable. Not a launch strategy.

### C. F2P with ads and/or consumable IAP — disqualified

- **Why disqualified:** the audience (per `AUDIENCE.md`) has ads as an explicit dealbreaker. Interstitial ads in a colonoscopy game would also be a content moderation nightmare (ads don't know your game is about medicine — you'd get weight-loss pills and telehealth promos between cases, which is tonally catastrophic).
- **Don't revisit.** This is a no.

## Recommended path

1. **Launch v1 as premium** at $3.99–$4.99. Single-tier. No discount launch promo — premium niche games do better at full price because it signals the product has respect for itself.
2. **Post-launch (~6 months after, contingent on signal):** add a cosmetic pack at $4.99. Not a sequel, not a DLC in the traditional sense — a small "Studio Pack" with 4-8 themed cosmetics. Sold once, permanent.
3. **Never add ads. Never add consumable IAP.** These are the permanent tripwires that would break the audience promise.

## Economy design ripple (affects Milestone 1 tuning NOW)

Per `BALANCE.md`, the current economy is action-reward-penalty with base rewards, quality bonuses, failure penalties. Under premium monetization:

- **No retention-grinding.** Case payouts should feel proportional to effort. Completion in ~4-8 hours of play across ~2-4 weeks of real time is the target pacing for full progression.
- **No pay-to-skip pressure points.** If a player feels "I need to grind or pay to get past this," that's a bug under premium. In the current model (no IAP at all), it's just a pacing bug — but the fix is "tune the numbers," not "add a shortcut SKU."
- **Staff hire and upgrade costs should cap out visibly.** A player who plays through the full progression should see the ceiling. No endless number-go-up.
- **Failure should teach, not punish.** A failed case costs reputation and some money, but shouldn't feel like losing an hour of progress. Premium audiences tolerate difficulty; they don't tolerate wasted time.

## Implications for Milestones 2-4

- **M2 (Management):** scheduling and turnover can deepen the strategic layer without turning into grind. Day length stays 5-15 min.
- **M3 (Variety):** more cases, events, and difficulty scaling — good. No limited-time events that require daily check-ins. Events are *content*, not *retention traps*.
- **M4 (Expansion):** multiple rooms and career paths become the progression *content* — the player works through them and finishes them. No prestige/reset loops unless the playtest specifically signals the audience wants replay.

## Revenue tracking (post-launch — not a launch feature)

- **Key metric:** units sold per week. Not DAU, not retention, not ARPDAU. Units.
- **Secondary:** completion rate (what % of buyers finish the progression). Low completion rate = pacing is wrong.
- **No analytics SDK at launch.** Audience-appropriate. If cosmetic pack ships, add minimal first-party purchase telemetry only.

## What could change this

- If a playtest shows the game is genuinely enjoyed by a broader audience beyond medical-adjacent, F2P-cosmetic becomes a stronger launch option. Revisit `AUDIENCE.md` first.
- If App Store featuring opportunities are contingent on free download (they sometimes are), F2P-cosmetic as a launch strategy becomes a tactical question. Evaluate at the time.
- **Do not** revisit ads or consumable IAP. That would require redefining the audience, which would require redefining the product.
