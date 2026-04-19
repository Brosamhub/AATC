namespace AroundTheCorner
{
    public class UpgradeManager
    {
        private readonly SaveData saveData;

        public UpgradeManager(SaveData saveData)
        {
            this.saveData = saveData;
        }

        public int GetLevel(UpgradeType type)
        {
            switch (type)
            {
                case UpgradeType.Scope:
                    return saveData.scopeUpgradeLevel;
                case UpgradeType.Monitor:
                    return saveData.monitorUpgradeLevel;
                case UpgradeType.SedationSupport:
                    return saveData.sedationSupportUpgradeLevel;
                case UpgradeType.EMR:
                    return saveData.emrUpgradeLevel;
                default:
                    return 0;
            }
        }

        public bool CanBuy(UpgradeType type)
        {
            return GetLevel(type) < GameBalance.MaxUpgradeLevel;
        }

        public int GetNextCost(UpgradeType type)
        {
            return GameBalance.GetUpgradeCost(type, GetLevel(type));
        }

        public void Buy(UpgradeType type)
        {
            switch (type)
            {
                case UpgradeType.Scope:
                    saveData.scopeUpgradeLevel += 1;
                    break;
                case UpgradeType.Monitor:
                    saveData.monitorUpgradeLevel += 1;
                    break;
                case UpgradeType.SedationSupport:
                    saveData.sedationSupportUpgradeLevel += 1;
                    break;
                case UpgradeType.EMR:
                    saveData.emrUpgradeLevel += 1;
                    break;
            }
        }

        public UpgradeBonuses GetBonuses()
        {
            UpgradeBonuses bonuses = new UpgradeBonuses();
            bonuses.scopeLevel = saveData.scopeUpgradeLevel;
            bonuses.monitorLevel = saveData.monitorUpgradeLevel;
            bonuses.sedationSupportLevel = saveData.sedationSupportUpgradeLevel;
            bonuses.emrLevel = saveData.emrUpgradeLevel;
            bonuses.loopReductionMultiplier = 1f - (saveData.scopeUpgradeLevel * 0.06f);
            bonuses.steerBonus = saveData.scopeUpgradeLevel * 0.07f;
            bonuses.monitorVisibilityBonus = saveData.monitorUpgradeLevel * 7f;
            bonuses.monitorWashMultiplier = 1f + (saveData.monitorUpgradeLevel * 0.10f);
            bonuses.detectionBonus = saveData.monitorUpgradeLevel * 0.06f;
            bonuses.qualityBonus = saveData.monitorUpgradeLevel * 6f;
            bonuses.comfortBuffer = saveData.sedationSupportUpgradeLevel * 6f;
            bonuses.sedationDoseBonus = saveData.sedationSupportUpgradeLevel * 4f;
            bonuses.extraCasesPerDay = saveData.emrUpgradeLevel;
            return bonuses;
        }
    }
}
