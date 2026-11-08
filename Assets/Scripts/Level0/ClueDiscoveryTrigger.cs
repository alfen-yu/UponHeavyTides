using UnityEngine;
using System.Collections.Generic;

namespace UponHeavyTides.Level0
{
    public class ClueDiscoveryTrigger : MonoBehaviour
    {
        [Header("Clue Details")]
        [SerializeField] private string clueTitle = "Monster Clue Recorded";
        [TextArea(2, 4)]
        [SerializeField] private string clueContent = "The monster has a large sucker that is lined with swiveling hooks.";

        [Header("Blocking Rock (Optional Check)")]
        [SerializeField] private Transform blockingBoulder;
        [SerializeField] private float requiredClearDistance = 3.5f;

        private bool hasDiscovered = false;
        private Vector3 initialBoulderPos;

        private void Start()
        {
            if (blockingBoulder != null)
            {
                initialBoulderPos = blockingBoulder.position;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (hasDiscovered) return;

            if (other.CompareTag("Player") || other.GetComponent<UponHeavyTides.Player.PlayerMovement>() != null)
            {
                // Check if boulder has been pushed aside
                if (blockingBoulder != null)
                {
                    float distMoved = Vector3.Distance(blockingBoulder.position, initialBoulderPos);
                    if (distMoved < requiredClearDistance)
                    {
                        // Rock not moved yet, notify player
                        if (DialogueManager.Instance != null && !DialogueManager.Instance.IsDialogueActive)
                        {
                            DialogueManager.Instance.StartDialogue(new List<DialogueLine>
                            {
                                new DialogueLine
                                {
                                    speakerName = "Crab Companion",
                                    speakerColor = new Color(1f, 0.65f, 0.3f),
                                    text = "There's something carved into this cliff behind the heavy boulder! Walk into the boulder to push it away!"
                                }
                            });
                        }
                        return;
                    }
                }

                DiscoverClue();
            }
        }

        public void DiscoverClue()
        {
            if (hasDiscovered) return;
            hasDiscovered = true;

            if (Level0Controller.Instance != null)
            {
                Level0Controller.Instance.OnClueDiscovered(clueTitle, clueContent);
            }
        }
    }
}
