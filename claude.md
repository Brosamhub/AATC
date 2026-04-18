# CLAUDE.md

## REQUIRED WORKFLOW (DO NOT SKIP)

Before making ANY code changes, you MUST:

1. Read the following files:
   - GAME_SPEC.md
   - MILESTONES.md
   - BALANCE.md

2. Identify:
   - The current milestone being worked on
   - The specific feature requested

3. Follow these rules strictly:
   - Only implement features in the current milestone
   - Do NOT implement future milestone features
   - Do NOT invent new systems unless explicitly requested
   - Follow all balance philosophy from BALANCE.md
   - Follow UI/UX and tone from GAME_SPEC.md

4. If there is a conflict:
   - GAME_SPEC.md is the source of truth for design
   - MILESTONES.md is the source of truth for scope
   - BALANCE.md is the source of truth for tuning
   - Code must be adjusted to match these documents

5. If instructions are unclear:
   - Ask for clarification instead of guessing

## REQUIRED OUTPUT

After completing any task, you MUST include:

- What part of GAME_SPEC.md was used
- Which milestone item was implemented
- Which balance rules were applied
- What was intentionally NOT implemented (out-of-scope)

If this section is missing, the task is incomplete.

## TASK TEMPLATE

When given a request, interpret it as:

"We are working on [CURRENT MILESTONE].

Task:
[feature description]

Constraints:
- Follow GAME_SPEC.md
- Follow MILESTONES.md
- Follow BALANCE.md
- Do not add extra systems
- Keep implementation minimal and extensible"

## ANTI-OVERENGINEERING RULE

Do NOT:
- Add systems that are not required for the current milestone
- Generalize for future features prematurely
- Add complex architecture unless clearly needed

Prefer:
- Simple, direct implementations
- Clear and readable code
- Easy-to-modify balance values

## MILESTONE LOCK

Assume current milestone is:

Milestone 1 — Core Prototype

Do not implement features from later milestones unless explicitly told:
"Advance to next milestone"

## Project
"...and Around the Corner" is a mobile-first cartoon 2D management / tycoon game about running a gastroenterology practice and performing colonoscopies.

The game is stylized, humorous, and readable. It should not be graphic, realistic, or gross.

## High-level product goals
- Make a fun mobile game with short sessions
- Blend procedure gameplay with management and progression
- Keep the experience approachable and systems-driven
- Prefer clarity and tuning flexibility over realism
- Build in small vertical slices

## Current technical direction
- Engine: Unity
- Language: C#
- Platform target: mobile first
- Single-player only
- Local save only for now
- No backend unless explicitly added later
- No ads, IAP, analytics, or multiplayer in the first milestone

## Design pillars
1. Fast to understand
2. Easy to play on a phone
3. Interesting tradeoffs, not spam tapping
4. Readable UI
5. Data-driven tuning
6. Simple architecture over clever architecture

## Tone
- Cartoon
- Light satire
- Friendly and slightly absurd
- Never graphic
- Never mean-spirited
- Never overly clinical in presentation

## Core game loops

### Procedure loop
A case is a short interactive minigame with these primary meters:
- Progress
- Loop Tension
- Patient Comfort
- Visibility

Procedure actions:
- Push
- Pull Back
- Steer
- Apply Pressure
- Wash / Suction
- Sedate

Core balancing rule:
Every action should help one thing and hurt another.

### Day loop
The player completes a series of cases in a day, earns money and reputation, and reviews outcomes.

### Career loop
The player improves staff, equipment, and operations over time.

## First milestone scope
Only build:
- Main Clinic screen
- Procedure screen
- Staff screen
- Upgrades screen
- Results screen

Systems to include:
- case generation
- simple procedure gameplay
- staff hiring
- upgrades
- persistent local save
- high-level stats

Do not add:
- backend
- ads
- IAP
- live events
- multiplayer
- 3D
- unnecessary animation systems
- unnecessary plugin dependencies

## Case tags
Each case may include:
- Anatomy: Easy / Redundant Colon / Fixed Sigmoid
- Prep: Excellent / Fair / Poor
- Sedation: Light / Moderate / Propofol
- Lesion: None / Small Polyp / Multiple Polyps

These should affect gameplay in understandable ways.

## Staff in milestone 1
### Nurse
- comfort support
- pressure skill

### Tech
- turnover bonus
- tool efficiency

## Upgrades in milestone 1
- Scope Upgrade
- Monitor Upgrade
- Sedation Support Upgrade
- EMR Upgrade

## Metrics to track
- money
- reputation
- completed cases
- cecal completion rate
- polyp detection count
- average withdrawal quality

## Code guidelines
- Prefer simple, readable C#
- Keep classes focused
- Keep game constants centralized
- Use comments only when they clarify intent or non-obvious logic
- Avoid premature abstraction
- Avoid large monolithic scripts if a small split improves clarity
- Favor maintainability over cleverness
- Use data/config objects for balancing where practical

## UI guidelines
- Mobile-first
- Large tap targets
- Strong visual hierarchy
- Clear meter labels
- Minimal text during procedures
- Use placeholder art if needed
- Keep the presentation clean and cartoon-like

## Working style for changes
When implementing a task:
1. inspect current project structure
2. make the smallest clean change that solves the problem
3. preserve existing behavior unless intentionally changing it
4. update docs if the architecture or workflow changes

## What good output looks like
- project compiles
- the prototype is actually playable
- the codebase is easy to extend
- the tuning points are easy to find
- future milestones are obvious from the structure

## What to avoid
- overengineering
- building future features that were not requested
- introducing complex frameworks without necessity
- turning the project into a full hospital simulator too early
