using UnityEngine;
using UnityEngine.AI;

namespace UponHeavyTides.AI
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Companion : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float followDistance = 3f;

        private NavMeshAgent agent;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.stoppingDistance = followDistance;

            if (target == null)
            {
                Debug.LogError("CompanionFollower has no target assigned.", this);
                enabled = false;
            }
        }

        private void Update()
        {
            agent.SetDestination(target.position);
        }
    }
}