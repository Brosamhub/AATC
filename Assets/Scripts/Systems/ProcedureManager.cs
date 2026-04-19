using System;
using UnityEngine;

namespace AroundTheCorner
{
    public class ProcedureManager
    {
        public ProcedureState CurrentState { get; private set; }

        private System.Random random;

        public ProcedureState StartCase(CaseData caseData, StaffBonuses staffBonuses, UpgradeBonuses upgradeBonuses)
        {
            GameBalance.PrepDefinition prep = GameBalance.GetPrepDefinition(caseData.prep);
            GameBalance.SedationDefinition sedation = GameBalance.GetSedationDefinition(caseData.sedation);

            random = new System.Random(caseData.seed);
            CurrentState = new ProcedureState();
            CurrentState.caseData = caseData;
            CurrentState.staffBonuses = staffBonuses;
            CurrentState.upgradeBonuses = upgradeBonuses;
            CurrentState.phase = ProcedurePhase.Insertion;
            CurrentState.cecumReached = false;
            CurrentState.progress = 0f;
            CurrentState.loopTension = 8f;
            CurrentState.patientComfort = Mathf.Min(100f, sedation.startingComfort + upgradeBonuses.comfortBuffer);
            CurrentState.visibility = Mathf.Min(100f, prep.startingVisibility + upgradeBonuses.monitorVisibilityBonus);
            CurrentState.nextPushMultiplier = 1f;
            CurrentState.nextWithdrawalFocusBonus = 0f;
            CurrentState.sedateChargesRemaining = sedation.sedateCharges;
            CurrentState.detectedPolyps = 0;
            CurrentState.remainingPolyps = caseData.lesionCount;
            CurrentState.supplySpend = 0;
            CurrentState.withdrawalQualityAccumulated = 0f;
            CurrentState.withdrawalChecks = 0;
            CurrentState.statusText = "Scope in. Clean motions, clean day.";

            return CurrentState;
        }

        public ProcedureActionResolution ApplyAction(ProcedureAction action, int availableMoney)
        {
            ProcedureActionResolution resolution = new ProcedureActionResolution();
            if (CurrentState == null)
            {
                resolution.blocked = true;
                resolution.message = "No active case.";
                return resolution;
            }

            if (CurrentState.phase == ProcedurePhase.Complete || CurrentState.phase == ProcedurePhase.Failed)
            {
                resolution.blocked = true;
                resolution.message = "This case already wrapped.";
                return resolution;
            }

            switch (action)
            {
                case ProcedureAction.Push:
                    HandlePush(resolution);
                    break;
                case ProcedureAction.PullBack:
                    HandlePullBack(resolution);
                    break;
                case ProcedureAction.Steer:
                    HandleSteer(resolution);
                    break;
                case ProcedureAction.ApplyPressure:
                    HandlePressure(resolution);
                    break;
                case ProcedureAction.WashSuction:
                    HandleWashSuction(resolution);
                    break;
                case ProcedureAction.Sedate:
                    HandleSedate(availableMoney, resolution);
                    break;
            }

            if (!resolution.blocked)
            {
                ApplyPassiveStaffSupport();
                ClampState();
                EvaluateOutcome(resolution);
            }

            CurrentState.statusText = resolution.message;
            return resolution;
        }

        private void HandlePush(ProcedureActionResolution resolution)
        {
            if (CurrentState.phase == ProcedurePhase.Withdrawal)
            {
                CurrentState.visibility = Mathf.Max(0f, CurrentState.visibility - 6f);
                CurrentState.withdrawalQualityAccumulated = Mathf.Max(0f, CurrentState.withdrawalQualityAccumulated - 6f);
                resolution.message = "Wrong direction. Withdrawal means backing out with style, not charging back in.";
                return;
            }

            GameBalance.AnatomyDefinition anatomy = GameBalance.GetAnatomyDefinition(CurrentState.caseData.anatomy);
            GameBalance.PrepDefinition prep = GameBalance.GetPrepDefinition(CurrentState.caseData.prep);
            GameBalance.SedationDefinition sedation = GameBalance.GetSedationDefinition(CurrentState.caseData.sedation);

            float visibilityFactor = Mathf.Lerp(0.82f, 1.10f, CurrentState.visibility / 100f);
            float pushGain = GameBalance.BasePushGain
                             * anatomy.pushMultiplier
                             * visibilityFactor
                             * CurrentState.nextPushMultiplier;
            float loopGain = GameBalance.BaseLoopGain
                             * anatomy.loopMultiplier
                             * CurrentState.upgradeBonuses.loopReductionMultiplier;
            float comfortLoss = GameBalance.BasePushComfortLoss
                                * anatomy.discomfortMultiplier
                                * sedation.discomfortMultiplier;

            if (CurrentState.loopTension >= 70f)
            {
                comfortLoss += 7f;
                loopGain += 3f;
            }

            CurrentState.progress += pushGain;
            CurrentState.loopTension += loopGain;
            CurrentState.patientComfort -= comfortLoss;
            CurrentState.visibility -= prep.visibilityDecay * GameBalance.VisibilityLossOnPushMultiplier;
            CurrentState.nextPushMultiplier = 1f;

            if (CurrentState.progress >= 100f)
            {
                CurrentState.phase = ProcedurePhase.Withdrawal;
                CurrentState.cecumReached = true;
                CurrentState.progress = 0f;
                CurrentState.nextWithdrawalFocusBonus = 0f;
                CurrentState.withdrawalQualityAccumulated = Mathf.Max(0f, prep.qualityModifier + CurrentState.upgradeBonuses.qualityBonus);
                resolution.enteredWithdrawal = true;
                resolution.message = "Cecum reached. Easy now. Withdrawal quality pays the bills.";
                return;
            }

            resolution.message = "Push landed. Progress climbs, but the loop meter notices.";
        }

        private void HandlePullBack(ProcedureActionResolution resolution)
        {
            GameBalance.PrepDefinition prep = GameBalance.GetPrepDefinition(CurrentState.caseData.prep);

            if (CurrentState.phase == ProcedurePhase.Insertion)
            {
                CurrentState.progress = Mathf.Max(0f, CurrentState.progress - GameBalance.BasePullProgressLoss);
                CurrentState.loopTension -= GameBalance.BasePullLoopReduction;
                CurrentState.patientComfort += GameBalance.BasePullComfortRecovery;
                resolution.message = "Pull back. Less loop, more dignity.";
                return;
            }

            float visibilityScore = CurrentState.visibility * 0.52f;
            float qualityGain = visibilityScore
                                + CurrentState.nextWithdrawalFocusBonus
                                + prep.qualityModifier
                                + CurrentState.upgradeBonuses.qualityBonus;

            CurrentState.progress += GameBalance.BaseWithdrawalAdvance;
            CurrentState.loopTension = Mathf.Max(0f, CurrentState.loopTension - (GameBalance.BasePullLoopReduction * 0.4f));
            CurrentState.withdrawalQualityAccumulated += Mathf.Max(0f, qualityGain);
            CurrentState.withdrawalChecks += 1;
            CurrentState.visibility -= prep.visibilityDecay * GameBalance.VisibilityLossOnWithdrawalMultiplier;

            TryDetectLesion(qualityGain, resolution);
            CurrentState.nextWithdrawalFocusBonus = 0f;

            if (CurrentState.progress >= 100f)
            {
                CurrentState.phase = ProcedurePhase.Complete;
                resolution.caseEnded = true;
                resolution.result = BuildSuccessResult();
                resolution.message = resolution.result.summary;
                return;
            }

            if (string.IsNullOrEmpty(resolution.message))
            {
                resolution.message = "Steady withdrawal. Quality score climbing.";
            }
            else
            {
                resolution.message += " Withdrawal quality keeps climbing.";
            }
        }

        private void HandleSteer(ProcedureActionResolution resolution)
        {
            if (CurrentState.phase == ProcedurePhase.Insertion)
            {
                CurrentState.nextPushMultiplier = 1f + GameBalance.BaseSteerBonus + CurrentState.upgradeBonuses.steerBonus;
                CurrentState.loopTension = Mathf.Max(0f, CurrentState.loopTension - 4f);
                resolution.message = "Tip lined up. The next push should travel cleaner.";
                return;
            }

            CurrentState.nextWithdrawalFocusBonus += GameBalance.BaseWithdrawalFocusBonus + (CurrentState.upgradeBonuses.steerBonus * 30f);
            resolution.message = "You linger on the folds. Great setup for the next pull.";
        }

        private void HandlePressure(ProcedureActionResolution resolution)
        {
            if (CurrentState.phase == ProcedurePhase.Withdrawal)
            {
                resolution.message = "No heroic abdominal save needed on the way out.";
                return;
            }

            GameBalance.AnatomyDefinition anatomy = GameBalance.GetAnatomyDefinition(CurrentState.caseData.anatomy);
            bool sweetSpot = CurrentState.loopTension >= anatomy.pressureSweetSpotMin &&
                             CurrentState.loopTension <= anatomy.pressureSweetSpotMax;

            float reduction = GameBalance.BasePressureReduction * CurrentState.staffBonuses.pressureMultiplier;
            if (sweetSpot)
            {
                CurrentState.loopTension -= reduction;
                CurrentState.progress += 4f;
                resolution.message = "Nice counter-pressure. The scope straightens and buys a bit of progress.";
                return;
            }

            CurrentState.loopTension -= reduction * 0.45f;
            CurrentState.patientComfort -= 4f;
            resolution.message = "Pressure was late. Some help, some grumbling.";
        }

        private void HandleWashSuction(ProcedureActionResolution resolution)
        {
            float gain = GameBalance.BaseWashVisibilityGain
                         * CurrentState.staffBonuses.toolEfficiencyMultiplier
                         * CurrentState.upgradeBonuses.monitorWashMultiplier;

            CurrentState.visibility += gain;
            if (CurrentState.phase == ProcedurePhase.Withdrawal)
            {
                CurrentState.withdrawalQualityAccumulated += 8f + CurrentState.upgradeBonuses.qualityBonus;
                CurrentState.withdrawalChecks += 1;
            }

            resolution.message = "View cleaned up. The monitor looks a lot less cursed.";
        }

        private void HandleSedate(int availableMoney, ProcedureActionResolution resolution)
        {
            GameBalance.SedationDefinition sedation = GameBalance.GetSedationDefinition(CurrentState.caseData.sedation);

            if (CurrentState.sedateChargesRemaining <= 0)
            {
                resolution.blocked = true;
                resolution.message = "No rescue sedation charges left.";
                return;
            }

            if (availableMoney < sedation.doseCost)
            {
                resolution.blocked = true;
                resolution.message = "Not enough cash for another sedation top-up.";
                return;
            }

            CurrentState.sedateChargesRemaining -= 1;
            CurrentState.supplySpend += sedation.doseCost;
            CurrentState.patientComfort += sedation.doseEffect + CurrentState.upgradeBonuses.sedationDoseBonus;
            CurrentState.loopTension -= 5f;
            resolution.moneySpent = sedation.doseCost;
            resolution.message = "Comfort improved. Finance will survive.";
        }

        private void ApplyPassiveStaffSupport()
        {
            CurrentState.patientComfort += CurrentState.staffBonuses.comfortSupport;
        }

        private void EvaluateOutcome(ProcedureActionResolution resolution)
        {
            if (CurrentState.loopTension >= 100f)
            {
                CurrentState.phase = ProcedurePhase.Failed;
                resolution.caseEnded = true;
                resolution.result = BuildFailureResult(ProcedureOutcome.LoopComplication);
                resolution.message = resolution.result.summary;
                return;
            }

            if (CurrentState.patientComfort <= 0f)
            {
                CurrentState.phase = ProcedurePhase.Failed;
                resolution.caseEnded = true;
                resolution.result = BuildFailureResult(ProcedureOutcome.AbortedDiscomfort);
                resolution.message = resolution.result.summary;
            }
        }

        private void TryDetectLesion(float qualityGain, ProcedureActionResolution resolution)
        {
            if (CurrentState.remainingPolyps <= 0)
            {
                return;
            }

            GameBalance.PrepDefinition prep = GameBalance.GetPrepDefinition(CurrentState.caseData.prep);
            GameBalance.LesionDefinition lesion = GameBalance.GetLesionDefinition(CurrentState.caseData.lesion);

            float visibilityFactor = CurrentState.visibility / 100f;
            float qualityFactor = Mathf.Clamp01(qualityGain / 100f);
            float chance = 0.16f
                           + (visibilityFactor * 0.32f)
                           + (qualityFactor * 0.24f)
                           + prep.detectionModifier
                           + CurrentState.upgradeBonuses.detectionBonus
                           - lesion.detectionPenalty;
            chance = Mathf.Clamp(chance, 0.05f, 0.90f);

            if (random.NextDouble() <= chance)
            {
                CurrentState.remainingPolyps -= 1;
                CurrentState.detectedPolyps += 1;
                resolution.message = "Withdrawal stayed sharp. You spotted a polyp.";
            }
        }

        private ProcedureResult BuildSuccessResult()
        {
            float quality = Mathf.Clamp(CurrentState.GetAverageWithdrawalQuality(), 0f, 100f);
            ProcedureResult result = new ProcedureResult();
            result.caseTitle = CurrentState.caseData.title;
            result.outcome = ProcedureOutcome.Success;
            result.success = true;
            result.cecumReached = true;
            result.detectedPolyps = CurrentState.detectedPolyps;
            result.withdrawalQuality = quality;
            result.withdrawalRating = GetWithdrawalRating(quality);
            result.supplySpend = CurrentState.supplySpend;
            result.summary = "Case complete. Withdrawal rating: " + result.withdrawalRating + ".";
            return result;
        }

        private ProcedureResult BuildFailureResult(ProcedureOutcome outcome)
        {
            ProcedureResult result = new ProcedureResult();
            result.caseTitle = CurrentState.caseData.title;
            result.outcome = outcome;
            result.success = false;
            result.cecumReached = CurrentState.cecumReached;
            result.detectedPolyps = CurrentState.detectedPolyps;
            result.withdrawalQuality = Mathf.Clamp(CurrentState.GetAverageWithdrawalQuality(), 0f, 100f);
            result.withdrawalRating = GetWithdrawalRating(result.withdrawalQuality);
            result.supplySpend = CurrentState.supplySpend;

            if (outcome == ProcedureOutcome.AbortedDiscomfort)
            {
                result.summary = "Case aborted. The comfort meter hit the floor before the cecum.";
            }
            else
            {
                result.summary = "Complication. The loop meter maxed out and the room got very quiet.";
            }

            return result;
        }

        private static string GetWithdrawalRating(float quality)
        {
            if (quality >= 85f) return "Textbook";
            if (quality >= 70f) return "Clean and Careful";
            if (quality >= 55f) return "Serviceable";
            if (quality >= 40f) return "Rushed";
            return "Murky";
        }

        private void ClampState()
        {
            CurrentState.progress = Mathf.Clamp(CurrentState.progress, 0f, 100f);
            CurrentState.loopTension = Mathf.Clamp(CurrentState.loopTension, 0f, 110f);
            CurrentState.patientComfort = Mathf.Clamp(CurrentState.patientComfort, 0f, 100f);
            CurrentState.visibility = Mathf.Clamp(CurrentState.visibility, 0f, 100f);
            CurrentState.nextPushMultiplier = Mathf.Clamp(CurrentState.nextPushMultiplier, 1f, 1.5f);
        }
    }
}
