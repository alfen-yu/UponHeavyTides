using UnityEngine;

namespace UponHeavyTides.Level0
{
    public class ObjectiveTrigger : MonoBehaviour
    {
        [Header("Objective Message")]
        [TextArea(2, 4)]
        [SerializeField] private string objectiveText;
        [SerializeField] private bool triggerOnlyOnce = true;

        [Header("Optional Accompanying Dialogue")]
        [SerializeField] private bool triggerDialogueAsWell = false;
        [SerializeField] private string speakerName = "Guide";
        [TextArea(2, 4)]
        [SerializeField] private string dialogueText;

        private bool hasTriggered = false;

        private void OnTriggerEnter(Collider other)
        {
            if (hasTriggered && triggerOnlyOnce) return;
            if (!other.CompareTag("Player") && other.GetComponent<UponHeavyTides.Player.PlayerMovement>() == null) return;

            hasTriggered = true;

            if (Level0Controller.Instance != null && !string.IsNullOrEmpty(objectiveText))
            {
                Level0Controller.Instance.UpdateObjective(objectiveText);
            }

            if (triggerDialogueAsWell && !string.IsNullOrEmpty(dialogueText) && DialogueManager.Instance != null)
            {
                DialogueManager.Instance.StartDialogue(dialogueText, speakerName);
            }
        }
    }
}
