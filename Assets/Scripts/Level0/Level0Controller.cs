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
        public int totalCollectibles = 6;
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

            // Ensure story progression is attached to managers
            if (GetComponent<Level0StoryFlow>() == null)
            {
                gameObject.AddComponent<Level0StoryFlow>();
            }

            // Always start Level 0 tutorial fresh
            collectiblesCollected = 0;
            hasMetElderTurtle = false;
            hasDiscoveredClue = false;
            Level0Save.Save(0, false, false);

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
                objectiveText.text = $"<color=#FFD700>[OBJECTIVE]</color> {text}";
            }
        }

        public void AddCollectible(string name)
        {
            collectiblesCollected = Mathf.Min(collectiblesCollected + 1, totalCollectibles);
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
                collectiblesCounterText.text = $"Shallows Relics: {collectiblesCollected}/{totalCollectibles}";
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
            UpdateObjective("<color=#00FFCC>Clue Recorded!</color> Monster sucker mark found! Press [Enter] to enter Level 1.");

            // Save progress
            Level0Save.Save(collectiblesCollected, hasMetElderTurtle, hasDiscoveredClue);

            // Disable all world dialogue triggers so no other dialogue can ever interrupt or re-fire
            foreach (var dt in FindObjectsByType<DialogueTrigger>(FindObjectsSortMode.None))
            {
                dt.enabled = false;
            }

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
                        text = "Your notebook is glowing! The prophecy was true—you have sketched the beak and sucker of the beast. Press [Enter] whenever you are ready to proceed to the Kelp Forest!"
                    }
                });
            }
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
            collectiblesCounterText.text = $"Shallows Relics: {collectiblesCollected}/{totalCollectibles}";

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

        private void Update()
        {
            if (hasDiscoveredClue)
            {
                if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
                {
                    return;
                }

                var kb = UnityEngine.InputSystem.Keyboard.current;
                bool enterPressed = kb != null && (kb.enterKey.wasPressedThisFrame || kb.numpadEnterKey.wasPressedThisFrame);
                if (enterPressed)
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene("Level1");
                }
            }
        }
    }
}
