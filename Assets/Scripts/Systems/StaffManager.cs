namespace AroundTheCorner
{
    public class StaffManager
    {
        private readonly SaveData saveData;

        public StaffManager(SaveData saveData)
        {
            this.saveData = saveData;
        }

        public int GetCount(StaffType type)
        {
            switch (type)
            {
                case StaffType.Nurse:
                    return saveData.nurseCount;
                case StaffType.Tech:
                    return saveData.techCount;
                default:
                    return 0;
            }
        }

        public int GetNextHireCost(StaffType type)
        {
            return GameBalance.GetStaffHireCost(type, GetCount(type));
        }

        public bool CanHire(StaffType type)
        {
            return GetCount(type) < GameBalance.MaxStaffPerType;
        }

        public void Hire(StaffType type)
        {
            switch (type)
            {
                case StaffType.Nurse:
                    saveData.nurseCount += 1;
                    break;
                case StaffType.Tech:
                    saveData.techCount += 1;
                    break;
            }
        }

        public StaffBonuses GetBonuses()
        {
            StaffBonuses bonuses = new StaffBonuses();
            bonuses.nurseCount = saveData.nurseCount;
            bonuses.techCount = saveData.techCount;
            bonuses.comfortSupport = saveData.nurseCount * 2.0f;
            bonuses.pressureMultiplier = 1f + (saveData.nurseCount * 0.18f);
            bonuses.toolEfficiencyMultiplier = 1f + (saveData.techCount * 0.18f);
            bonuses.extraCasesPerDay = saveData.techCount;
            bonuses.dailyUpkeep = (saveData.nurseCount * GameBalance.GetStaffUpkeepPerHire(StaffType.Nurse)) +
                                  (saveData.techCount * GameBalance.GetStaffUpkeepPerHire(StaffType.Tech));
            return bonuses;
        }
    }
}
