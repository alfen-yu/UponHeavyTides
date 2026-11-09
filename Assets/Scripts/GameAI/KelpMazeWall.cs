using UnityEngine;
using UnityEngine.AI;
using UponHeavyTides.Player;

namespace UponHeavyTides.AI
{
    
    /// Prevents Barracudas from entering
    
    public class KelpMazeWall : MonoBehaviour
    {
        private BoxCollider solidCollider;
        private BoxCollider triggerCollider;
        private NavMeshObstacle navObstacle;

        private void Awake()
        {
            SetupCollidersAndObstacle();
        }

        private void Start()
        {
            IgnorePlayerCollision();
        }

        public void SetupCollidersAndObstacle()
        {
            // Solid Collider 
            solidCollider = GetComponent<BoxCollider>();
            if (solidCollider == null)
            {
                solidCollider = gameObject.AddComponent<BoxCollider>();
            }
            solidCollider.isTrigger = false;

            // Trigger Collider for Stealth Detection
            var existingTriggers = GetComponents<BoxCollider>();
            foreach (var col in existingTriggers)
            {
                if (col.isTrigger)
                {
                    triggerCollider = col;
                    break;
                }
            }

            if (triggerCollider == null)
            {
                triggerCollider = gameObject.AddComponent<BoxCollider>();
                triggerCollider.isTrigger = true;
                triggerCollider.size = solidCollider.size + new Vector3(0.4f, 0f, 0.4f);
                triggerCollider.center = solidCollider.center;
            }

            // NavMeshObstacle ensures Barracuda NavMeshAgents cannot path through kelp
            navObstacle = GetComponent<NavMeshObstacle>();
            if (navObstacle == null)
            {
                navObstacle = gameObject.AddComponent<NavMeshObstacle>();
            }
            navObstacle.carving = true;
            navObstacle.size = solidCollider.size;
            navObstacle.center = solidCollider.center;
        }

        public void IgnorePlayerCollision()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                var charController = player.GetComponent<CharacterController>();
                if (charController != null && solidCollider != null)
                {
                    Physics.IgnoreCollision(charController, solidCollider, true);
                }

                var playerColliders = player.GetComponentsInChildren<Collider>();
                foreach (var col in playerColliders)
                {
                    if (solidCollider != null && col != solidCollider && col != triggerCollider)
                    {
                        Physics.IgnoreCollision(col, solidCollider, true);
                    }
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") || other.GetComponent<PlayerMovement>() != null)
            {
                SetBarracudasPlayerHiding(true);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player") || other.GetComponent<PlayerMovement>() != null)
            {
                SetBarracudasPlayerHiding(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") || other.GetComponent<PlayerMovement>() != null)
            {
                SetBarracudasPlayerHiding(false);
            }
        }

        private void SetBarracudasPlayerHiding(bool hiding)
        {
            var barracudas = Object.FindObjectsByType<BarracudaAI>(FindObjectsSortMode.None);
            foreach (var b in barracudas)
            {
                if (b != null)
                {
                    b.SetPlayerHiding(hiding);
                }
            }
        }
    }
}
