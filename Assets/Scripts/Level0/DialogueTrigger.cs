using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace UponHeavyTides.Level0
{
    public class DialogueTrigger : MonoBehaviour
    {
        [Header("Dialogue Configuration")]
        [SerializeField] private List<DialogueLine> dialogueLines = new List<DialogueLine>();
        [SerializeField] public bool triggerOnEnter = false;
        [SerializeField] public bool triggerOnlyOnce = true;
        [SerializeField] public float interactRadius = 4.5f;

        [Header("Events")]
        public UnityEvent onDialogueFinished;
        public Action onFinishedCallback;

        private bool hasTriggered = false;
        private bool playerInRange = false;
        private Transform playerTransform;

        private static float lastDialogueEndTime = -10f;

        public bool HasTriggered => hasTriggered;

        private void Start()
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) playerTransform = p.transform;
        }

        private void Update()
        {
            if (hasTriggered && triggerOnlyOnce) return;

            // Prevent re-triggering immediately when another dialogue just ended
            if (Time.time < lastDialogueEndTime + 0.8f) return;

            if (playerTransform == null)
            {
                var p = GameObject.FindGameObjectWithTag("Player");
                if (p != null) playerTransform = p.transform;
                if (playerTransform == null) return;
            }

            float dist = Vector3.Distance(transform.position, playerTransform.position);
            playerInRange = dist <= interactRadius;

            if (triggerOnEnter)
            {
                if (playerInRange && DialogueManager.Instance != null && !DialogueManager.Instance.IsDialogueActive)
                {
                    TriggerDialogue();
                }
                return;
            }

            bool interactPressed = false;
            var keyboard = Keyboard.current;
            if (keyboard != null)
            {
                interactPressed = keyboard.eKey.wasPressedThisFrame;
            }

            if (playerInRange && interactPressed)
            {
                if (DialogueManager.Instance != null && !DialogueManager.Instance.IsDialogueActive)
                {
                    TriggerDialogue();
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (hasTriggered && triggerOnlyOnce) return;
            if (!triggerOnEnter) return;
            if (Time.time < lastDialogueEndTime + 0.8f) return;

            if (other.CompareTag("Player") || other.GetComponent<UponHeavyTides.Player.PlayerMovement>() != null)
            {
                TriggerDialogue();
            }
        }

        public void TriggerDialogue()
        {
            if (hasTriggered && triggerOnlyOnce) return;
            hasTriggered = true;

            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.StartDialogue(dialogueLines, () =>
                {
                    lastDialogueEndTime = Time.time;
                    onDialogueFinished?.Invoke();
                    onFinishedCallback?.Invoke();

                    // Direct progression safety call
                    if (name == "Crab")
                    {
                        Level0StoryFlow.Instance?.OnCrabIntroFinished();
                    }
                    else if (name == "Elder_Sea_Turtle" || name == "ElderSeaTurtle")
                    {
                        Level0StoryFlow.Instance?.OnTurtleDialogueFinished();
                    }
                    else if (name == "School_Of_Fish_NPC" || name == "SchoolOfFish")
                    {
                        Level0StoryFlow.Instance?.OnFishDialogueFinished();
                    }

                    if (triggerOnlyOnce)
                    {
                        enabled = false;
                    }
                });
            }
        }

        public void ClearLines()
        {
            dialogueLines.Clear();
        }

        public void AddLine(string speaker, string text, Color color)
        {
            dialogueLines.Add(new DialogueLine { speakerName = speaker, text = text, speakerColor = color });
        }

        public void ResetTrigger()
        {
            hasTriggered = false;
            enabled = true;
        }
    }
}
