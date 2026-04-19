using System;

namespace AroundTheCorner
{
    public static class GameBalance
    {
        public sealed class AnatomyDefinition
        {
            public readonly CaseAnatomy type;
            public readonly string label;
            public readonly string clinicNote;
            public readonly float pushMultiplier;
            public readonly float loopMultiplier;
            public readonly float discomfortMultiplier;
            public readonly float pressureSweetSpotMin;
            public readonly float pressureSweetSpotMax;
            public readonly int payoutBonus;

            public AnatomyDefinition(
                CaseAnatomy type,
                string label,
                string clinicNote,
                float pushMultiplier,
                float loopMultiplier,
                float discomfortMultiplier,
                float pressureSweetSpotMin,
                float pressureSweetSpotMax,
                int payoutBonus)
            {
                this.type = type;
                this.label = label;
                this.clinicNote = clinicNote;
                this.pushMultiplier = pushMultiplier;
                this.loopMultiplier = loopMultiplier;
                this.discomfortMultiplier = discomfortMultiplier;
                this.pressureSweetSpotMin = pressureSweetSpotMin;
                this.pressureSweetSpotMax = pressureSweetSpotMax;
                this.payoutBonus = payoutBonus;
            }
        }

        public sealed class PrepDefinition
        {
            public readonly PrepQuality type;
            public readonly string label;
            public readonly string clinicNote;
            public readonly float startingVisibility;
            public readonly float visibilityDecay;
            public readonly float detectionModifier;
            public readonly float qualityModifier;
            public readonly int payoutBonus;

            public PrepDefinition(
                PrepQuality type,
                string label,
                string clinicNote,
                float startingVisibility,
                float visibilityDecay,
                float detectionModifier,
                float qualityModifier,
                int payoutBonus)
            {
                this.type = type;
                this.label = label;
                this.clinicNote = clinicNote;
                this.startingVisibility = startingVisibility;
                this.visibilityDecay = visibilityDecay;
                this.detectionModifier = detectionModifier;
                this.qualityModifier = qualityModifier;
                this.payoutBonus = payoutBonus;
            }
        }

        public sealed class SedationDefinition
        {
            public readonly SedationProfile type;
            public readonly string label;
            public readonly string clinicNote;
            public readonly float startingComfort;
            public readonly int sedateCharges;
            public readonly float doseEffect;
            public readonly int doseCost;
            public readonly float discomfortMultiplier;
            public readonly int payoutBonus;

            public SedationDefinition(
                SedationProfile type,
                string label,
                string clinicNote,
                float startingComfort,
                int sedateCharges,
                float doseEffect,
                int doseCost,
                float discomfortMultiplier,
                int payoutBonus)
            {
                this.type = type;
                this.label = label;
                this.clinicNote = clinicNote;
                this.startingComfort = startingComfort;
                this.sedateCharges = sedateCharges;
                this.doseEffect = doseEffect;
                this.doseCost = doseCost;
                this.discomfortMultiplier = discomfortMultiplier;
                this.payoutBonus = payoutBonus;
            }
        }

        public sealed class LesionDefinition
        {
            public readonly LesionProfile type;
            public readonly string label;
            public readonly string clinicNote;
            public readonly int lesionCountMin;
            public readonly int lesionCountMax;
            public readonly float detectionPenalty;
            public readonly int payoutPerDetection;

            public LesionDefinition(
                LesionProfile type,
                string label,
                string clinicNote,
                int lesionCountMin,
                int lesionCountMax,
                float detectionPenalty,
                int payoutPerDetection)
            {
                this.type = type;
                this.label = label;
                this.clinicNote = clinicNote;
                this.lesionCountMin = lesionCountMin;
                this.lesionCountMax = lesionCountMax;
                this.detectionPenalty = detectionPenalty;
                this.payoutPerDetection = payoutPerDetection;
            }
        }

        public const int StartingMoney = 500;
        public const int StartingReputation = 2;
        public const int StartingDayNumber = 1;
        public const int BaseCasesPerDay = 3;
        public const int MaxStaffPerType = 3;
        public const int MaxUpgradeLevel = 3;

        public const float BasePushGain = 15f;
        public const float BaseLoopGain = 16f;
        public const float BasePushComfortLoss = 10f;
        public const float BasePullLoopReduction = 22f;
        public const float BasePullComfortRecovery = 8f;
        public const float BasePullProgressLoss = 9f;
        public const float BaseSteerBonus = 0.16f;
        public const float BasePressureReduction = 18f;
        public const float BaseWashVisibilityGain = 22f;
        public const float BaseWithdrawalAdvance = 24f;
        public const float BaseWithdrawalFocusBonus = 16f;
        public const float VisibilityLossOnPushMultiplier = 0.35f;
        public const float VisibilityLossOnWithdrawalMultiplier = 0.50f;

        private static readonly AnatomyDefinition[] AnatomyDefinitions =
        {
            new AnatomyDefinition(CaseAnatomy.Easy, "Easy", "Friendly turns, low drama.", 1.08f, 0.86f, 0.90f, 30f, 65f, 0),
            new AnatomyDefinition(CaseAnatomy.RedundantColon, "Redundant Colon", "The scenic route. Bring patience.", 0.90f, 1.25f, 1.00f, 45f, 82f, 20),
            new AnatomyDefinition(CaseAnatomy.FixedSigmoid, "Fixed Sigmoid", "Tight early turns that punish sloppy pushes.", 0.95f, 1.15f, 1.18f, 40f, 70f, 15)
        };

        private static readonly PrepDefinition[] PrepDefinitions =
        {
            new PrepDefinition(PrepQuality.Excellent, "Excellent", "Sparkling views and happier adenoma hunters.", 82f, 1.8f, 0.14f, 12f, 10),
            new PrepDefinition(PrepQuality.Fair, "Fair", "Playable, not glamorous.", 62f, 3.0f, 0.00f, 0f, 0),
            new PrepDefinition(PrepQuality.Poor, "Poor", "The suction canister sees things.", 42f, 4.4f, -0.18f, -12f, -10)
        };

        private static readonly SedationDefinition[] SedationDefinitions =
        {
            new SedationDefinition(SedationProfile.Light, "Light", "Very awake, very opinionated.", 72f, 1, 15f, 10, 1.12f, 0),
            new SedationDefinition(SedationProfile.Moderate, "Moderate", "The dependable middle lane.", 82f, 2, 18f, 16, 1.00f, 8),
            new SedationDefinition(SedationProfile.Propofol, "Propofol", "Smooth ride, pricier invoice.", 92f, 3, 22f, 24, 0.86f, 18)
        };

        private static readonly LesionDefinition[] LesionDefinitions =
        {
            new LesionDefinition(LesionProfile.None, "None", "Nothing suspicious on the radar.", 0, 0, 0.00f, 0),
            new LesionDefinition(LesionProfile.SmallPolyp, "Small Polyp", "One sneaky little paycheck.", 1, 1, 0.08f, 35),
            new LesionDefinition(LesionProfile.MultiplePolyps, "Multiple Polyps", "A productive withdrawal if you keep the view clean.", 2, 3, 0.16f, 30)
        };

        private static readonly int[] ScopeUpgradeCosts = { 125, 250, 425 };
        private static readonly int[] MonitorUpgradeCosts = { 125, 240, 420 };
        private static readonly int[] SedationUpgradeCosts = { 150, 285, 460 };
        private static readonly int[] EmrUpgradeCosts = { 175, 325, 550 };

        private static readonly string[] CasePrefixes =
        {
            "Routine Round",
            "Hallway Hero",
            "Scope Sprint",
            "Turn Signal Special",
            "Quiet Morning",
            "Snack Break Special"
        };

        private static readonly string[] CaseNicknames =
        {
            "The Scenic Route",
            "The Sharp Left",
            "The Bubble Bath",
            "The Hidden Bonus Round",
            "The Nervous Nellie",
            "The Clean Sweep"
        };

        public static SaveData CreateDefaultSave()
        {
            return new SaveData
            {
                money = StartingMoney,
                reputation = StartingReputation,
                currentDayNumber = StartingDayNumber,
                nurseCount = 0,
                techCount = 0,
                scopeUpgradeLevel = 0,
                monitorUpgradeLevel = 0,
                sedationSupportUpgradeLevel = 0,
                emrUpgradeLevel = 0,
                daysCompleted = 0,
                totalCasesStarted = 0,
                completedCases = 0,
                cecalCompletions = 0,
                detectedPolyps = 0,
                totalWithdrawalQuality = 0f,
                complications = 0,
                abortedCases = 0
            };
        }

        public static AnatomyDefinition GetAnatomyDefinition(CaseAnatomy anatomy)
        {
            for (int i = 0; i < AnatomyDefinitions.Length; i++)
            {
                if (AnatomyDefinitions[i].type == anatomy)
                {
                    return AnatomyDefinitions[i];
                }
            }

            return AnatomyDefinitions[0];
        }

        public static PrepDefinition GetPrepDefinition(PrepQuality prep)
        {
            for (int i = 0; i < PrepDefinitions.Length; i++)
            {
                if (PrepDefinitions[i].type == prep)
                {
                    return PrepDefinitions[i];
                }
            }

            return PrepDefinitions[0];
        }

        public static SedationDefinition GetSedationDefinition(SedationProfile sedation)
        {
            for (int i = 0; i < SedationDefinitions.Length; i++)
            {
                if (SedationDefinitions[i].type == sedation)
                {
                    return SedationDefinitions[i];
                }
            }

            return SedationDefinitions[0];
        }

        public static LesionDefinition GetLesionDefinition(LesionProfile lesion)
        {
            for (int i = 0; i < LesionDefinitions.Length; i++)
            {
                if (LesionDefinitions[i].type == lesion)
                {
                    return LesionDefinitions[i];
                }
            }

            return LesionDefinitions[0];
        }

        public static int GetStaffHireCost(StaffType type, int currentCount)
        {
            switch (type)
            {
                case StaffType.Nurse:
                    return 140 + (currentCount * 70);
                case StaffType.Tech:
                    return 160 + (currentCount * 80);
                default:
                    return 9999;
            }
        }

        public static int GetStaffUpkeepPerHire(StaffType type)
        {
            switch (type)
            {
                case StaffType.Nurse:
                    return 18;
                case StaffType.Tech:
                    return 22;
                default:
                    return 0;
            }
        }

        public static int GetUpgradeCost(UpgradeType type, int currentLevel)
        {
            if (currentLevel < 0 || currentLevel >= MaxUpgradeLevel)
            {
                return -1;
            }

            switch (type)
            {
                case UpgradeType.Scope:
                    return ScopeUpgradeCosts[currentLevel];
                case UpgradeType.Monitor:
                    return MonitorUpgradeCosts[currentLevel];
                case UpgradeType.SedationSupport:
                    return SedationUpgradeCosts[currentLevel];
                case UpgradeType.EMR:
                    return EmrUpgradeCosts[currentLevel];
                default:
                    return -1;
            }
        }

        public static int GetDailyCaseCount(SaveData save)
        {
            return BaseCasesPerDay + save.techCount + save.emrUpgradeLevel;
        }

        public static string GetUpgradeTitle(UpgradeType type)
        {
            switch (type)
            {
                case UpgradeType.Scope:
                    return "Scope Upgrade";
                case UpgradeType.Monitor:
                    return "Monitor Upgrade";
                case UpgradeType.SedationSupport:
                    return "Sedation Support";
                case UpgradeType.EMR:
                    return "EMR Upgrade";
                default:
                    return "Upgrade";
            }
        }

        public static string GetUpgradeDescription(UpgradeType type)
        {
            switch (type)
            {
                case UpgradeType.Scope:
                    return "Reduces loop buildup and makes steering convert into cleaner pushes.";
                case UpgradeType.Monitor:
                    return "Boosts visibility, wash strength, and lesion detection on withdrawal.";
                case UpgradeType.SedationSupport:
                    return "Adds comfort buffer and makes rescue sedation more effective.";
                case UpgradeType.EMR:
                    return "Cuts admin drag so you can book one extra case each day per tier.";
                default:
                    return string.Empty;
            }
        }

        public static string GetStaffTitle(StaffType type)
        {
            switch (type)
            {
                case StaffType.Nurse:
                    return "Nurse";
                case StaffType.Tech:
                    return "Tech";
                default:
                    return "Staff";
            }
        }

        public static string GetStaffDescription(StaffType type)
        {
            switch (type)
            {
                case StaffType.Nurse:
                    return "Comfort support plus smarter abdominal pressure during tough loops.";
                case StaffType.Tech:
                    return "Faster turnover and stronger wash/suction efficiency.";
                default:
                    return string.Empty;
            }
        }

        public static string GetCaseTitle(Random random)
        {
            string prefix = CasePrefixes[random.Next(CasePrefixes.Length)];
            string nickname = CaseNicknames[random.Next(CaseNicknames.Length)];
            return prefix + ": " + nickname;
        }
    }
}
