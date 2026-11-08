using UnityEngine;
using UnityEngine.AI;

namespace UponHeavyTides.AI
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Companion : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float followDistance = 3f;
        [SerializeField] private float updateInterval = 0.2f;

        private NavMeshAgent agent;
        private float nextUpdateTime = 0f;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.stoppingDistance = followDistance;
            }
        }

        private void Start()
        {
            if (target == null)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    target = player.transform;
                }
            }
        }

        private void Update()
        {
            if (target == null) return;
            if (Time.time < nextUpdateTime) return;
            nextUpdateTime = Time.time + updateInterval;

            if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
            {
                float dist = Vector3.Distance(transform.position, target.position);
                if (dist > followDistance)
                {
                    agent.SetDestination(target.position);
                }
            }
            else
            {
                // Smooth fallback if NavMesh is not baked in the scene
                float dist = Vector3.Distance(transform.position, target.position);
                if (dist > followDistance)
                {
                    Vector3 dir = (target.position - transform.position).normalized;
                    dir.y = 0;
                    transform.position += dir * (3f * Time.deltaTime);
                    if (dir.sqrMagnitude > 0.01f)
                    {
                        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 5f * Time.deltaTime);
                    }
                }
            }
        }
    }
}
