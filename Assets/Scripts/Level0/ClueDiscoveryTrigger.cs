using UnityEngine;
using System.Collections.Generic;

namespace UponHeavyTides.Level0
{
    public class ClueDiscoveryTrigger : MonoBehaviour
    {
        [Header("Clue Details")]
        [SerializeField] private string clueTitle = "Monster Clue Recorded";
        [TextArea(2, 4)]
        [SerializeField] private string clueContent = "The monster has a colossal sucker that is lined with swiveling hooks.";

        [Header("Blocking Rock Check")]
        [SerializeField] private Transform blockingBoulder;
        [SerializeField] private float requiredClearDistance = 2.2f;

        private bool hasDiscovered = false;
        private Vector3 initialBoulderPos;
        private bool boulderFound = false;

        private void Start()
        {
            if (blockingBoulder == null)
            {
                var b = GameObject.Find("FinalBoulder");
                if (b == null) b = GameObject.Find("Final_Trial_Boulder");
                if (b == null) b = GameObject.Find("Pushable_BoulderGate");
                if (b != null) blockingBoulder = b.transform;
            }

            if (blockingBoulder != null)
            {
                initialBoulderPos = blockingBoulder.position;
                boulderFound = true;
            }
        }

        private void Update()
        {
            if (hasDiscovered) return;

            // Constantly verify if the boulder has been moved away from its starting spot
            if (boulderFound && blockingBoulder != null)
            {
                float distMoved = Vector3.Distance(blockingBoulder.position, initialBoulderPos);
                if (distMoved >= requiredClearDistance)
                {
                    DiscoverClue();
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            CheckAndTrigger(other);
        }

        private void OnTriggerStay(Collider other)
        {
            CheckAndTrigger(other);
        }

        private void CheckAndTrigger(Collider other)
        {
            if (hasDiscovered) return;
            if (!other.CompareTag("Player") && other.GetComponent<UponHeavyTides.Player.PlayerMovement>() == null) return;

            if (boulderFound && blockingBoulder != null)
            {
                float distMoved = Vector3.Distance(blockingBoulder.position, initialBoulderPos);
                if (distMoved < requiredClearDistance)
                {
                    // Boulder still blocking the carving
                    if (DialogueManager.Instance != null && !DialogueManager.Instance.IsDialogueActive)
                    {
                        DialogueManager.Instance.StartDialogue(
                            "There's something carved into this cliff behind the heavy boulder! Walk into the boulder to push it aside!",
                            "Crab Companion"
                        );
                    }
                    return;
                }
            }

            DiscoverClue();
        }

        public void DiscoverClue()
        {
            if (hasDiscovered) return;
            hasDiscovered = true;

            Debug.Log("[Upon Heavy Tides] Monster Clue Revealed behind the boulder!");

            if (Level0Controller.Instance != null)
            {
                Level0Controller.Instance.OnClueDiscovered(clueTitle, clueContent);
            }
            else if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.StartDialogue(
                    "Look at that carved mark! " + clueContent,
                    "Crab Companion"
                );
            }
        }
    }
}
