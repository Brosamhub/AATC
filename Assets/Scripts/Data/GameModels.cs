using System;
using System.Collections.Generic;

namespace AroundTheCorner
{
    public enum AppScreen
    {
        MainMenu,
        Clinic,
        Procedure,
        Staff,
        Upgrades,
        EndOfDay
    }

    public enum StaffType
    {
        Nurse,
        Tech
    }

    public enum UpgradeType
    {
        Scope,
        Monitor,
        SedationSupport,
        EMR
    }

    public enum CaseAnatomy
    {
        Easy,
        RedundantColon,
        FixedSigmoid
    }

    public enum PrepQuality
    {
        Excellent,
        Fair,
        Poor
    }

    public enum SedationProfile
    {
        Light,
        Moderate,
        Propofol
    }

    public enum LesionProfile
    {
        None,
        SmallPolyp,
        MultiplePolyps
    }

    public enum ProcedurePhase
    {
        Insertion,
        Withdrawal,
        Complete,
        Failed
    }

    public enum ProcedureOutcome
    {
        Success,
        AbortedDiscomfort,
        LoopComplication
    }

    public enum ProcedureAction
    {
        Push,
        PullBack,
        Steer,
        ApplyPressure,
        WashSuction,
        Sedate
    }

    [Serializable]
    public class SaveData
    {
        public int money;
        public int reputation;
        public int currentDayNumber;
        public int nurseCount;
        public int techCount;
        public int scopeUpgradeLevel;
        public int monitorUpgradeLevel;
        public int sedationSupportUpgradeLevel;
        public int emrUpgradeLevel;
        public int daysCompleted;
        public int totalCasesStarted;
        public int completedCases;
        public int cecalCompletions;
        public int detectedPolyps;
        public float totalWithdrawalQuality;
        public int complications;
        public int abortedCases;

        public float GetCecalCompletionRate()
        {
            if (totalCasesStarted <= 0)
            {
                return 0f;
            }

            return (float)cecalCompletions / totalCasesStarted;
        }

        public float GetAverageWithdrawalQuality()
        {
            if (cecalCompletions <= 0)
            {
                return 0f;
            }

            return totalWithdrawalQuality / cecalCompletions;
        }
    }

    [Serializable]
    public class StaffBonuses
    {
        public int nurseCount;
        public int techCount;
        public float comfortSupport;
        public float pressureMultiplier;
        public float toolEfficiencyMultiplier;
        public int extraCasesPerDay;
        public int dailyUpkeep;
    }

    [Serializable]
    public class UpgradeBonuses
    {
        public int scopeLevel;
        public int monitorLevel;
        public int sedationSupportLevel;
        public int emrLevel;
        public float loopReductionMultiplier;
        public float steerBonus;
        public float monitorVisibilityBonus;
        public float monitorWashMultiplier;
        public float detectionBonus;
        public float qualityBonus;
        public float comfortBuffer;
        public float sedationDoseBonus;
        public int extraCasesPerDay;
    }

    [Serializable]
    public class CaseData
    {
        public int seed;
        public int dayNumber;
        public int caseNumber;
        public string title;
        public string flavor;
        public CaseAnatomy anatomy;
        public PrepQuality prep;
        public SedationProfile sedation;
        public LesionProfile lesion;
        public int lesionCount;
        public int basePayout;
    }

    [Serializable]
    public class ProcedureState
    {
        public CaseData caseData;
        public StaffBonuses staffBonuses;
        public UpgradeBonuses upgradeBonuses;
        public ProcedurePhase phase;
        public bool cecumReached;
        public float progress;
        public float loopTension;
        public float patientComfort;
        public float visibility;
        public float nextPushMultiplier;
        public float nextWithdrawalFocusBonus;
        public int sedateChargesRemaining;
        public int detectedPolyps;
        public int remainingPolyps;
        public int supplySpend;
        public float withdrawalQualityAccumulated;
        public int withdrawalChecks;
        public string statusText;

        public float GetAverageWithdrawalQuality()
        {
            if (withdrawalChecks <= 0)
            {
                return 0f;
            }

            return withdrawalQualityAccumulated / withdrawalChecks;
        }
    }

    [Serializable]
    public class ProcedureResult
    {
        public string caseTitle;
        public ProcedureOutcome outcome;
        public bool success;
        public bool cecumReached;
        public int detectedPolyps;
        public float withdrawalQuality;
        public string withdrawalRating;
        public int moneyEarned;
        public int reputationChange;
        public int supplySpend;
        public string summary;
    }

    [Serializable]
    public class ProcedureActionResolution
    {
        public bool blocked;
        public bool enteredWithdrawal;
        public bool caseEnded;
        public int moneySpent;
        public string message;
        public ProcedureResult result;
    }

    [Serializable]
    public class DayRunData
    {
        public int dayNumber;
        public int startingMoney;
        public int upkeepPaid;
        public int scheduledCases;
        public int currentCaseIndex;
        public List<CaseData> cases = new List<CaseData>();
        public List<ProcedureResult> results = new List<ProcedureResult>();

        public int GetRemainingCaseCount()
        {
            return Math.Max(0, scheduledCases - currentCaseIndex);
        }

        public int GetGrossRevenue()
        {
            int total = 0;
            for (int i = 0; i < results.Count; i++)
            {
                total += results[i].moneyEarned;
            }

            return total;
        }

        public int GetSupplySpend()
        {
            int total = 0;
            for (int i = 0; i < results.Count; i++)
            {
                total += results[i].supplySpend;
            }

            return total;
        }

        public int GetReputationDelta()
        {
            int total = 0;
            for (int i = 0; i < results.Count; i++)
            {
                total += results[i].reputationChange;
            }

            return total;
        }

        public float GetAverageWithdrawalQuality()
        {
            if (results.Count <= 0)
            {
                return 0f;
            }

            float total = 0f;
            int counted = 0;
            for (int i = 0; i < results.Count; i++)
            {
                if (results[i].cecumReached)
                {
                    total += results[i].withdrawalQuality;
                    counted += 1;
                }
            }

            if (counted <= 0)
            {
                return 0f;
            }

            return total / counted;
        }
    }
}
