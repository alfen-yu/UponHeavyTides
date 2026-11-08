using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UponHeavyTides.Level0
{
    public class DialogueTrigger : MonoBehaviour
    {
        [Header("Dialogue Configuration")]
        [SerializeField] private List<DialogueLine> dialogueLines = new List<DialogueLine>();
        [SerializeField] private bool triggerOnEnter = false;
        [SerializeField] private bool triggerOnlyOnce = true;
        [SerializeField] private float interactRadius = 3f;

        [Header("Events")]
        public UnityEvent onDialogueFinished;

        private bool hasTriggered = false;
        private bool playerInRange = false;
        private Transform playerTransform;

        private void Start()
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) playerTransform = p.transform;
        }

        private void Update()
        {
            if (triggerOnEnter) return;
            if (triggerOnlyOnce && hasTriggered) return;

            if (playerTransform != null)
            {
                float dist = Vector3.Distance(transform.position, playerTransform.position);
                playerInRange = dist <= interactRadius;

                if (playerInRange && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return)))
                {
                    if (DialogueManager.Instance != null && !DialogueManager.Instance.IsDialogueActive)
                    {
                        TriggerDialogue();
                    }
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!triggerOnEnter) return;
            if (triggerOnlyOnce && hasTriggered) return;

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
                    onDialogueFinished?.Invoke();
                });
            }
        }

        public void AddLine(string speaker, string text, Color color)
        {
            dialogueLines.Add(new DialogueLine { speakerName = speaker, text = text, speakerColor = color });
        }
    }
}
