using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UponHeavyTides.AI;
using UponHeavyTides.Level0;
using UponHeavyTides.Player;

namespace UponHeavyTides.Level1
{
    public class Level1Controller : MonoBehaviour
    {
        public static Level1Controller Instance { get; private set; }

        public enum Level1Stage
        {
            MeetHermitCrab,
            InfiltrateMaze,
            ObjectivesActive,
            ReturnToHermitCrab,
            LevelComplete
        }

        [Header("State")]
        [SerializeField] private Level1Stage currentStage = Level1Stage.MeetHermitCrab;
        [SerializeField] private bool hasFoundShell = false;
        [SerializeField] private bool hasDiscoveredClue = false;
        [SerializeField] private bool hasReturnedShell = false;

        [Header("Scene References")]
        [SerializeField] private DialogueTrigger hermitDialogue;
        [SerializeField] private Transform hermitCrabObj;
        [SerializeField] private GameObject restoredShellOnHermit;
        [SerializeField] private GameObject goldenShellInMaze;

        [Header("Maze Entry / Exit")]
        [SerializeField] private Vector3 mazeRespawnPosition = new Vector3(-20f, 1f, -22f);

        private TextMeshProUGUI objectiveText;
        private GameObject popupPanel;
        private TextMeshProUGUI popupTitle;
        private TextMeshProUGUI popupBody;
        private GameObject victoryPanel;
        private float lastCaughtTime = -10f;

        public Level1Stage CurrentStage => currentStage;
        public bool HasFoundShell => hasFoundShell;
        public bool HasDiscoveredClue => hasDiscoveredClue;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            CreateHUD();
            ResetLevelState();
        }

        private void Start()
        {
            ResetLevelState();
            FindSceneReferences();
            UpdateObjective("Sanctuary: Head right across the sands to speak with the Hermit Crab.");
        }

        public void ResetLevelState()
        {
            currentStage = Level1Stage.MeetHermitCrab;
            hasFoundShell = false;
            hasDiscoveredClue = false;
            hasReturnedShell = false;
            lastCaughtTime = -10f;
            if (popupPanel != null) popupPanel.SetActive(false);
            if (victoryPanel != null) victoryPanel.SetActive(false);
        }

        public void FindSceneReferences()
        {
            if (hermitCrabObj == null)
            {
                var h = GameObject.Find("HermitCrab_Ailing_NPC");
                if (h != null) hermitCrabObj = h.transform;
            }

            if (goldenShellInMaze == null)
            {
                var shellObj = GameObject.Find("Hermit_Golden_Shell");
                if (shellObj != null) goldenShellInMaze = shellObj;
                else
                {
                    var chamber = GameObject.Find("Objective_LostGoldenShell");
                    if (chamber != null)
                    {
                        var t = chamber.transform.Find("Hermit_Golden_Shell");
                        if (t != null) goldenShellInMaze = t.gameObject;
                    }
                }
            }

            if (hermitDialogue == null && hermitCrabObj != null)
            {
                hermitDialogue = hermitCrabObj.GetComponent<DialogueTrigger>();
                if (hermitDialogue != null)
                {
                    hermitDialogue.triggerOnlyOnce = true;
                    hermitDialogue.triggerOnEnter = true;
                    hermitDialogue.interactRadius = 4.5f;
                    hermitDialogue.ClearLines();
                    hermitDialogue.AddLine("Hermit Crab", "Oh, woe is me! Please don't look at me... I'm completely naked and defenseless without my shell!", new Color(1f, 0.55f, 0.3f));
                    hermitDialogue.AddLine("Crab Companion", "Where is your golden spiral shell, friend? It's dangerous out in the open ocean without your armor!", new Color(1f, 0.75f, 0.3f));
                    hermitDialogue.AddLine("Hermit Crab", "The barracudas! They ambushed me and chased me out of the kelp forest. My golden shell was knocked loose and dragged deep into that twisted maze to the north!", new Color(1f, 0.55f, 0.3f));
                    hermitDialogue.AddLine("Hermit Crab", "The ONLY entrance into the maze is through the archway on the left wall! The rest of the ridge is completely blocked by stone!", new Color(1f, 0.55f, 0.3f));
                    hermitDialogue.AddLine("Hermit Crab", "Watch out for the two barracuda patrol groups! If they catch you, they'll chase you all the way back out! Duck into the 3 glowing kelp hiding zones along the paths to stay out of their sight!", new Color(1f, 0.55f, 0.3f));
                    hermitDialogue.AddLine("Crab Companion", "Don't you worry, friend! We will sneak in through the left archway, evade the barracudas, and bring your shell back safely!", new Color(1f, 0.75f, 0.3f));

                    hermitDialogue.onFinishedCallback = OnHermitCrabInitialSpokenTo;
                }
            }
        }

        private void Update()
        {
            // Check if player returned to Hermit Crab with the shell
            if (hasFoundShell && !hasReturnedShell && hermitCrabObj != null)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    float dist = Vector3.Distance(player.transform.position, hermitCrabObj.position);
                    if (dist <= 4.5f && DialogueManager.Instance != null && !DialogueManager.Instance.IsDialogueActive)
                    {
                        ReturnShellToHermitCrab();
                    }
                }
            }

            // Check if Objective Complete
            if (hasReturnedShell)
            {
                if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
                {
                    return;
                }

                var kb = UnityEngine.InputSystem.Keyboard.current;
                if (kb != null && (kb.enterKey.wasPressedThisFrame || kb.numpadEnterKey.wasPressedThisFrame))
                {
                    UpdateObjective("<color=#00FFCC>Objective Complete</color>");
                }
            }
        }

        public void UpdateObjective(string text)
        {
            if (objectiveText != null)
            {
                objectiveText.text = $"<color=#00FFCC>[OBJECTIVE]</color> {text}";
            }
        }

        public void OnHermitCrabInitialSpokenTo()
        {
            if (currentStage == Level1Stage.MeetHermitCrab)
            {
                currentStage = Level1Stage.InfiltrateMaze;
                UpdateObjective("Kelp Maze: Enter through the left archway, evade barracudas using the 3 hiding zones, and retrieve the Golden Shell & Clue Tablet!");
            }
        }

        public void OnPlayerCaughtByBarracuda()
        {
            if (Time.time < lastCaughtTime + 2.0f) return;
            lastCaughtTime = Time.time;

            // Teleport player back outside the left archway
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                var cc = player.GetComponent<CharacterController>();
                if (cc != null) cc.enabled = false;
                player.transform.position = mazeRespawnPosition;
                if (cc != null) cc.enabled = true;
            }

            // Teleport Crab companion
            var crab = GameObject.Find("Crab");
            if (crab != null)
            {
                Vector3 crabPos = mazeRespawnPosition + new Vector3(-2f, -0.6f, 0f);
                var agent = crab.GetComponent<UnityEngine.AI.NavMeshAgent>();
                if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
                {
                    agent.Warp(crabPos);
                }
                else
                {
                    crab.transform.position = crabPos;
                }
            }

            // Reset all barracudas to their starting patrol points
            var barracudas = Object.FindObjectsByType<BarracudaAI>(FindObjectsSortMode.None);
            foreach (var b in barracudas)
            {
                if (b != null) b.ResetToPatrol();
            }

            // Drop shell if player had collected it!
            bool droppedShell = false;
            if (hasFoundShell && !hasReturnedShell)
            {
                hasFoundShell = false;
                droppedShell = true;

                if (goldenShellInMaze != null)
                {
                    goldenShellInMaze.SetActive(true);
                    var trig = goldenShellInMaze.GetComponent<Level1ObjectiveTrigger>();
                    if (trig != null) trig.ResetTrigger();
                }
                else
                {
                    var chamber = GameObject.Find("Objective_LostGoldenShell");
                    if (chamber != null)
                    {
                        var shellChild = chamber.transform.Find("Hermit_Golden_Shell");
                        if (shellChild != null)
                        {
                            goldenShellInMaze = shellChild.gameObject;
                            goldenShellInMaze.SetActive(true);
                            goldenShellInMaze.GetComponent<Level1ObjectiveTrigger>()?.ResetTrigger();
                        }
                    }
                }
            }

            // Alert dialogue from Crab
            if (DialogueManager.Instance != null)
            {
                if (droppedShell)
                {
                    DialogueManager.Instance.StartDialogue(
                        "Watch out! That barracuda knocked the golden shell right out of your grip and dragged it back to the pedestal in the maze! Let's sneak back in and retrieve it again!",
                        "Crab Companion"
                    );
                }
                else
                {
                    DialogueManager.Instance.StartDialogue(
                        "Watch out! That barracuda spotted us and chased us right out of the maze! Let's sneak back in through the left archway and duck into the glowing kelp groves when they swim near!",
                        "Crab Companion"
                    );
                }
            }

            if (droppedShell)
            {
                if (hasDiscoveredClue)
                {
                    currentStage = Level1Stage.ObjectivesActive;
                    UpdateObjective("Shell Lost! Retrieve the Lost Golden Shell again from the central pedestal.");
                }
                else
                {
                    currentStage = Level1Stage.InfiltrateMaze;
                    UpdateObjective("Shell Lost! Infiltrate the maze to retrieve the Golden Shell and find the Clue Tablet.");
                }
            }
            else
            {
                UpdateObjective("Evade the barracudas! Enter through the left archway and use the 3 glowing hiding zones.");
            }
        }

        public void OnGoldenShellCollected()
        {
            if (hasFoundShell) return;
            hasFoundShell = true;

            if (hasDiscoveredClue)
            {
                currentStage = Level1Stage.ReturnToHermitCrab;
                UpdateObjective("Both Objectives Complete! Exit through the left archway and return the shell to the Hermit Crab!");
                if (DialogueManager.Instance != null)
                {
                    DialogueManager.Instance.StartDialogue(
                        "You got the Golden Shell! We now have both the shell and the second monster clue! Let's exit through the left archway and bring the shell back to the Hermit Crab!",
                        "Crab Companion"
                    );
                }
            }
            else
            {
                currentStage = Level1Stage.ObjectivesActive;
                UpdateObjective("Golden Shell Retrieved! Find the Monster Clue Tablet at the far northern altar.");
                if (DialogueManager.Instance != null)
                {
                    DialogueManager.Instance.StartDialogue(
                        "You found the Hermit Crab's golden shell! Now head north to discover the monster clue carved upon the altar!",
                        "Crab Companion"
                    );
                }
            }
        }

        public void OnClueTabletDiscovered()
        {
            if (hasDiscoveredClue) return;
            hasDiscoveredClue = true;

            if (popupPanel != null)
            {
                popupPanel.SetActive(true);
                if (popupTitle != null) popupTitle.text = "Second Clue: Serrated Beak";
                if (popupBody != null) popupBody.text = "The beast possesses an enormous razor-sharp beak powerful enough to crush ancient reefs and shells.";
                CancelInvoke(nameof(HidePopupPanel));
                Invoke(nameof(HidePopupPanel), 5.0f);
            }

            if (hasFoundShell)
            {
                currentStage = Level1Stage.ReturnToHermitCrab;
                UpdateObjective("Both Objectives Complete! Exit through the left archway and return the shell to the Hermit Crab!");
                if (DialogueManager.Instance != null)
                {
                    DialogueManager.Instance.StartDialogue(
                        "The second clue has been recorded in your journal! And you already have the golden shell! Let's exit through the left archway and return the shell to our friend!",
                        "Crab Companion"
                    );
                }
            }
            else
            {
                currentStage = Level1Stage.ObjectivesActive;
                UpdateObjective("Clue Tablet Recorded! Retrieve the Lost Golden Shell from the central chamber.");
                if (DialogueManager.Instance != null)
                {
                    DialogueManager.Instance.StartDialogue(
                        "Look at that carved mark! The beast has a colossal beak! Now let's retrieve the golden shell from the central chamber!",
                        "Crab Companion"
                    );
                }
            }
        }

        private void HidePopupPanel()
        {
            if (popupPanel != null) popupPanel.SetActive(false);
        }

        private void ReturnShellToHermitCrab()
        {
            hasReturnedShell = true;
            currentStage = Level1Stage.LevelComplete;

            // Restore shell on Hermit Crab model if present
            if (restoredShellOnHermit != null)
            {
                restoredShellOnHermit.SetActive(true);
            }

            UpdateObjective("<color=#00FFCC>Objective Complete</color>");

            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.StartDialogue(new List<DialogueLine>
                {
                    new DialogueLine
                    {
                        speakerName = "Hermit Crab",
                        speakerColor = new Color(1f, 0.55f, 0.3f),
                        text = "My golden spiral shell! You actually sneaked into the maze, survived the predators, and brought it back! I can finally wear my armor again!"
                    },
                    new DialogueLine
                    {
                        speakerName = "Crab Companion",
                        speakerColor = new Color(1f, 0.75f, 0.3f),
                        text = "We're happy to help, friend! We also uncovered the monster's second clue—it possesses a massive razor-sharp beak that crushed these waters."
                    },
                    new DialogueLine
                    {
                        speakerName = "Hermit Crab",
                        speakerColor = new Color(1f, 0.55f, 0.3f),
                        text = "You are true heroes of the shallows! Thank you, brave travelers! The journey ahead is perilous, but your courage is unmatched!"
                    }
                }, () =>
                {
                    UpdateObjective("<color=#00FFCC>Objective Complete</color>");
                });
            }
        }

        private void CreateHUD()
        {
            GameObject hudCanvasObj = new GameObject("Level1_HUD_Canvas");
            hudCanvasObj.transform.SetParent(transform);
            Canvas canvas = hudCanvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 50;

            var scaler = hudCanvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            hudCanvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();

            // Objective
            GameObject bannerObj = new GameObject("ObjectiveBanner");
            bannerObj.transform.SetParent(hudCanvasObj.transform, false);
            RectTransform bannerRect = bannerObj.AddComponent<RectTransform>();
            bannerRect.anchorMin = new Vector2(0.15f, 0.90f);
            bannerRect.anchorMax = new Vector2(0.85f, 0.97f);
            bannerRect.offsetMin = Vector2.zero;
            bannerRect.offsetMax = Vector2.zero;

            var bannerImg = bannerObj.AddComponent<UnityEngine.UI.Image>();
            bannerImg.color = new Color(0.04f, 0.16f, 0.14f, 0.88f);

            GameObject objTextObj = new GameObject("ObjectiveText");
            objTextObj.transform.SetParent(bannerObj.transform, false);
            RectTransform textRect = objTextObj.AddComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0.02f, 0f);
            textRect.anchorMax = new Vector2(0.98f, 1f);
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            objectiveText = objTextObj.AddComponent<TextMeshProUGUI>();
            objectiveText.fontSize = 20;
            objectiveText.fontStyle = FontStyles.Bold;
            objectiveText.color = Color.white;
            objectiveText.alignment = TextAlignmentOptions.Center;

            // Clue Popup
            popupPanel = new GameObject("CluePopupPanel");
            popupPanel.transform.SetParent(hudCanvasObj.transform, false);
            RectTransform cpRect = popupPanel.AddComponent<RectTransform>();
            cpRect.anchorMin = new Vector2(0.25f, 0.65f);
            cpRect.anchorMax = new Vector2(0.75f, 0.85f);
            cpRect.offsetMin = Vector2.zero;
            cpRect.offsetMax = Vector2.zero;
            var cpBg = popupPanel.AddComponent<UnityEngine.UI.Image>();
            cpBg.color = new Color(0.05f, 0.25f, 0.22f, 0.92f);

            GameObject cpTitleObj = new GameObject("ClueTitle");
            cpTitleObj.transform.SetParent(popupPanel.transform, false);
            RectTransform cptRect = cpTitleObj.AddComponent<RectTransform>();
            cptRect.anchorMin = new Vector2(0.05f, 0.60f);
            cptRect.anchorMax = new Vector2(0.95f, 0.92f);
            cptRect.offsetMin = Vector2.zero;
            cptRect.offsetMax = Vector2.zero;
            popupTitle = cpTitleObj.AddComponent<TextMeshProUGUI>();
            popupTitle.fontSize = 24;
            popupTitle.fontStyle = FontStyles.Bold;
            popupTitle.color = new Color(0.3f, 1f, 0.8f);
            popupTitle.alignment = TextAlignmentOptions.Center;
            popupTitle.text = "CLUE RECORDED";

            GameObject cpBodyObj = new GameObject("ClueBody");
            cpBodyObj.transform.SetParent(popupPanel.transform, false);
            RectTransform cpbRect = cpBodyObj.AddComponent<RectTransform>();
            cpbRect.anchorMin = new Vector2(0.05f, 0.10f);
            cpbRect.anchorMax = new Vector2(0.95f, 0.58f);
            cpbRect.offsetMin = Vector2.zero;
            cpbRect.offsetMax = Vector2.zero;
            popupBody = cpBodyObj.AddComponent<TextMeshProUGUI>();
            popupBody.fontSize = 18;
            popupBody.color = Color.white;
            popupBody.alignment = TextAlignmentOptions.Center;
            popupBody.text = "...";

            popupPanel.SetActive(false);

            
            victoryPanel = null;
        }
    }
}
