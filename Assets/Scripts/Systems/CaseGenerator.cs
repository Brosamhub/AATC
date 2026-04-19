using System;
using System.Collections.Generic;

namespace AroundTheCorner
{
    public class CaseGenerator
    {
        public List<CaseData> GenerateDay(int dayNumber, int caseCount, int reputation)
        {
            List<CaseData> cases = new List<CaseData>();
            Random random = new Random((dayNumber * 7919) + (reputation * 131));

            for (int i = 0; i < caseCount; i++)
            {
                CaseAnatomy anatomy = RollAnatomy(random, dayNumber);
                PrepQuality prep = RollPrep(random, dayNumber);
                SedationProfile sedation = RollSedation(random);
                LesionProfile lesion = RollLesion(random, dayNumber);
                GameBalance.LesionDefinition lesionDefinition = GameBalance.GetLesionDefinition(lesion);

                int lesionCount = 0;
                if (lesionDefinition.lesionCountMax > 0)
                {
                    lesionCount = random.Next(lesionDefinition.lesionCountMin, lesionDefinition.lesionCountMax + 1);
                }

                CaseData caseData = new CaseData();
                caseData.seed = random.Next(10000, 99999);
                caseData.dayNumber = dayNumber;
                caseData.caseNumber = i + 1;
                caseData.title = GameBalance.GetCaseTitle(random);
                caseData.flavor = BuildFlavorText(anatomy, prep, sedation, lesion);
                caseData.anatomy = anatomy;
                caseData.prep = prep;
                caseData.sedation = sedation;
                caseData.lesion = lesion;
                caseData.lesionCount = lesionCount;
                caseData.basePayout = 90
                                      + GameBalance.GetAnatomyDefinition(anatomy).payoutBonus
                                      + GameBalance.GetPrepDefinition(prep).payoutBonus
                                      + GameBalance.GetSedationDefinition(sedation).payoutBonus;

                cases.Add(caseData);
            }

            return cases;
        }

        private static CaseAnatomy RollAnatomy(Random random, int dayNumber)
        {
            int roll = random.Next(100);
            if (dayNumber < 3)
            {
                if (roll < 55) return CaseAnatomy.Easy;
                if (roll < 82) return CaseAnatomy.FixedSigmoid;
                return CaseAnatomy.RedundantColon;
            }

            if (roll < 40) return CaseAnatomy.Easy;
            if (roll < 72) return CaseAnatomy.FixedSigmoid;
            return CaseAnatomy.RedundantColon;
        }

        private static PrepQuality RollPrep(Random random, int dayNumber)
        {
            int roll = random.Next(100);
            if (dayNumber < 3)
            {
                if (roll < 45) return PrepQuality.Excellent;
                if (roll < 82) return PrepQuality.Fair;
                return PrepQuality.Poor;
            }

            if (roll < 34) return PrepQuality.Excellent;
            if (roll < 76) return PrepQuality.Fair;
            return PrepQuality.Poor;
        }

        private static SedationProfile RollSedation(Random random)
        {
            int roll = random.Next(100);
            if (roll < 30) return SedationProfile.Light;
            if (roll < 80) return SedationProfile.Moderate;
            return SedationProfile.Propofol;
        }

        private static LesionProfile RollLesion(Random random, int dayNumber)
        {
            int roll = random.Next(100);
            if (dayNumber < 2)
            {
                if (roll < 42) return LesionProfile.None;
                if (roll < 78) return LesionProfile.SmallPolyp;
                return LesionProfile.MultiplePolyps;
            }

            if (roll < 32) return LesionProfile.None;
            if (roll < 70) return LesionProfile.SmallPolyp;
            return LesionProfile.MultiplePolyps;
        }

        private static string BuildFlavorText(CaseAnatomy anatomy, PrepQuality prep, SedationProfile sedation, LesionProfile lesion)
        {
            return GameBalance.GetAnatomyDefinition(anatomy).clinicNote + " " +
                   GameBalance.GetPrepDefinition(prep).clinicNote + " " +
                   GameBalance.GetSedationDefinition(sedation).clinicNote + " " +
                   GameBalance.GetLesionDefinition(lesion).clinicNote;
        }
    }
}
