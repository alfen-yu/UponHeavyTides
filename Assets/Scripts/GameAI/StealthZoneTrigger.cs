using UnityEngine;
using UponHeavyTides.Player;

namespace UponHeavyTides.AI
{
    public class StealthZoneTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") || other.GetComponent<PlayerMovement>() != null)
            {
                NotifyBarracudas(true);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player") || other.GetComponent<PlayerMovement>() != null)
            {
                NotifyBarracudas(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") || other.GetComponent<PlayerMovement>() != null)
            {
                NotifyBarracudas(false);
            }
        }

        private void NotifyBarracudas(bool isHiding)
        {
            var barracudas = Object.FindObjectsByType<BarracudaAI>(FindObjectsSortMode.None);
            foreach (var b in barracudas)
            {
                if (b != null)
                {
                    b.SetPlayerHiding(isHiding);
                }
            }
        }
    }
}
