using UnityEngine;

namespace AroundTheCorner
{
    public class EconomyManager
    {
        public void ApplyProcedureOutcome(SaveData saveData, CaseData caseData, ProcedureResult result)
        {
            GameBalance.LesionDefinition lesion = GameBalance.GetLesionDefinition(caseData.lesion);

            int payout = caseData.basePayout;
            if (result.outcome == ProcedureOutcome.Success)
            {
                payout += 70;
            }
            else if (result.outcome == ProcedureOutcome.AbortedDiscomfort)
            {
                payout += 20;
            }
            else
            {
                payout += 10;
            }

            if (result.cecumReached)
            {
                payout += 22;
            }

            payout += Mathf.RoundToInt(result.withdrawalQuality * 0.35f);
            payout += result.detectedPolyps * lesion.payoutPerDetection;

            int reputationDelta = 0;
            if (result.outcome == ProcedureOutcome.Success)
            {
                reputationDelta += 2;
            }
            else if (result.outcome == ProcedureOutcome.AbortedDiscomfort)
            {
                reputationDelta -= 1;
            }
            else
            {
                reputationDelta -= 3;
            }

            if (result.cecumReached)
            {
                reputationDelta += 1;
            }

            if (result.withdrawalQuality >= 80f)
            {
                reputationDelta += 1;
            }

            reputationDelta += result.detectedPolyps;

            result.moneyEarned = payout;
            result.reputationChange = reputationDelta;

            saveData.money += payout;
            saveData.reputation = Mathf.Max(0, saveData.reputation + reputationDelta);
            saveData.totalCasesStarted += 1;

            if (result.outcome == ProcedureOutcome.Success)
            {
                saveData.completedCases += 1;
            }

            if (result.cecumReached)
            {
                saveData.cecalCompletions += 1;
                saveData.totalWithdrawalQuality += result.withdrawalQuality;
            }

            if (result.outcome == ProcedureOutcome.AbortedDiscomfort)
            {
                saveData.abortedCases += 1;
            }

            if (result.outcome == ProcedureOutcome.LoopComplication)
            {
                saveData.complications += 1;
            }

            saveData.detectedPolyps += result.detectedPolyps;
        }
    }
}
