using System.Collections.Generic;
using UnityEngine;

namespace AroundTheCorner
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public SaveData SaveData { get; private set; }
        public DayRunData CurrentDay { get; private set; }
        public ProcedureResult LastProcedureResult { get; private set; }
        public AppScreen CurrentScreen { get; private set; }

        public StaffManager StaffManager { get; private set; }
        public UpgradeManager UpgradeManager { get; private set; }
        public ProcedureManager ProcedureManager { get; private set; }
        public SaveManager SaveManager { get; private set; }

        private EconomyManager economyManager;
        private CaseGenerator caseGenerator;
        private GameUI gameUI;

        public bool HasSaveFile
        {
            get { return SaveManager != null && SaveManager.HasSave(); }
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            Application.targetFrameRate = 60;

            SaveManager = new SaveManager();
            LoadSaveData();

            economyManager = new EconomyManager();
            caseGenerator = new CaseGenerator();
            ProcedureManager = new ProcedureManager();

            GameObject uiRoot = new GameObject("GameUIRoot");
            DontDestroyOnLoad(uiRoot);
            gameUI = uiRoot.AddComponent<GameUI>();
            gameUI.Initialize(this);

            ShowScreen(AppScreen.MainMenu);
        }

        public void LoadSaveData()
        {
            SaveData = SaveManager.Load();
            StaffManager = new StaffManager(SaveData);
            UpgradeManager = new UpgradeManager(SaveData);
        }

        public void BeginContinueFlow()
        {
            if (CurrentDay == null || CurrentDay.GetRemainingCaseCount() <= 0)
            {
                StartNewDay();
                return;
            }

            ShowScreen(AppScreen.Clinic);
        }

        public void ResetSave()
        {
            SaveData = GameBalance.CreateDefaultSave();
            StaffManager = new StaffManager(SaveData);
            UpgradeManager = new UpgradeManager(SaveData);
            CurrentDay = null;
            LastProcedureResult = null;
            SaveManager.Save(SaveData);
            ShowScreen(AppScreen.Clinic);
            StartNewDay();
        }

        public void StartNewDay()
        {
            CurrentDay = new DayRunData();
            CurrentDay.dayNumber = SaveData.currentDayNumber;
            CurrentDay.startingMoney = SaveData.money;
            CurrentDay.upkeepPaid = StaffManager.GetBonuses().dailyUpkeep;

            if (CurrentDay.upkeepPaid > 0)
            {
                SaveData.money = Mathf.Max(0, SaveData.money - CurrentDay.upkeepPaid);
            }

            CurrentDay.scheduledCases = GameBalance.GetDailyCaseCount(SaveData);
            CurrentDay.currentCaseIndex = 0;
            CurrentDay.cases = caseGenerator.GenerateDay(CurrentDay.dayNumber, CurrentDay.scheduledCases, SaveData.reputation);
            CurrentDay.results = new List<ProcedureResult>();
            LastProcedureResult = null;
            Persist();
            ShowScreen(AppScreen.Clinic);
        }

        public void StartNextCase()
        {
            if (CurrentDay == null)
            {
                StartNewDay();
                return;
            }

            if (CurrentDay.currentCaseIndex >= CurrentDay.cases.Count)
            {
                EndDay();
                return;
            }

            CaseData caseData = CurrentDay.cases[CurrentDay.currentCaseIndex];
            ProcedureManager.StartCase(caseData, StaffManager.GetBonuses(), UpgradeManager.GetBonuses());
            LastProcedureResult = null;
            ShowScreen(AppScreen.Procedure);
        }

        public void ApplyProcedureAction(ProcedureAction action)
        {
            ProcedureActionResolution resolution = ProcedureManager.ApplyAction(action, SaveData.money);
            if (resolution.moneySpent > 0)
            {
                SaveData.money = Mathf.Max(0, SaveData.money - resolution.moneySpent);
            }

            if (resolution.caseEnded && resolution.result != null)
            {
                LastProcedureResult = resolution.result;
                economyManager.ApplyProcedureOutcome(SaveData, ProcedureManager.CurrentState.caseData, resolution.result);
                CurrentDay.results.Add(resolution.result);
                CurrentDay.currentCaseIndex += 1;
                Persist();
            }

            gameUI.Refresh();
        }

        public void ContinueAfterCase()
        {
            LastProcedureResult = null;
            if (CurrentDay == null || CurrentDay.currentCaseIndex >= CurrentDay.cases.Count)
            {
                EndDay();
                return;
            }

            ShowScreen(AppScreen.Clinic);
        }

        public void ShowScreen(AppScreen screen)
        {
            CurrentScreen = screen;
            if (gameUI != null)
            {
                gameUI.ShowScreen(screen);
            }
        }

        public void OpenClinic()
        {
            ShowScreen(AppScreen.Clinic);
        }

        public void OpenStaffScreen()
        {
            ShowScreen(AppScreen.Staff);
        }

        public void OpenUpgradeScreen()
        {
            ShowScreen(AppScreen.Upgrades);
        }

        public void OpenMainMenu()
        {
            ShowScreen(AppScreen.MainMenu);
        }

        public bool TryHireStaff(StaffType type)
        {
            if (!StaffManager.CanHire(type))
            {
                return false;
            }

            int cost = StaffManager.GetNextHireCost(type);
            if (SaveData.money < cost)
            {
                return false;
            }

            SaveData.money -= cost;
            StaffManager.Hire(type);
            Persist();
            gameUI.Refresh();
            return true;
        }

        public bool TryBuyUpgrade(UpgradeType type)
        {
            if (!UpgradeManager.CanBuy(type))
            {
                return false;
            }

            int cost = UpgradeManager.GetNextCost(type);
            if (SaveData.money < cost || cost < 0)
            {
                return false;
            }

            SaveData.money -= cost;
            UpgradeManager.Buy(type);
            Persist();
            gameUI.Refresh();
            return true;
        }

        public void EndDay()
        {
            if (CurrentScreen == AppScreen.EndOfDay)
            {
                ShowScreen(AppScreen.EndOfDay);
                return;
            }

            SaveData.daysCompleted += 1;
            SaveData.currentDayNumber += 1;
            Persist();
            ShowScreen(AppScreen.EndOfDay);
        }

        public void Persist()
        {
            SaveManager.Save(SaveData);
        }

        public CaseData GetNextScheduledCase()
        {
            if (CurrentDay == null || CurrentDay.currentCaseIndex >= CurrentDay.cases.Count)
            {
                return null;
            }

            return CurrentDay.cases[CurrentDay.currentCaseIndex];
        }

        public List<CaseData> GetRemainingCases()
        {
            List<CaseData> remaining = new List<CaseData>();
            if (CurrentDay == null)
            {
                return remaining;
            }

            for (int i = CurrentDay.currentCaseIndex; i < CurrentDay.cases.Count; i++)
            {
                remaining.Add(CurrentDay.cases[i]);
            }

            return remaining;
        }
    }
}
