using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace UponHeavyTides.Level0
{
    public class Level0Controller : MonoBehaviour
    {
        public static Level0Controller Instance { get; private set; }

        [Header("State")]
        public int collectiblesCollected = 0;
        public int totalCollectibles = 5;
        public bool hasMetElderTurtle = false;
        public bool hasDiscoveredClue = false;

        [Header("UI Elements")]
        private TextMeshProUGUI objectiveText;
        private TextMeshProUGUI collectiblesCounterText;
        private GameObject cluePopupPanel;
        private TextMeshProUGUI clueTitleText;
        private TextMeshProUGUI clueBodyText;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            // Load persisted state
            int savedCollected;
            bool savedMetTurtle;
            bool savedClue;
            Level0Save.Load(out savedCollected, out savedMetTurtle, out savedClue);
            collectiblesCollected = savedCollected;
            hasMetElderTurtle = savedMetTurtle;
            hasDiscoveredClue = savedClue;

            CreateHUD();
        }

        private void Start()
        {
            UpdateObjective("Awakening: Talk to the nervous crab sheltering behind the rock.");
            UpdateCollectiblesUI();
        }

        public void UpdateObjective(string text)
        {
            if (objectiveText != null)
            {
                objectiveText.text = $"<color=#FFD700>◆ OBJECTIVE:</color> {text}";
            }
        }

        public void AddCollectible(string name)
        {
            collectiblesCollected++;
            UpdateCollectiblesUI();
            Level0Save.Save(collectiblesCollected, hasMetElderTurtle, hasDiscoveredClue);

            if (collectiblesCollected >= totalCollectibles)
            {
                UpdateObjective("All relics gathered! Find the Elder Sea Turtle to learn the next step.");
            }

            // Crab reaction comment (first collectible)
            if (DialogueManager.Instance != null && !DialogueManager.Instance.IsDialogueActive && collectiblesCollected == 1)
            {
                DialogueManager.Instance.StartDialogue(new List<DialogueLine>
                {
                    new DialogueLine
                    {
                        speakerName = "Crab Companion",
                        speakerColor = new Color(1f, 0.65f, 0.3f),
                        text = "Nice find! Those sunlit pearls and shells carry ancient memories of the shallows. Gather as many as you like before we leave!"
                    }
                });
            }
        }

        private void UpdateCollectiblesUI()
        {
            if (collectiblesCounterText != null)
            {
                collectiblesCounterText.text = $"★ Shallows Relics: {collectiblesCollected}/{totalCollectibles}";
            }
        }

        public void OnElderTurtleMet()
        {
            hasMetElderTurtle = true;
            UpdateObjective("The Way of the Water: Practice moving (WASD), jumping (Space), and pushing stones on your way to the final gate.");
            Level0Save.Save(collectiblesCollected, hasMetElderTurtle, hasDiscoveredClue);
        }

        public void OnClueDiscovered(string title, string content)
        {
            hasDiscoveredClue = true;
            UpdateObjective("<color=#00FF7F>Tutorial Complete!</color> The first monster clue has been recorded in your journal.");

            if (cluePopupPanel != null)
            {
                cluePopupPanel.SetActive(true);
                if (clueTitleText != null) clueTitleText.text = title;
                if (clueBodyText != null) clueBodyText.text = content;
            }

            // Save progress
            Level0Save.Save(collectiblesCollected, hasMetElderTurtle, hasDiscoveredClue);

            // Post‑discovery dialogue
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.StartDialogue(new List<DialogueLine>
                {
                    new DialogueLine
                    {
                        speakerName = "Crab Companion",
                        speakerColor = new Color(1f, 0.65f, 0.3f),
                        text = "By the Great Current...! Look at the sheer size of that sucker scar! It's lined with sharp hooked barbs..."
                    },
                    new DialogueLine
                    {
                        speakerName = "Crab Companion",
                        speakerColor = new Color(1f, 0.65f, 0.3f),
                        text = "Your notebook is glowing! The prophecy was true—you have sketched the beak of the terror haunting our seas. The path to the Kelp Forest is now open!"
                    }
                });
            }

            // Show level complete UI
            CompleteLevel();
        }

        private void CreateHUD()
        {
            GameObject hudCanvasObj = new GameObject("Level0_HUD_Canvas");
            hudCanvasObj.transform.SetParent(transform);
            Canvas canvas = hudCanvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 50;

            var scaler = hudCanvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            hudCanvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();

            // Objective Box
            GameObject objContainer = new GameObject("ObjectiveContainer");
            objContainer.transform.SetParent(hudCanvasObj.transform, false);
            RectTransform objRect = objContainer.AddComponent<RectTransform>();
            objRect.anchorMin = new Vector2(0.02f, 0.90f);
            objRect.anchorMax = new Vector2(0.60f, 0.98f);
            objRect.offsetMin = Vector2.zero;
            objRect.offsetMax = Vector2.zero;
            var objBg = objContainer.AddComponent<UnityEngine.UI.Image>();
            objBg.color = new Color(0.02f, 0.08f, 0.12f, 0.75f);

            GameObject objTextObj = new GameObject("ObjectiveText");
            objTextObj.transform.SetParent(objContainer.transform, false);
            RectTransform tRect = objTextObj.AddComponent<RectTransform>();
            tRect.anchorMin = new Vector2(0.03f, 0.1f);
            tRect.anchorMax = new Vector2(0.97f, 0.9f);
            tRect.offsetMin = Vector2.zero;
            tRect.offsetMax = Vector2.zero;
            objectiveText = objTextObj.AddComponent<TextMeshProUGUI>();
            objectiveText.fontSize = 20;
            objectiveText.color = Color.white;
            objectiveText.text = "Loading Objective...";

            // Collectibles Box
            GameObject colContainer = new GameObject("CollectiblesContainer");
            colContainer.transform.SetParent(hudCanvasObj.transform, false);
            RectTransform colRect = colContainer.AddComponent<RectTransform>();
            colRect.anchorMin = new Vector2(0.78f, 0.90f);
            colRect.anchorMax = new Vector2(0.98f, 0.98f);
            colRect.offsetMin = Vector2.zero;
            colRect.offsetMax = Vector2.zero;
            var colBg = colContainer.AddComponent<UnityEngine.UI.Image>();
            colBg.color = new Color(0.02f, 0.08f, 0.12f, 0.75f);

            GameObject colTextObj = new GameObject("CollectiblesText");
            colTextObj.transform.SetParent(colContainer.transform, false);
            RectTransform ctRect = colTextObj.AddComponent<RectTransform>();
            ctRect.anchorMin = new Vector2(0.05f, 0.1f);
            ctRect.anchorMax = new Vector2(0.95f, 0.9f);
            ctRect.offsetMin = Vector2.zero;
            ctRect.offsetMax = Vector2.zero;
            collectiblesCounterText = colTextObj.AddComponent<TextMeshProUGUI>();
            collectiblesCounterText.fontSize = 20;
            collectiblesCounterText.color = new Color(1f, 0.85f, 0.3f);
            collectiblesCounterText.alignment = TextAlignmentOptions.Center;
            collectiblesCounterText.text = $"★ Shallows Relics: {collectiblesCollected}/{totalCollectibles}";

            // Clue Popup
            cluePopupPanel = new GameObject("CluePopupPanel");
            cluePopupPanel.transform.SetParent(hudCanvasObj.transform, false);
            RectTransform cpRect = cluePopupPanel.AddComponent<RectTransform>();
            cpRect.anchorMin = new Vector2(0.25f, 0.65f);
            cpRect.anchorMax = new Vector2(0.75f, 0.85f);
            cpRect.offsetMin = Vector2.zero;
            cpRect.offsetMax = Vector2.zero;
            var cpBg = cluePopupPanel.AddComponent<UnityEngine.UI.Image>();
            cpBg.color = new Color(0.05f, 0.25f, 0.22f, 0.92f);

            GameObject cpTitleObj = new GameObject("ClueTitle");
            cpTitleObj.transform.SetParent(cluePopupPanel.transform, false);
            RectTransform cptRect = cpTitleObj.AddComponent<RectTransform>();
            cptRect.anchorMin = new Vector2(0.05f, 0.60f);
            cptRect.anchorMax = new Vector2(0.95f, 0.92f);
            cptRect.offsetMin = Vector2.zero;
            cptRect.offsetMax = Vector2.zero;
            clueTitleText = cpTitleObj.AddComponent<TextMeshProUGUI>();
            clueTitleText.fontSize = 24;
            clueTitleText.fontStyle = FontStyles.Bold;
            clueTitleText.color = new Color(0.3f, 1f, 0.8f);
            clueTitleText.alignment = TextAlignmentOptions.Center;
            clueTitleText.text = "CLUE RECORDED";

            GameObject cpBodyObj = new GameObject("ClueBody");
            cpBodyObj.transform.SetParent(cluePopupPanel.transform, false);
            RectTransform cpbRect = cpBodyObj.AddComponent<RectTransform>();
            cpbRect.anchorMin = new Vector2(0.05f, 0.10f);
            cpbRect.anchorMax = new Vector2(0.95f, 0.58f);
            cpbRect.offsetMin = Vector2.zero;
            cpbRect.offsetMax = Vector2.zero;
            clueBodyText = cpBodyObj.AddComponent<TextMeshProUGUI>();
            clueBodyText.fontSize = 18;
            clueBodyText.color = Color.white;
            clueBodyText.alignment = TextAlignmentOptions.Center;
            clueBodyText.text = "...";

            cluePopupPanel.SetActive(false);
        }

        private void CompleteLevel()
        {
            GameObject panel = new GameObject("LevelCompletePanel");
            panel.transform.SetParent(GameObject.Find("Level0_HUD_Canvas").transform, false);
            var rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.25f, 0.4f);
            rect.anchorMax = new Vector2(0.75f, 0.6f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var img = panel.AddComponent<UnityEngine.UI.Image>();
            img.color = new Color(0f, 0.5f, 0f, 0.85f);
            var txtObj = new GameObject("Message");
            txtObj.transform.SetParent(panel.transform, false);
            var txtRect = txtObj.AddComponent<RectTransform>();
            txtRect.anchorMin = new Vector2(0.1f, 0.2f);
            txtRect.anchorMax = new Vector2(0.9f, 0.8f);
            txtRect.offsetMin = Vector2.zero;
            txtRect.offsetMax = Vector2.zero;
            var txt = txtObj.AddComponent<TextMeshProUGUI>();
            txt.text = "Level 0 Complete!\nProceed to Level 1 when ready.";
            txt.fontSize = 28;
            txt.alignment = TMPro.TextAlignmentOptions.Center;
            txt.color = Color.white;
        }
    }
}
