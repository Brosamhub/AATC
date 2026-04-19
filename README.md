# ...and Around the Corner

A mobile-first Unity 2D vertical slice for a cartoon gastroenterology practice-management game. This prototype focuses on one playable day loop with multiple cases, simple procedure interactions, staff hiring, upgrades, and local persistence.

## How to Open and Run

1. Open the repository root in Unity Hub as an existing project.
2. Use Unity `6.3 LTS` (`6000.3.13f1`) or let Hub match the version pinned in `ProjectVersion.txt`.
3. Press Play from any scene. The prototype uses a runtime bootstrap, so the UI and managers are created in code on launch.
4. Start from the main menu, enter the clinic dashboard, then tap `Play Next Case` to run a day.

Notes:

- The UI is tuned for landscape mobile play and uses large touch targets.
- No backend, multiplayer, ads, or IAP are included in this milestone.
- Save data is written locally with `JsonUtility` to `Application.persistentDataPath`.

## Implemented Systems

- `GameManager`: overall flow, day lifecycle, screen switching, purchases, and persistence calls.
- `SaveManager`: local JSON save/load for money, staff, upgrades, and career stats.
- `CaseGenerator`: builds a daily lineup using anatomy, prep, sedation, and lesion tags.
- `ProcedureManager`: handles insertion and withdrawal gameplay with the required action buttons and failure states.
- `EconomyManager`: calculates payouts and reputation after each case.
- `StaffManager`: nurse and tech hiring, upkeep, and visible stat bonuses.
- `UpgradeManager`: scope, monitor, sedation support, and EMR upgrades with persistent levels.
- `GameUI`: runtime-generated Canvas UI for the main menu, clinic, procedure, staff, upgrades, and end-of-day screens.

## Folder Structure

```text
Assets/
  Scripts/
    Core/
      GameBootstrap.cs
      GameBalance.cs
      GameManager.cs
    Data/
      GameModels.cs
    Systems/
      SaveManager.cs
      CaseGenerator.cs
      ProcedureManager.cs
      EconomyManager.cs
      StaffManager.cs
      UpgradeManager.cs
    UI/
      UIFactory.cs
      GameUI.cs
Packages/
  manifest.json
ProjectSettings/
  ProjectVersion.txt
README.md
```

## Gameplay in This Milestone

- Main clinic dashboard with persistent stats.
- Multi-case day flow.
- Case tags that change comfort, looping, visibility, detection, and sedation feel.
- Procedure actions:
  - `Push`
  - `Pull Back`
  - `Steer`
  - `Apply Pressure`
  - `Wash / Suction`
  - `Sedate`
- Success and failure outcomes:
  - Successful completion
  - Aborted due to discomfort
  - Complication due to excessive looping
- Simplified withdrawal phase with quality scoring and lesion detection.
- Staff hiring and upgrade purchases that affect later runs.

## Balancing Constants to Tune Next

Most tuning lives in [`Assets/Scripts/Core/GameBalance.cs`](/Users/aaron/Projects/AATC/Assets/Scripts/Core/GameBalance.cs).

The first knobs to tune are:

- `BasePushGain`
- `BaseLoopGain`
- `BasePushComfortLoss`
- `BasePullLoopReduction`
- `BaseWashVisibilityGain`
- `BaseWithdrawalAdvance`
- staff hire costs and upkeep
- upgrade costs
- per-tag multipliers in the anatomy, prep, sedation, and lesion definitions

## Recommended Next Milestone

1. Replace runtime-only UI with hand-authored Unity scenes, prefabs, and polish animations.
2. Add a lightweight colon visualization and better feedback for insertion vs withdrawal.
3. Expand the career layer with staffing limits, room scheduling, and daily events.
4. Improve case variety with patient archetypes, room turnover, and more nuanced lesion outcomes.
5. Add audio, juice, and clearer onboarding/tutorial copy for first-time mobile players.
