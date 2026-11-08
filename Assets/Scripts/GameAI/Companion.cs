using System;
using UnityEngine;
using UnityEngine.AI;

namespace UponHeavyTides.AI
{
    public class Companion : MonoBehaviour
    {
        public enum CompanionMode
        {
            WaitingForPlayer,   // Stays put until found and spoken to
            LeadingToTarget,    // Leads the player to a destination (e.g. Elder Turtle)
            FollowingPlayer     // Follows player everywhere
        }

        [Header("State")]
        [SerializeField] private CompanionMode currentMode = CompanionMode.WaitingForPlayer;
        [SerializeField] private Transform target;
        [SerializeField] private float followDistance = 2.5f;
        [SerializeField] private float moveSpeed = 3.8f;
        [SerializeField] private float updateInterval = 0.2f;

        [Header("Leading Destination")]
        [SerializeField] private Vector3 leadDestination;
        private Action onArrivedAtDestination;

        private NavMeshAgent agent;
        private float nextUpdateTime = 0f;

        public CompanionMode CurrentMode => currentMode;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.stoppingDistance = followDistance;
                agent.speed = moveSpeed;
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

        public void SetWaiting(bool isWaiting = true)
        {
            currentMode = isWaiting ? CompanionMode.WaitingForPlayer : CompanionMode.FollowingPlayer;
            if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
            {
                agent.ResetPath();
            }
        }

        public void LeadTowards(Vector3 destination, Action onArrive = null)
        {
            leadDestination = destination;
            onArrivedAtDestination = onArrive;
            currentMode = CompanionMode.LeadingToTarget;
        }

        public void FollowPlayer()
        {
            currentMode = CompanionMode.FollowingPlayer;
            if (target == null)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) target = player.transform;
            }
        }

        private void Update()
        {
            if (currentMode == CompanionMode.WaitingForPlayer) return;

            if (Time.time < nextUpdateTime) return;
            nextUpdateTime = Time.time + updateInterval;

            if (currentMode == CompanionMode.LeadingToTarget)
            {
                MoveTowardsPoint(leadDestination);
                float distToDest = Vector3.Distance(transform.position, leadDestination);
                if (distToDest <= 3.0f)
                {
                    var callback = onArrivedAtDestination;
                    onArrivedAtDestination = null;
                    callback?.Invoke();
                }
            }
            else if (currentMode == CompanionMode.FollowingPlayer)
            {
                if (target == null)
                {
                    var player = GameObject.FindGameObjectWithTag("Player");
                    if (player != null) target = player.transform;
                    return;
                }

                float distToPlayer = Vector3.Distance(transform.position, target.position);
                if (distToPlayer > followDistance)
                {
                    MoveTowardsPoint(target.position);
                }
            }
        }

        private void MoveTowardsPoint(Vector3 point)
        {
            if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
            {
                agent.SetDestination(point);
            }
            else
            {
                // Fallback smooth steering when navmesh is not loaded
                Vector3 dir = (point - transform.position);
                dir.y = 0;
                if (dir.sqrMagnitude > 0.5f)
                {
                    dir.Normalize();
                    transform.position += dir * (moveSpeed * Time.deltaTime * 3.5f);
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 6f * Time.deltaTime * 3.5f);
                }
            }
        }
    }
}
