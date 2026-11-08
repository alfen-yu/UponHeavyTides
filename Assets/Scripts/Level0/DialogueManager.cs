using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UponHeavyTides.Level0
{
    [System.Serializable]
    public class DialogueLine
    {
        public string speakerName;
        public Color speakerColor = Color.white;
        [TextArea(2, 5)] public string text;
    }

    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private TextMeshProUGUI speakerText;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private TextMeshProUGUI promptText;

        [Header("Settings")]
        [SerializeField] private float typingSpeed = 0.025f;

        private Queue<DialogueLine> linesQueue = new Queue<DialogueLine>();
        private bool isTyping = false;
        private string currentFullLine = "";
        private Action onDialogueCompleted;
        private Coroutine typingCoroutine;

        public bool IsDialogueActive => dialoguePanel != null && dialoguePanel.activeSelf;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (dialoguePanel == null)
            {
                CreateRuntimeUI();
            }

            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }
        }

        private void Update()
        {
            if (!IsDialogueActive) return;

            // Check for advance key: Space, E, Enter, or Mouse click
            bool advancePressed = Input.GetKeyDown(KeyCode.E) ||
                                  Input.GetKeyDown(KeyCode.Space) ||
                                  Input.GetKeyDown(KeyCode.Return) ||
                                  Input.GetMouseButtonDown(0);

            if (advancePressed)
            {
                if (isTyping)
                {
                    // Instant finish typing
                    StopCoroutine(typingCoroutine);
                    dialogueText.text = currentFullLine;
                    isTyping = false;
                }
                else
                {
                    DisplayNextLine();
                }
            }
        }

        public void StartDialogue(IEnumerable<DialogueLine> lines, Action onComplete = null)
        {
            linesQueue.Clear();
            foreach (var line in lines)
            {
                linesQueue.Enqueue(line);
            }

            onDialogueCompleted = onComplete;

            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(true);
            }

            DisplayNextLine();
        }

        public void DisplayNextLine()
        {
            if (linesQueue.Count == 0)
            {
                EndDialogue();
                return;
            }

            DialogueLine line = linesQueue.Dequeue();
            if (speakerText != null)
            {
                speakerText.text = line.speakerName;
                speakerText.color = line.speakerColor;
            }

            currentFullLine = line.text;

            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(TypeLineRoutine(line.text));
        }

        private IEnumerator TypeLineRoutine(string line)
        {
            isTyping = true;
            dialogueText.text = "";

            foreach (char c in line.ToCharArray())
            {
                dialogueText.text += c;
                yield return new WaitForSeconds(typingSpeed);
            }

            isTyping = false;
        }

        private void EndDialogue()
        {
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }

            var callback = onDialogueCompleted;
            onDialogueCompleted = null;
            callback?.Invoke();
        }

        private void CreateRuntimeUI()
        {
            // Create a Canvas
            GameObject canvasObj = new GameObject("DialogueCanvas");
            canvasObj.transform.SetParent(transform);
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasObj.AddComponent<GraphicRaycaster>();

            // Main Panel
            dialoguePanel = new GameObject("DialoguePanel");
            dialoguePanel.transform.SetParent(canvasObj.transform, false);
            RectTransform panelRect = dialoguePanel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.15f, 0.05f);
            panelRect.anchorMax = new Vector2(0.85f, 0.28f);
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            Image panelImg = dialoguePanel.AddComponent<Image>();
            panelImg.color = new Color(0.04f, 0.12f, 0.18f, 0.88f);

            // Header Banner
            GameObject headerObj = new GameObject("SpeakerText");
            headerObj.transform.SetParent(dialoguePanel.transform, false);
            RectTransform headerRect = headerObj.AddComponent<RectTransform>();
            headerRect.anchorMin = new Vector2(0.04f, 0.72f);
            headerRect.anchorMax = new Vector2(0.96f, 0.95f);
            headerRect.offsetMin = Vector2.zero;
            headerRect.offsetMax = Vector2.zero;
            speakerText = headerObj.AddComponent<TextMeshProUGUI>();
            speakerText.fontSize = 28;
            speakerText.fontStyle = FontStyles.Bold;
            speakerText.color = new Color(1f, 0.65f, 0.3f);
            speakerText.text = "Character Name";

            // Body Text
            GameObject bodyObj = new GameObject("DialogueText");
            bodyObj.transform.SetParent(dialoguePanel.transform, false);
            RectTransform bodyRect = bodyObj.AddComponent<RectTransform>();
            bodyRect.anchorMin = new Vector2(0.04f, 0.22f);
            bodyRect.anchorMax = new Vector2(0.96f, 0.70f);
            bodyRect.offsetMin = Vector2.zero;
            bodyRect.offsetMax = Vector2.zero;
            dialogueText = bodyObj.AddComponent<TextMeshProUGUI>();
            dialogueText.fontSize = 22;
            dialogueText.color = Color.white;
            dialogueText.text = "Dialogue content will appear here...";

            // Advance Prompt
            GameObject promptObj = new GameObject("PromptText");
            promptObj.transform.SetParent(dialoguePanel.transform, false);
            RectTransform promptRect = promptObj.AddComponent<RectTransform>();
            promptRect.anchorMin = new Vector2(0.5f, 0.04f);
            promptRect.anchorMax = new Vector2(0.96f, 0.22f);
            promptRect.offsetMin = Vector2.zero;
            promptRect.offsetMax = Vector2.zero;
            promptText = promptObj.AddComponent<TextMeshProUGUI>();
            promptText.fontSize = 16;
            promptText.alignment = TextAlignmentOptions.Right;
            promptText.color = new Color(0.8f, 0.9f, 1f, 0.6f);
            promptText.text = "Press [E] or [Space] to continue ▸";
        }
    }
}
