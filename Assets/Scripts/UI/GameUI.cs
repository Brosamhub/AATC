using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AroundTheCorner
{
    public class GameUI : MonoBehaviour
    {
        private sealed class MetricWidget
        {
            public Text valueText;
            public Text labelText;
        }

        private sealed class MeterWidget
        {
            public Text titleText;
            public Text valueText;
            public Image fillImage;
        }

        private sealed class MainMenuView
        {
            public RectTransform root;
            public Text savePathText;
            public Button continueButton;
            public Button newSaveButton;
        }

        private sealed class ClinicView
        {
            public RectTransform root;
            public Text titleText;
            public Text dayInfoText;
            public MetricWidget moneyMetric;
            public MetricWidget reputationMetric;
            public MetricWidget casesMetric;
            public MetricWidget cecalMetric;
            public MetricWidget polypsMetric;
            public MetricWidget withdrawalMetric;
            public Text scheduleSummaryText;
            public Text nextCaseTitleText;
            public Text nextCaseTagsText;
            public Text nextCaseFlavorText;
            public Text lineupText;
            public Button startCaseButton;
            public Button staffButton;
            public Button upgradesButton;
        }

        private sealed class ProcedureView
        {
            public RectTransform root;
            public Text headerTitleText;
            public Text headerTagsText;
            public Text headerStatsText;
            public MeterWidget progressMeter;
            public MeterWidget loopMeter;
            public MeterWidget comfortMeter;
            public MeterWidget visibilityMeter;
            public Text helperText;
            public Text statusLogText;
            public Text chargesText;
            public Button pushButton;
            public Button pullBackButton;
            public Button steerButton;
            public Button pressureButton;
            public Button washButton;
            public Button sedateButton;
            public RectTransform resultOverlay;
            public Text resultTitleText;
            public Text resultBodyText;
            public Button resultContinueButton;
        }

        private sealed class StaffView
        {
            public RectTransform root;
            public Text headerText;
            public Text payrollText;
            public Text nurseBodyText;
            public Text techBodyText;
            public Button nurseHireButton;
            public Button techHireButton;
            public Text nurseButtonLabel;
            public Text techButtonLabel;
            public Button backButton;
        }

        private sealed class UpgradesView
        {
            public RectTransform root;
            public Text headerText;
            public Text scopeBodyText;
            public Text monitorBodyText;
            public Text sedationBodyText;
            public Text emrBodyText;
            public Button scopeButton;
            public Button monitorButton;
            public Button sedationButton;
            public Button emrButton;
            public Text scopeButtonLabel;
            public Text monitorButtonLabel;
            public Text sedationButtonLabel;
            public Text emrButtonLabel;
            public Button backButton;
        }

        private sealed class EndDayView
        {
            public RectTransform root;
            public Text headerText;
            public Text summaryText;
            public Text caseResultsText;
            public Button nextDayButton;
            public Button mainMenuButton;
        }

        private GameManager gameManager;
        private Font font;
        private Canvas canvas;
        private RectTransform screenRoot;

        private MainMenuView mainMenuView = new MainMenuView();
        private ClinicView clinicView = new ClinicView();
        private ProcedureView procedureView = new ProcedureView();
        private StaffView staffView = new StaffView();
        private UpgradesView upgradesView = new UpgradesView();
        private EndDayView endDayView = new EndDayView();

        private readonly List<GameObject> screenObjects = new List<GameObject>();

        private readonly Color backgroundColor = new Color(0.93f, 0.95f, 0.90f, 1f);
        private readonly Color cardColor = new Color(0.99f, 0.98f, 0.95f, 0.96f);
        private readonly Color cardAccent = new Color(0.13f, 0.35f, 0.34f, 1f);
        private readonly Color secondaryAccent = new Color(0.93f, 0.48f, 0.28f, 1f);
        private readonly Color softAccent = new Color(0.98f, 0.84f, 0.51f, 1f);
        private readonly Color textDark = new Color(0.15f, 0.12f, 0.10f, 1f);
        private readonly Color mutedText = new Color(0.29f, 0.27f, 0.24f, 1f);
        private readonly Color successColor = new Color(0.27f, 0.67f, 0.38f, 1f);
        private readonly Color dangerColor = new Color(0.80f, 0.30f, 0.26f, 1f);

        public void Initialize(GameManager manager)
        {
            gameManager = manager;
            font = LoadUIFont();
            BuildCanvas();
            BuildScreens();
            Refresh();
        }

        public void ShowScreen(AppScreen screen)
        {
            SetAllScreensInactive();
            switch (screen)
            {
                case AppScreen.MainMenu:
                    mainMenuView.root.gameObject.SetActive(true);
                    break;
                case AppScreen.Clinic:
                    clinicView.root.gameObject.SetActive(true);
                    break;
                case AppScreen.Procedure:
                    procedureView.root.gameObject.SetActive(true);
                    break;
                case AppScreen.Staff:
                    staffView.root.gameObject.SetActive(true);
                    break;
                case AppScreen.Upgrades:
                    upgradesView.root.gameObject.SetActive(true);
                    break;
                case AppScreen.EndOfDay:
                    endDayView.root.gameObject.SetActive(true);
                    break;
            }

            Refresh();
        }

        public void Refresh()
        {
            RefreshMainMenu();
            RefreshClinic();
            RefreshProcedure();
            RefreshStaff();
            RefreshUpgrades();
            RefreshEndDay();
        }

        private void BuildCanvas()
        {
            GameObject canvasObject = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasObject.transform.SetParent(transform, false);
            canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.65f;

            if (FindFirstObjectByType<EventSystem>() == null)
            {
                GameObject eventSystemObject = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
                eventSystemObject.transform.SetParent(transform, false);
            }

            Image background = UIFactory.CreatePanel("Background", canvasObject.transform, backgroundColor);
            UIFactory.Stretch(background.rectTransform, 0f, 0f, 0f, 0f);

            screenRoot = UIFactory.CreateUIObject("ScreenRoot", canvasObject.transform);
            UIFactory.Stretch(screenRoot, 24f, 24f, 24f, 24f);
        }

        private void BuildScreens()
        {
            BuildMainMenu();
            BuildClinicScreen();
            BuildProcedureScreen();
            BuildStaffScreen();
            BuildUpgradesScreen();
            BuildEndDayScreen();
            SetAllScreensInactive();
        }

        private void BuildMainMenu()
        {
            Image screen = CreateScreenBase("MainMenu");
            mainMenuView.root = screen.rectTransform;
            UIFactory.AddVerticalLayout(screen.gameObject, 18, new RectOffset(120, 120, 100, 100), TextAnchor.MiddleCenter, false, false);

            Text title = UIFactory.CreateText("Title", screen.transform, font, "...and Around the Corner", 54, TextAnchor.MiddleCenter, textDark, FontStyle.Bold);
            UIFactory.AddLayoutElement(title.gameObject, 90, 90, 0, 0, -1, 1);

            Text subtitle = UIFactory.CreateText("Subtitle", screen.transform, font, "A satirical GI clinic vertical slice built for big buttons, quick cases, and suspiciously profitable withdrawal quality.", 26, TextAnchor.MiddleCenter, mutedText, FontStyle.Normal);
            UIFactory.AddLayoutElement(subtitle.gameObject, 90, 90, 0, 0, -1, 1);

            Image menuCard = UIFactory.CreatePanel("MenuCard", screen.transform, cardColor);
            UIFactory.AddLayoutElement(menuCard.gameObject, 360, 360, 0, 900, 1100, 0);
            UIFactory.AddVerticalLayout(menuCard.gameObject, 20, new RectOffset(40, 40, 36, 36), TextAnchor.UpperCenter, false, false);

            Text description = UIFactory.CreateText("Description", menuCard.transform, font, "Run a compact cartoon colonoscopy clinic: clear cases, protect comfort, hire helpers, and buy upgrades that let tomorrow run just a little smoother.", 24, TextAnchor.UpperLeft, textDark, FontStyle.Normal);
            UIFactory.AddLayoutElement(description.gameObject, 120, 140, 0, 0, -1, 1);

            mainMenuView.continueButton = UIFactory.CreateButton("ContinueButton", menuCard.transform, font, "Continue / Start Day", 28, cardAccent, Color.white);
            UIFactory.AddLayoutElement(mainMenuView.continueButton.gameObject, 78, 78, 0, 0, -1, 1);
            mainMenuView.continueButton.onClick.AddListener(gameManager.BeginContinueFlow);

            mainMenuView.newSaveButton = UIFactory.CreateButton("NewSaveButton", menuCard.transform, font, "New Save", 28, secondaryAccent, Color.white);
            UIFactory.AddLayoutElement(mainMenuView.newSaveButton.gameObject, 78, 78, 0, 0, -1, 1);
            mainMenuView.newSaveButton.onClick.AddListener(gameManager.ResetSave);

            mainMenuView.savePathText = UIFactory.CreateText("SavePathText", menuCard.transform, font, string.Empty, 18, TextAnchor.MiddleCenter, mutedText, FontStyle.Italic);
            UIFactory.AddLayoutElement(mainMenuView.savePathText.gameObject, 36, 50, 0, 0, -1, 1);
        }

        private void BuildClinicScreen()
        {
            Image screen = CreateScreenBase("Clinic");
            clinicView.root = screen.rectTransform;
            UIFactory.AddVerticalLayout(screen.gameObject, 16, new RectOffset(36, 36, 24, 24), TextAnchor.UpperLeft, true, false);

            Image header = CreateHeaderCard(screen.transform, out clinicView.titleText, out clinicView.dayInfoText);
            UIFactory.AddLayoutElement(header.gameObject, 110, 110, 0, 0, -1, 1);

            RectTransform actionRow = UIFactory.CreateUIObject("ActionRow", screen.transform);
            UIFactory.AddLayoutElement(actionRow.gameObject, 92, 92, 0, 0, -1, 1);
            UIFactory.AddHorizontalLayout(actionRow.gameObject, 16, new RectOffset(0, 0, 0, 0), TextAnchor.MiddleLeft, true, true);

            clinicView.staffButton = UIFactory.CreateButton("StaffButton", actionRow, font, "Staff", 24, softAccent, textDark);
            clinicView.upgradesButton = UIFactory.CreateButton("UpgradesButton", actionRow, font, "Upgrades", 24, secondaryAccent, Color.white);
            clinicView.startCaseButton = UIFactory.CreateButton("StartCaseButton", actionRow, font, "Play Next Case", 24, cardAccent, Color.white);
            clinicView.staffButton.onClick.AddListener(gameManager.OpenStaffScreen);
            clinicView.upgradesButton.onClick.AddListener(gameManager.OpenUpgradeScreen);
            clinicView.startCaseButton.onClick.AddListener(gameManager.StartNextCase);

            RectTransform metricsGrid = UIFactory.CreateUIObject("MetricsGrid", screen.transform);
            UIFactory.AddLayoutElement(metricsGrid.gameObject, 180, 180, 0, 0, -1, 1);
            GridLayoutGroup grid = UIFactory.AddGridLayout(metricsGrid.gameObject, new Vector2(0f, 0f), new Vector2(16f, 16f), new RectOffset(0, 0, 0, 0));
            grid.constraintCount = 3;
            grid.startAxis = GridLayoutGroup.Axis.Horizontal;
            grid.cellSize = new Vector2(560f, 82f);
            ContentSizeFitter fitter = metricsGrid.gameObject.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            clinicView.moneyMetric = CreateMetricCard(metricsGrid, "Money");
            clinicView.reputationMetric = CreateMetricCard(metricsGrid, "Reputation");
            clinicView.casesMetric = CreateMetricCard(metricsGrid, "Completed Cases");
            clinicView.cecalMetric = CreateMetricCard(metricsGrid, "Cecal Rate");
            clinicView.polypsMetric = CreateMetricCard(metricsGrid, "Detected Polyps");
            clinicView.withdrawalMetric = CreateMetricCard(metricsGrid, "Avg Withdrawal");

            RectTransform lowerRow = UIFactory.CreateUIObject("LowerRow", screen.transform);
            UIFactory.AddLayoutElement(lowerRow.gameObject, 0, 540, 1, 0, -1, 1);
            UIFactory.AddHorizontalLayout(lowerRow.gameObject, 16, new RectOffset(0, 0, 0, 0), TextAnchor.UpperLeft, true, true);

            Image scheduleCard = UIFactory.CreatePanel("ScheduleCard", lowerRow, cardColor);
            UIFactory.AddLayoutElement(scheduleCard.gameObject, 0, -1, 1, 0, 640, 1);
            UIFactory.AddVerticalLayout(scheduleCard.gameObject, 14, new RectOffset(24, 24, 24, 24), TextAnchor.UpperLeft, true, false);
            Text scheduleTitle = UIFactory.CreateText("ScheduleTitle", scheduleCard.transform, font, "Today in Clinic", 30, TextAnchor.MiddleLeft, textDark, FontStyle.Bold);
            UIFactory.AddLayoutElement(scheduleTitle.gameObject, 44, 44, 0, 0, -1, 1);
            clinicView.scheduleSummaryText = UIFactory.CreateText("ScheduleSummary", scheduleCard.transform, font, string.Empty, 22, TextAnchor.UpperLeft, mutedText, FontStyle.Normal);
            UIFactory.AddLayoutElement(clinicView.scheduleSummaryText.gameObject, 150, 170, 0, 0, -1, 1);
            clinicView.lineupText = UIFactory.CreateText("LineupText", scheduleCard.transform, font, string.Empty, 21, TextAnchor.UpperLeft, textDark, FontStyle.Normal);
            UIFactory.AddLayoutElement(clinicView.lineupText.gameObject, 0, -1, 1, 0, -1, 1);

            Image nextCaseCard = UIFactory.CreatePanel("NextCaseCard", lowerRow, cardColor);
            UIFactory.AddLayoutElement(nextCaseCard.gameObject, 0, -1, 1, 0, 760, 1);
            UIFactory.AddVerticalLayout(nextCaseCard.gameObject, 14, new RectOffset(24, 24, 24, 24), TextAnchor.UpperLeft, true, false);
            Text nextCaseLabel = UIFactory.CreateText("NextCaseLabel", nextCaseCard.transform, font, "Next Case Preview", 30, TextAnchor.MiddleLeft, textDark, FontStyle.Bold);
            UIFactory.AddLayoutElement(nextCaseLabel.gameObject, 44, 44, 0, 0, -1, 1);
            clinicView.nextCaseTitleText = UIFactory.CreateText("NextCaseTitle", nextCaseCard.transform, font, string.Empty, 28, TextAnchor.UpperLeft, cardAccent, FontStyle.Bold);
            UIFactory.AddLayoutElement(clinicView.nextCaseTitleText.gameObject, 46, 56, 0, 0, -1, 1);
            clinicView.nextCaseTagsText = UIFactory.CreateText("NextCaseTags", nextCaseCard.transform, font, string.Empty, 24, TextAnchor.UpperLeft, textDark, FontStyle.Bold);
            UIFactory.AddLayoutElement(clinicView.nextCaseTagsText.gameObject, 110, 130, 0, 0, -1, 1);
            clinicView.nextCaseFlavorText = UIFactory.CreateText("NextCaseFlavor", nextCaseCard.transform, font, string.Empty, 21, TextAnchor.UpperLeft, mutedText, FontStyle.Normal);
            UIFactory.AddLayoutElement(clinicView.nextCaseFlavorText.gameObject, 0, -1, 1, 0, -1, 1);
        }

        private void BuildProcedureScreen()
        {
            Image screen = CreateScreenBase("Procedure");
            procedureView.root = screen.rectTransform;
            UIFactory.AddVerticalLayout(screen.gameObject, 16, new RectOffset(36, 36, 24, 24), TextAnchor.UpperLeft, true, false);

            Image header = CreateHeaderCard(screen.transform, out procedureView.headerTitleText, out procedureView.headerStatsText);
            UIFactory.AddLayoutElement(header.gameObject, 120, 120, 0, 0, -1, 1);

            procedureView.headerTagsText = UIFactory.CreateText("HeaderTags", header.transform, font, string.Empty, 22, TextAnchor.LowerLeft, mutedText, FontStyle.Bold);
            UIFactory.SetAnchors(procedureView.headerTagsText.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(24f, 20f), new Vector2(-24f, -18f));

            RectTransform body = UIFactory.CreateUIObject("Body", screen.transform);
            UIFactory.AddLayoutElement(body.gameObject, 0, 720, 1, 0, -1, 1);
            UIFactory.AddHorizontalLayout(body.gameObject, 16, new RectOffset(0, 0, 0, 0), TextAnchor.UpperLeft, true, true);

            Image leftColumn = UIFactory.CreatePanel("LeftColumn", body, cardColor);
            UIFactory.AddLayoutElement(leftColumn.gameObject, 0, -1, 1, 0, 850, 1);
            UIFactory.AddVerticalLayout(leftColumn.gameObject, 16, new RectOffset(24, 24, 24, 24), TextAnchor.UpperLeft, true, false);

            procedureView.progressMeter = CreateMeterCard(leftColumn.transform, "Progress");
            procedureView.loopMeter = CreateMeterCard(leftColumn.transform, "Loop Tension");
            procedureView.comfortMeter = CreateMeterCard(leftColumn.transform, "Patient Comfort");
            procedureView.visibilityMeter = CreateMeterCard(leftColumn.transform, "Visibility");

            procedureView.helperText = UIFactory.CreateText("HelperText", leftColumn.transform, font, string.Empty, 20, TextAnchor.UpperLeft, mutedText, FontStyle.Italic);
            UIFactory.AddLayoutElement(procedureView.helperText.gameObject, 120, 140, 0, 0, -1, 1);

            procedureView.statusLogText = UIFactory.CreateText("StatusLogText", leftColumn.transform, font, string.Empty, 22, TextAnchor.UpperLeft, textDark, FontStyle.Normal);
            UIFactory.AddLayoutElement(procedureView.statusLogText.gameObject, 0, -1, 1, 0, -1, 1);

            Image rightColumn = UIFactory.CreatePanel("RightColumn", body, cardColor);
            UIFactory.AddLayoutElement(rightColumn.gameObject, 0, -1, 1, 0, 920, 1);
            UIFactory.AddVerticalLayout(rightColumn.gameObject, 18, new RectOffset(24, 24, 24, 24), TextAnchor.UpperLeft, true, false);

            procedureView.chargesText = UIFactory.CreateText("ChargesText", rightColumn.transform, font, string.Empty, 24, TextAnchor.UpperLeft, textDark, FontStyle.Bold);
            UIFactory.AddLayoutElement(procedureView.chargesText.gameObject, 44, 54, 0, 0, -1, 1);

            RectTransform buttonGrid = UIFactory.CreateUIObject("ActionGrid", rightColumn.transform);
            UIFactory.AddLayoutElement(buttonGrid.gameObject, 0, 420, 0, 0, -1, 1);
            GridLayoutGroup actionGrid = UIFactory.AddGridLayout(buttonGrid.gameObject, new Vector2(0f, 0f), new Vector2(16f, 16f), new RectOffset(0, 0, 0, 0));
            actionGrid.constraintCount = 2;
            actionGrid.cellSize = new Vector2(410f, 120f);

            procedureView.pushButton = UIFactory.CreateButton("PushButton", buttonGrid, font, "Push", 28, cardAccent, Color.white);
            procedureView.pullBackButton = UIFactory.CreateButton("PullBackButton", buttonGrid, font, "Pull Back", 28, secondaryAccent, Color.white);
            procedureView.steerButton = UIFactory.CreateButton("SteerButton", buttonGrid, font, "Steer", 28, softAccent, textDark);
            procedureView.pressureButton = UIFactory.CreateButton("PressureButton", buttonGrid, font, "Apply Pressure", 28, new Color(0.67f, 0.75f, 0.36f, 1f), textDark);
            procedureView.washButton = UIFactory.CreateButton("WashButton", buttonGrid, font, "Wash / Suction", 28, new Color(0.44f, 0.67f, 0.90f, 1f), Color.white);
            procedureView.sedateButton = UIFactory.CreateButton("SedateButton", buttonGrid, font, "Sedate", 28, dangerColor, Color.white);

            procedureView.pushButton.onClick.AddListener(delegate { gameManager.ApplyProcedureAction(ProcedureAction.Push); });
            procedureView.pullBackButton.onClick.AddListener(delegate { gameManager.ApplyProcedureAction(ProcedureAction.PullBack); });
            procedureView.steerButton.onClick.AddListener(delegate { gameManager.ApplyProcedureAction(ProcedureAction.Steer); });
            procedureView.pressureButton.onClick.AddListener(delegate { gameManager.ApplyProcedureAction(ProcedureAction.ApplyPressure); });
            procedureView.washButton.onClick.AddListener(delegate { gameManager.ApplyProcedureAction(ProcedureAction.WashSuction); });
            procedureView.sedateButton.onClick.AddListener(delegate { gameManager.ApplyProcedureAction(ProcedureAction.Sedate); });

            Text legend = UIFactory.CreateText("Legend", rightColumn.transform, font, "Push advances but builds loop and discomfort. Pull Back cools things off. Steer buffs the next move. Pressure only shines when loop is already brewing. Wash keeps withdrawal quality alive.", 20, TextAnchor.UpperLeft, mutedText, FontStyle.Normal);
            UIFactory.AddLayoutElement(legend.gameObject, 160, 180, 0, 0, -1, 1);

            procedureView.resultOverlay = UIFactory.CreateUIObject("ResultOverlay", screen.transform);
            procedureView.resultOverlay.gameObject.SetActive(false);
            UIFactory.Stretch(procedureView.resultOverlay, 260f, 160f, 260f, 160f);
            Image overlayPanel = procedureView.resultOverlay.gameObject.AddComponent<Image>();
            overlayPanel.color = new Color(0.99f, 0.98f, 0.95f, 0.98f);
            UIFactory.AddVerticalLayout(procedureView.resultOverlay.gameObject, 16, new RectOffset(36, 36, 36, 36), TextAnchor.MiddleCenter, true, false);
            procedureView.resultTitleText = UIFactory.CreateText("ResultTitle", procedureView.resultOverlay, font, string.Empty, 38, TextAnchor.MiddleCenter, textDark, FontStyle.Bold);
            UIFactory.AddLayoutElement(procedureView.resultTitleText.gameObject, 60, 60, 0, 0, -1, 1);
            procedureView.resultBodyText = UIFactory.CreateText("ResultBody", procedureView.resultOverlay, font, string.Empty, 24, TextAnchor.UpperLeft, mutedText, FontStyle.Normal);
            UIFactory.AddLayoutElement(procedureView.resultBodyText.gameObject, 180, 220, 0, 0, -1, 1);
            procedureView.resultContinueButton = UIFactory.CreateButton("ResultContinueButton", procedureView.resultOverlay, font, "Continue", 28, cardAccent, Color.white);
            UIFactory.AddLayoutElement(procedureView.resultContinueButton.gameObject, 80, 80, 0, 0, -1, 1);
            procedureView.resultContinueButton.onClick.AddListener(gameManager.ContinueAfterCase);
        }

        private void BuildStaffScreen()
        {
            Image screen = CreateScreenBase("Staff");
            staffView.root = screen.rectTransform;
            UIFactory.AddVerticalLayout(screen.gameObject, 16, new RectOffset(36, 36, 24, 24), TextAnchor.UpperLeft, true, false);

            Image header = CreateHeaderCard(screen.transform, out staffView.headerText, out staffView.payrollText);
            UIFactory.AddLayoutElement(header.gameObject, 110, 110, 0, 0, -1, 1);

            RectTransform cards = UIFactory.CreateUIObject("StaffCards", screen.transform);
            UIFactory.AddLayoutElement(cards.gameObject, 0, 620, 1, 0, -1, 1);
            UIFactory.AddHorizontalLayout(cards.gameObject, 16, new RectOffset(0, 0, 0, 0), TextAnchor.UpperLeft, true, true);

            Image nurseCard = CreateStaffCard(cards, "Nurse", out staffView.nurseBodyText, out staffView.nurseHireButton, out staffView.nurseButtonLabel);
            Image techCard = CreateStaffCard(cards, "Tech", out staffView.techBodyText, out staffView.techHireButton, out staffView.techButtonLabel);

            staffView.nurseHireButton.onClick.AddListener(delegate { gameManager.TryHireStaff(StaffType.Nurse); });
            staffView.techHireButton.onClick.AddListener(delegate { gameManager.TryHireStaff(StaffType.Tech); });

            staffView.backButton = UIFactory.CreateButton("BackButton", screen.transform, font, "Back to Clinic", 26, cardAccent, Color.white);
            UIFactory.AddLayoutElement(staffView.backButton.gameObject, 78, 78, 0, 420, 520, 0);
            staffView.backButton.onClick.AddListener(gameManager.OpenClinic);

            UIFactory.AddLayoutElement(nurseCard.gameObject, 0, -1, 1, 0, 0, 1);
            UIFactory.AddLayoutElement(techCard.gameObject, 0, -1, 1, 0, 0, 1);
        }

        private void BuildUpgradesScreen()
        {
            Image screen = CreateScreenBase("Upgrades");
            upgradesView.root = screen.rectTransform;
            UIFactory.AddVerticalLayout(screen.gameObject, 16, new RectOffset(36, 36, 24, 24), TextAnchor.UpperLeft, true, false);

            Text ignoredSubtitle;
            Image header = CreateHeaderCard(screen.transform, out upgradesView.headerText, out ignoredSubtitle);
            UIFactory.AddLayoutElement(header.gameObject, 110, 110, 0, 0, -1, 1);

            RectTransform gridRoot = UIFactory.CreateUIObject("UpgradeGrid", screen.transform);
            UIFactory.AddLayoutElement(gridRoot.gameObject, 0, 680, 1, 0, -1, 1);
            GridLayoutGroup grid = UIFactory.AddGridLayout(gridRoot.gameObject, new Vector2(0f, 0f), new Vector2(16f, 16f), new RectOffset(0, 0, 0, 0));
            grid.constraintCount = 2;
            grid.cellSize = new Vector2(870f, 290f);

            CreateUpgradeCard(gridRoot, UpgradeType.Scope, out upgradesView.scopeBodyText, out upgradesView.scopeButton, out upgradesView.scopeButtonLabel);
            CreateUpgradeCard(gridRoot, UpgradeType.Monitor, out upgradesView.monitorBodyText, out upgradesView.monitorButton, out upgradesView.monitorButtonLabel);
            CreateUpgradeCard(gridRoot, UpgradeType.SedationSupport, out upgradesView.sedationBodyText, out upgradesView.sedationButton, out upgradesView.sedationButtonLabel);
            CreateUpgradeCard(gridRoot, UpgradeType.EMR, out upgradesView.emrBodyText, out upgradesView.emrButton, out upgradesView.emrButtonLabel);

            upgradesView.scopeButton.onClick.AddListener(delegate { gameManager.TryBuyUpgrade(UpgradeType.Scope); });
            upgradesView.monitorButton.onClick.AddListener(delegate { gameManager.TryBuyUpgrade(UpgradeType.Monitor); });
            upgradesView.sedationButton.onClick.AddListener(delegate { gameManager.TryBuyUpgrade(UpgradeType.SedationSupport); });
            upgradesView.emrButton.onClick.AddListener(delegate { gameManager.TryBuyUpgrade(UpgradeType.EMR); });

            upgradesView.backButton = UIFactory.CreateButton("BackButton", screen.transform, font, "Back to Clinic", 26, cardAccent, Color.white);
            UIFactory.AddLayoutElement(upgradesView.backButton.gameObject, 78, 78, 0, 420, 520, 0);
            upgradesView.backButton.onClick.AddListener(gameManager.OpenClinic);
        }

        private void BuildEndDayScreen()
        {
            Image screen = CreateScreenBase("EndOfDay");
            endDayView.root = screen.rectTransform;
            UIFactory.AddVerticalLayout(screen.gameObject, 16, new RectOffset(120, 120, 40, 40), TextAnchor.UpperLeft, true, false);

            endDayView.headerText = UIFactory.CreateText("Header", screen.transform, font, "End of Day", 48, TextAnchor.MiddleCenter, textDark, FontStyle.Bold);
            UIFactory.AddLayoutElement(endDayView.headerText.gameObject, 64, 64, 0, 0, -1, 1);

            Image summaryCard = UIFactory.CreatePanel("SummaryCard", screen.transform, cardColor);
            UIFactory.AddLayoutElement(summaryCard.gameObject, 230, 250, 0, 0, -1, 1);
            UIFactory.AddVerticalLayout(summaryCard.gameObject, 12, new RectOffset(32, 32, 24, 24), TextAnchor.UpperLeft, true, false);
            endDayView.summaryText = UIFactory.CreateText("SummaryText", summaryCard.transform, font, string.Empty, 24, TextAnchor.UpperLeft, textDark, FontStyle.Normal);
            UIFactory.AddLayoutElement(endDayView.summaryText.gameObject, 0, -1, 1, 0, -1, 1);

            Image caseCard = UIFactory.CreatePanel("CaseCard", screen.transform, cardColor);
            UIFactory.AddLayoutElement(caseCard.gameObject, 0, 380, 1, 0, -1, 1);
            UIFactory.AddVerticalLayout(caseCard.gameObject, 12, new RectOffset(32, 32, 24, 24), TextAnchor.UpperLeft, true, false);
            Text caseTitle = UIFactory.CreateText("CaseTitle", caseCard.transform, font, "Today's Results", 30, TextAnchor.UpperLeft, cardAccent, FontStyle.Bold);
            UIFactory.AddLayoutElement(caseTitle.gameObject, 44, 44, 0, 0, -1, 1);
            endDayView.caseResultsText = UIFactory.CreateText("CaseResultsText", caseCard.transform, font, string.Empty, 21, TextAnchor.UpperLeft, textDark, FontStyle.Normal);
            UIFactory.AddLayoutElement(endDayView.caseResultsText.gameObject, 0, -1, 1, 0, -1, 1);

            RectTransform footer = UIFactory.CreateUIObject("Footer", screen.transform);
            UIFactory.AddLayoutElement(footer.gameObject, 90, 90, 0, 0, -1, 1);
            UIFactory.AddHorizontalLayout(footer.gameObject, 16, new RectOffset(0, 0, 0, 0), TextAnchor.MiddleCenter, true, true);
            endDayView.nextDayButton = UIFactory.CreateButton("NextDayButton", footer, font, "Start Next Day", 28, cardAccent, Color.white);
            endDayView.mainMenuButton = UIFactory.CreateButton("MainMenuButton", footer, font, "Main Menu", 28, secondaryAccent, Color.white);
            endDayView.nextDayButton.onClick.AddListener(gameManager.StartNewDay);
            endDayView.mainMenuButton.onClick.AddListener(gameManager.OpenMainMenu);
        }

        private Image CreateScreenBase(string name)
        {
            Image screen = UIFactory.CreatePanel(name, screenRoot, new Color(1f, 1f, 1f, 0f));
            UIFactory.Stretch(screen.rectTransform, 0f, 0f, 0f, 0f);
            screenObjects.Add(screen.gameObject);
            return screen;
        }

        private Image CreateHeaderCard(Transform parent, out Text titleText, out Text subtitleText)
        {
            Image header = UIFactory.CreatePanel("HeaderCard", parent, cardColor);
            Text title = UIFactory.CreateText("Title", header.transform, font, string.Empty, 42, TextAnchor.UpperLeft, textDark, FontStyle.Bold);
            Text subtitle = UIFactory.CreateText("Subtitle", header.transform, font, string.Empty, 22, TextAnchor.UpperRight, mutedText, FontStyle.Bold);
            UIFactory.SetAnchors(title.rectTransform, new Vector2(0f, 0f), new Vector2(0.6f, 1f), new Vector2(24f, 18f), new Vector2(-12f, -18f));
            UIFactory.SetAnchors(subtitle.rectTransform, new Vector2(0.5f, 0f), new Vector2(1f, 1f), new Vector2(12f, 18f), new Vector2(-24f, -18f));
            titleText = title;
            subtitleText = subtitle;
            return header;
        }

        private MetricWidget CreateMetricCard(Transform parent, string label)
        {
            Image card = UIFactory.CreatePanel(label + "Card", parent, cardColor);
            UIFactory.AddLayoutElement(card.gameObject, 120, 120, 0, 0, 0, 1);
            UIFactory.AddVerticalLayout(card.gameObject, 4, new RectOffset(20, 20, 16, 16), TextAnchor.MiddleCenter, true, true);
            MetricWidget widget = new MetricWidget();
            widget.valueText = UIFactory.CreateText("Value", card.transform, font, "--", 34, TextAnchor.MiddleCenter, cardAccent, FontStyle.Bold);
            widget.labelText = UIFactory.CreateText("Label", card.transform, font, label, 20, TextAnchor.MiddleCenter, mutedText, FontStyle.Normal);
            UIFactory.AddLayoutElement(widget.valueText.gameObject, 48, 48, 1, 0, -1, 1);
            UIFactory.AddLayoutElement(widget.labelText.gameObject, 24, 32, 0, 0, -1, 1);
            return widget;
        }

        private MeterWidget CreateMeterCard(Transform parent, string title)
        {
            Image card = UIFactory.CreatePanel(title + "Card", parent, cardColor);
            UIFactory.AddLayoutElement(card.gameObject, 120, 120, 0, 0, -1, 1);
            UIFactory.AddVerticalLayout(card.gameObject, 10, new RectOffset(20, 20, 16, 16), TextAnchor.UpperLeft, true, false);

            RectTransform header = UIFactory.CreateUIObject("Header", card.transform);
            UIFactory.AddLayoutElement(header.gameObject, 34, 34, 0, 0, -1, 1);
            UIFactory.AddHorizontalLayout(header.gameObject, 6, new RectOffset(0, 0, 0, 0), TextAnchor.MiddleLeft, true, true);

            MeterWidget meter = new MeterWidget();
            meter.titleText = UIFactory.CreateText("Title", header, font, title, 24, TextAnchor.MiddleLeft, textDark, FontStyle.Bold);
            meter.valueText = UIFactory.CreateText("Value", header, font, "--", 22, TextAnchor.MiddleRight, mutedText, FontStyle.Bold);

            Image barBackground = UIFactory.CreatePanel("BarBackground", card.transform, new Color(0.83f, 0.84f, 0.82f, 1f));
            UIFactory.AddLayoutElement(barBackground.gameObject, 28, 28, 0, 0, -1, 1);

            RectTransform fillRoot = UIFactory.CreateUIObject("Fill", barBackground.transform);
            UIFactory.SetAnchors(fillRoot, new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(0f, 0f), new Vector2(0f, 0f));
            meter.fillImage = fillRoot.gameObject.AddComponent<Image>();
            meter.fillImage.color = cardAccent;
            meter.fillImage.type = Image.Type.Simple;

            return meter;
        }

        private Image CreateStaffCard(Transform parent, string title, out Text bodyText, out Button button, out Text buttonLabel)
        {
            Image card = UIFactory.CreatePanel(title + "Card", parent, cardColor);
            UIFactory.AddVerticalLayout(card.gameObject, 16, new RectOffset(24, 24, 24, 24), TextAnchor.UpperLeft, true, false);
            Text titleText = UIFactory.CreateText("Title", card.transform, font, title, 34, TextAnchor.UpperLeft, cardAccent, FontStyle.Bold);
            UIFactory.AddLayoutElement(titleText.gameObject, 44, 44, 0, 0, -1, 1);
            bodyText = UIFactory.CreateText("Body", card.transform, font, string.Empty, 22, TextAnchor.UpperLeft, textDark, FontStyle.Normal);
            UIFactory.AddLayoutElement(bodyText.gameObject, 0, -1, 1, 0, -1, 1);
            button = UIFactory.CreateButton("Button", card.transform, font, "Hire", 26, secondaryAccent, Color.white);
            UIFactory.AddLayoutElement(button.gameObject, 72, 72, 0, 0, -1, 1);
            buttonLabel = button.GetComponentInChildren<Text>();
            return card;
        }

        private void CreateUpgradeCard(Transform parent, UpgradeType type, out Text bodyText, out Button button, out Text buttonLabel)
        {
            Image card = UIFactory.CreatePanel(type.ToString() + "Card", parent, cardColor);
            UIFactory.AddVerticalLayout(card.gameObject, 14, new RectOffset(24, 24, 24, 24), TextAnchor.UpperLeft, true, false);
            Text title = UIFactory.CreateText("Title", card.transform, font, GameBalance.GetUpgradeTitle(type), 30, TextAnchor.UpperLeft, cardAccent, FontStyle.Bold);
            UIFactory.AddLayoutElement(title.gameObject, 42, 42, 0, 0, -1, 1);
            bodyText = UIFactory.CreateText("Body", card.transform, font, string.Empty, 21, TextAnchor.UpperLeft, textDark, FontStyle.Normal);
            UIFactory.AddLayoutElement(bodyText.gameObject, 0, -1, 1, 0, -1, 1);
            button = UIFactory.CreateButton("Button", card.transform, font, "Buy", 24, secondaryAccent, Color.white);
            UIFactory.AddLayoutElement(button.gameObject, 66, 66, 0, 0, -1, 1);
            buttonLabel = button.GetComponentInChildren<Text>();
        }

        private void RefreshMainMenu()
        {
            mainMenuView.continueButton.interactable = true;
            mainMenuView.savePathText.text = "Save file: " + gameManager.SaveManager.GetSavePath();
        }

        private void RefreshClinic()
        {
            SaveData save = gameManager.SaveData;
            DayRunData day = gameManager.CurrentDay;
            clinicView.titleText.text = "Clinic Dashboard";
            if (day == null)
            {
                clinicView.dayInfoText.text = "Tap Continue to build a fresh day.";
                clinicView.scheduleSummaryText.text = "No active day yet.";
                clinicView.startCaseButton.interactable = false;
                clinicView.nextCaseTitleText.text = "No case scheduled";
                clinicView.nextCaseTagsText.text = "Start a day from the main menu.";
                clinicView.nextCaseFlavorText.text = string.Empty;
                clinicView.lineupText.text = string.Empty;
            }
            else
            {
                clinicView.dayInfoText.text = "Day " + day.dayNumber + "  |  Remaining cases: " + day.GetRemainingCaseCount();
                clinicView.scheduleSummaryText.text = "Upkeep paid this morning: $" + day.upkeepPaid + "\n" +
                                                     "Scheduled cases: " + day.scheduledCases + "\n" +
                                                     "Clinic efficiency comes from Tech hires and the EMR upgrade.\n" +
                                                     "Friendly reminder: nobody gets a medal for rushing the withdrawal.";
                CaseData nextCase = gameManager.GetNextScheduledCase();
                clinicView.startCaseButton.interactable = nextCase != null;
                if (nextCase != null)
                {
                    clinicView.nextCaseTitleText.text = nextCase.title;
                    clinicView.nextCaseTagsText.text = BuildCaseTagString(nextCase);
                    clinicView.nextCaseFlavorText.text = nextCase.flavor;
                }
                else
                {
                    clinicView.nextCaseTitleText.text = "Day wrapped";
                    clinicView.nextCaseTagsText.text = "All listed cases are done.";
                    clinicView.nextCaseFlavorText.text = "Head to the results screen for the tally.";
                }

                List<CaseData> remaining = gameManager.GetRemainingCases();
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < remaining.Count; i++)
                {
                    builder.Append(i + 1);
                    builder.Append(". ");
                    builder.Append(remaining[i].title);
                    builder.Append("  |  ");
                    builder.Append(BuildCompactCaseTagString(remaining[i]));
                    if (i < remaining.Count - 1)
                    {
                        builder.AppendLine();
                    }
                }
                clinicView.lineupText.text = remaining.Count > 0 ? builder.ToString() : "No remaining cases in the queue.";
            }

            SetMetric(clinicView.moneyMetric, "$" + save.money, "Money");
            SetMetric(clinicView.reputationMetric, save.reputation.ToString(), "Reputation");
            SetMetric(clinicView.casesMetric, save.completedCases.ToString(), "Completed Cases");
            SetMetric(clinicView.cecalMetric, Mathf.RoundToInt(save.GetCecalCompletionRate() * 100f) + "%", "Cecal Rate");
            SetMetric(clinicView.polypsMetric, save.detectedPolyps.ToString(), "Detected Polyps");
            SetMetric(clinicView.withdrawalMetric, Mathf.RoundToInt(save.GetAverageWithdrawalQuality()).ToString(), "Avg Withdrawal");
        }

        private void RefreshProcedure()
        {
            ProcedureState state = gameManager.ProcedureManager.CurrentState;
            if (state == null)
            {
                procedureView.headerTitleText.text = "No active case";
                procedureView.headerStatsText.text = string.Empty;
                procedureView.headerTagsText.text = string.Empty;
                procedureView.helperText.text = "Start a case from the clinic screen.";
                procedureView.statusLogText.text = string.Empty;
                procedureView.resultOverlay.gameObject.SetActive(false);
                return;
            }

            procedureView.headerTitleText.text = state.caseData.title;
            procedureView.headerStatsText.text = "Day " + state.caseData.dayNumber + "  |  Money $" + gameManager.SaveData.money + "  |  Rep " + gameManager.SaveData.reputation;
            procedureView.headerTagsText.text = BuildCaseTagString(state.caseData);
            procedureView.chargesText.text = "Sedation charges: " + state.sedateChargesRemaining + "  |  Detected polyps: " + state.detectedPolyps;

            string progressLabel = state.phase == ProcedurePhase.Withdrawal ? "Withdrawal " + Mathf.RoundToInt(state.progress) + "%" : "To cecum " + Mathf.RoundToInt(state.progress) + "%";
            SetMeter(procedureView.progressMeter, "Progress", progressLabel, state.progress, cardAccent);
            SetMeter(procedureView.loopMeter, "Loop Tension", Mathf.RoundToInt(state.loopTension).ToString(), state.loopTension, softAccent);
            SetMeter(procedureView.comfortMeter, "Patient Comfort", Mathf.RoundToInt(state.patientComfort).ToString(), state.patientComfort, successColor);
            SetMeter(procedureView.visibilityMeter, "Visibility", Mathf.RoundToInt(state.visibility).ToString(), state.visibility, new Color(0.44f, 0.67f, 0.90f, 1f));

            if (state.phase == ProcedurePhase.Withdrawal)
            {
                procedureView.helperText.text = "Withdrawal phase. Pull Back finishes the case; Steer and Wash raise quality and lesion detection odds.";
            }
            else
            {
                procedureView.helperText.text = "Insertion phase. Push advances. Pull Back reduces loop. Pressure only shines when the loop is already substantial.";
            }

            procedureView.statusLogText.text = state.statusText;
            procedureView.resultOverlay.gameObject.SetActive(gameManager.LastProcedureResult != null);
            if (gameManager.LastProcedureResult != null)
            {
                ProcedureResult result = gameManager.LastProcedureResult;
                procedureView.resultTitleText.text = result.outcome == ProcedureOutcome.Success ? "Case Complete" : "Case Ended";
                procedureView.resultBodyText.text = result.summary + "\n\n" +
                                                   "Payout: $" + result.moneyEarned + "\n" +
                                                   "Reputation: " + (result.reputationChange >= 0 ? "+" : string.Empty) + result.reputationChange + "\n" +
                                                   "Detected polyps: " + result.detectedPolyps + "\n" +
                                                   "Withdrawal quality: " + Mathf.RoundToInt(result.withdrawalQuality) + " (" + result.withdrawalRating + ")\n" +
                                                   "Sedation spend: $" + result.supplySpend;
                procedureView.resultContinueButton.GetComponentInChildren<Text>().text =
                    gameManager.CurrentDay != null && gameManager.CurrentDay.currentCaseIndex >= gameManager.CurrentDay.cases.Count
                        ? "See End of Day"
                        : "Continue Day";
            }

            bool active = gameManager.LastProcedureResult == null;
            procedureView.pushButton.interactable = active;
            procedureView.pullBackButton.interactable = active;
            procedureView.steerButton.interactable = active;
            procedureView.pressureButton.interactable = active;
            procedureView.washButton.interactable = active;
            procedureView.sedateButton.interactable = active;
        }

        private void RefreshStaff()
        {
            StaffBonuses bonuses = gameManager.StaffManager.GetBonuses();
            staffView.headerText.text = "Staff Roster";
            staffView.payrollText.text = "Money $" + gameManager.SaveData.money + "  |  Daily payroll $" + bonuses.dailyUpkeep;

            int nurseCount = gameManager.StaffManager.GetCount(StaffType.Nurse);
            int techCount = gameManager.StaffManager.GetCount(StaffType.Tech);
            int nurseCost = gameManager.StaffManager.GetNextHireCost(StaffType.Nurse);
            int techCost = gameManager.StaffManager.GetNextHireCost(StaffType.Tech);

            staffView.nurseBodyText.text =
                "Comfort support: +" + (nurseCount * 2f).ToString("0.#") + " comfort per action\n" +
                "Pressure skill: +" + Mathf.RoundToInt((bonuses.pressureMultiplier - 1f) * 100f) + "%\n" +
                "Hired: " + nurseCount + "/" + GameBalance.MaxStaffPerType + "\n" +
                "Upkeep each: $" + GameBalance.GetStaffUpkeepPerHire(StaffType.Nurse) + "\n\n" +
                GameBalance.GetStaffDescription(StaffType.Nurse);

            staffView.techBodyText.text =
                "Turnover bonus: +" + techCount + " case(s) per day\n" +
                "Tool efficiency: +" + Mathf.RoundToInt((bonuses.toolEfficiencyMultiplier - 1f) * 100f) + "% wash/suction\n" +
                "Hired: " + techCount + "/" + GameBalance.MaxStaffPerType + "\n" +
                "Upkeep each: $" + GameBalance.GetStaffUpkeepPerHire(StaffType.Tech) + "\n\n" +
                GameBalance.GetStaffDescription(StaffType.Tech);

            bool nurseAffordable = gameManager.StaffManager.CanHire(StaffType.Nurse) && gameManager.SaveData.money >= nurseCost;
            bool techAffordable = gameManager.StaffManager.CanHire(StaffType.Tech) && gameManager.SaveData.money >= techCost;
            string nurseLockedLabel = gameManager.StaffManager.CanHire(StaffType.Nurse) ? "Need $" + nurseCost : "Maxed";
            string techLockedLabel = gameManager.StaffManager.CanHire(StaffType.Tech) ? "Need $" + techCost : "Maxed";

            UpdateActionButton(staffView.nurseHireButton, staffView.nurseButtonLabel, nurseAffordable, "Hire Nurse ($" + nurseCost + ")", nurseLockedLabel);
            UpdateActionButton(staffView.techHireButton, staffView.techButtonLabel, techAffordable, "Hire Tech ($" + techCost + ")", techLockedLabel);
        }

        private void RefreshUpgrades()
        {
            UpgradeManager manager = gameManager.UpgradeManager;
            upgradesView.headerText.text = "Upgrades  |  Money $" + gameManager.SaveData.money;

            RefreshUpgradeCard(UpgradeType.Scope, manager, upgradesView.scopeBodyText, upgradesView.scopeButton, upgradesView.scopeButtonLabel);
            RefreshUpgradeCard(UpgradeType.Monitor, manager, upgradesView.monitorBodyText, upgradesView.monitorButton, upgradesView.monitorButtonLabel);
            RefreshUpgradeCard(UpgradeType.SedationSupport, manager, upgradesView.sedationBodyText, upgradesView.sedationButton, upgradesView.sedationButtonLabel);
            RefreshUpgradeCard(UpgradeType.EMR, manager, upgradesView.emrBodyText, upgradesView.emrButton, upgradesView.emrButtonLabel);
        }

        private void RefreshEndDay()
        {
            DayRunData day = gameManager.CurrentDay;
            if (day == null)
            {
                endDayView.summaryText.text = "No day has been played yet.";
                endDayView.caseResultsText.text = string.Empty;
                return;
            }

            float averageQuality = day.GetAverageWithdrawalQuality();
            int net = day.GetGrossRevenue() - day.GetSupplySpend() - day.upkeepPaid;
            endDayView.headerText.text = "End of Day " + day.dayNumber;
            endDayView.summaryText.text =
                "Gross revenue: $" + day.GetGrossRevenue() + "\n" +
                "Sedation spend: $" + day.GetSupplySpend() + "\n" +
                "Payroll: $" + day.upkeepPaid + "\n" +
                "Net today: $" + net + "\n" +
                "Reputation delta: " + (day.GetReputationDelta() >= 0 ? "+" : string.Empty) + day.GetReputationDelta() + "\n" +
                "Average withdrawal quality: " + Mathf.RoundToInt(averageQuality) + "\n" +
                "Total detected polyps today: " + CountPolypsToday(day);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < day.results.Count; i++)
            {
                ProcedureResult result = day.results[i];
                builder.Append(i + 1);
                builder.Append(". ");
                builder.Append(result.caseTitle);
                builder.Append("  |  ");
                builder.Append(result.outcome == ProcedureOutcome.Success ? "Success" : result.outcome == ProcedureOutcome.AbortedDiscomfort ? "Aborted" : "Complication");
                builder.Append("  |  payout $");
                builder.Append(result.moneyEarned);
                builder.Append("  |  quality ");
                builder.Append(Mathf.RoundToInt(result.withdrawalQuality));
                if (result.detectedPolyps > 0)
                {
                    builder.Append("  |  polyps ");
                    builder.Append(result.detectedPolyps);
                }
                if (i < day.results.Count - 1)
                {
                    builder.AppendLine();
                }
            }
            endDayView.caseResultsText.text = day.results.Count > 0 ? builder.ToString() : "No cases logged yet.";
        }

        private void RefreshUpgradeCard(UpgradeType type, UpgradeManager manager, Text bodyText, Button button, Text buttonLabel)
        {
            int level = manager.GetLevel(type);
            int nextCost = manager.GetNextCost(type);
            bool canBuy = manager.CanBuy(type);
            bodyText.text =
                "Level: " + level + "/" + GameBalance.MaxUpgradeLevel + "\n" +
                GameBalance.GetUpgradeDescription(type) + "\n\n" +
                BuildUpgradeEffectText(type, level);

            bool affordable = canBuy && nextCost >= 0 && gameManager.SaveData.money >= nextCost;
            string lockedLabel = canBuy && nextCost >= 0 ? "Need $" + nextCost : "Maxed";
            UpdateActionButton(button, buttonLabel, affordable, nextCost >= 0 ? "Buy ($" + nextCost + ")" : "Maxed", lockedLabel);
        }

        private void SetMetric(MetricWidget widget, string value, string label)
        {
            widget.valueText.text = value;
            widget.labelText.text = label;
        }

        private void SetMeter(MeterWidget meter, string title, string value, float normalizedValue, Color fillColor)
        {
            meter.titleText.text = title;
            meter.valueText.text = value;
            meter.fillImage.color = fillColor;
            RectTransform fillTransform = meter.fillImage.rectTransform;
            float clamped = Mathf.Clamp01(normalizedValue / 100f);
            fillTransform.anchorMax = new Vector2(clamped, 1f);
            fillTransform.offsetMin = Vector2.zero;
            fillTransform.offsetMax = Vector2.zero;
        }

        private void UpdateActionButton(Button button, Text label, bool available, string availableLabel, string unavailableLabel)
        {
            button.interactable = available;
            label.text = available ? availableLabel : unavailableLabel;
        }

        private void SetAllScreensInactive()
        {
            for (int i = 0; i < screenObjects.Count; i++)
            {
                screenObjects[i].SetActive(false);
            }
        }

        private static Font LoadUIFont()
        {
            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (font != null) return font;

            font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            if (font != null) return font;

            string[] osFonts = Font.GetOSInstalledFontNames();
            if (osFonts != null && osFonts.Length > 0)
            {
                return Font.CreateDynamicFontFromOSFont(osFonts[0], 24);
            }

            return null;
        }

        private static int CountPolypsToday(DayRunData day)
        {
            int total = 0;
            for (int i = 0; i < day.results.Count; i++)
            {
                total += day.results[i].detectedPolyps;
            }

            return total;
        }

        private static string BuildCaseTagString(CaseData caseData)
        {
            return "Anatomy: " + caseData.anatomy +
                   "  |  Prep: " + caseData.prep +
                   "  |  Sedation: " + caseData.sedation +
                   "  |  Lesions: " + caseData.lesion;
        }

        private static string BuildCompactCaseTagString(CaseData caseData)
        {
            return caseData.anatomy + " / " + caseData.prep + " / " + caseData.sedation + " / " + caseData.lesion;
        }

        private static string BuildUpgradeEffectText(UpgradeType type, int level)
        {
            switch (type)
            {
                case UpgradeType.Scope:
                    return "Loop reduction: -" + (level * 6) + "%\nSteer bonus: +" + (level * 7) + "% to the next push";
                case UpgradeType.Monitor:
                    return "Start visibility: +" + (level * 7) + "\nDetection bonus: +" + Mathf.RoundToInt(level * 6f) + "%\nWash strength: +" + (level * 10) + "%";
                case UpgradeType.SedationSupport:
                    return "Comfort buffer: +" + (level * 6) + "\nSedation strength: +" + (level * 4);
                case UpgradeType.EMR:
                    return "Extra scheduled cases per day: +" + level;
                default:
                    return string.Empty;
            }
        }
    }
}
