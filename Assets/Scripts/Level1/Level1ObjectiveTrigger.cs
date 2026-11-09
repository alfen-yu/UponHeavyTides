using UnityEngine;
using UponHeavyTides.Player;

namespace UponHeavyTides.Level1
{
    public class Level1ObjectiveTrigger : MonoBehaviour
    {
        public enum TriggerType
        {
            GoldenShell,
            ClueTablet
        }

        [SerializeField] private TriggerType triggerType = TriggerType.GoldenShell;
        [SerializeField] private float triggerRadius = 4.2f;
        private bool hasTriggered = false;

        public bool HasTriggered => hasTriggered;

        public void SetTriggerType(TriggerType type)
        {
            triggerType = type;
        }

        public void ResetTrigger()
        {
            hasTriggered = false;
        }

        private void Update()
        {
            if (hasTriggered) return;

            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                float dist = Vector3.Distance(transform.position, player.transform.position);
                if (dist <= triggerRadius)
                {
                    ActivateObjective();
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (hasTriggered) return;
            if (!other.CompareTag("Player") && other.GetComponent<PlayerMovement>() == null) return;

            ActivateObjective();
        }

        private void ActivateObjective()
        {
            if (hasTriggered) return;
            hasTriggered = true;

            if (triggerType == TriggerType.GoldenShell)
            {
                if (Level1Controller.Instance != null)
                {
                    Level1Controller.Instance.OnGoldenShellCollected();
                }
                gameObject.SetActive(false);
            }
            else if (triggerType == TriggerType.ClueTablet)
            {
                if (Level1Controller.Instance != null)
                {
                    Level1Controller.Instance.OnClueTabletDiscovered();
                }
            }
        }
    }
}
