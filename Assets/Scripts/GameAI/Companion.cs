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
        [SerializeField] private float moveSpeed = 4.2f;

        [Header("Leading Destination")]
        [SerializeField] private Vector3 leadDestination;
        private Action onArrivedAtDestination;

        private NavMeshAgent agent;
        private float pathRecalcTimer = 0f;

        public CompanionMode CurrentMode => currentMode;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.stoppingDistance = followDistance;
                agent.speed = moveSpeed;
                agent.acceleration = 12f;
            }
        }

        private void Start()
        {
            FindPlayer();

            if (agent != null && agent.isActiveAndEnabled && !agent.isOnNavMesh)
            {
                if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 5.0f, NavMesh.AllAreas))
                {
                    agent.Warp(hit.position);
                }
            }
        }

        private void FindPlayer()
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

            if (agent != null && agent.isActiveAndEnabled)
            {
                if (!agent.isOnNavMesh)
                {
                    if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 5.0f, NavMesh.AllAreas))
                    {
                        agent.Warp(hit.position);
                    }
                }

                if (agent.isOnNavMesh)
                {
                    agent.isStopped = false;
                    agent.stoppingDistance = 1.0f;
                    agent.SetDestination(destination);
                }
            }
        }

        public void FollowPlayer()
        {
            currentMode = CompanionMode.FollowingPlayer;
            FindPlayer();

            if (agent != null && agent.isActiveAndEnabled)
            {
                agent.stoppingDistance = followDistance;
                if (!agent.isOnNavMesh)
                {
                    if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 5.0f, NavMesh.AllAreas))
                    {
                        agent.Warp(hit.position);
                    }
                }

                if (agent.isOnNavMesh)
                {
                    agent.isStopped = false;
                }
            }
        }

        private void Update()
        {
            if (currentMode == CompanionMode.WaitingForPlayer) return;

            if (currentMode == CompanionMode.LeadingToTarget)
            {
                float distToDest = Vector3.Distance(transform.position, leadDestination);
                if (distToDest > 0.8f)
                {
                    MoveTowardsPoint(leadDestination);
                }
                else
                {
                    if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
                    {
                        agent.ResetPath();
                    }

                    var callback = onArrivedAtDestination;
                    onArrivedAtDestination = null;
                    callback?.Invoke();

                    // Face towards the Elder Sea Turtle
                    Vector3 lookDir = (new Vector3(0f, 0f, -9.5f) - transform.position);
                    lookDir.y = 0;
                    if (lookDir.sqrMagnitude > 0.01f)
                    {
                        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), 5f * Time.deltaTime);
                    }
                }
            }
            else if (currentMode == CompanionMode.FollowingPlayer)
            {
                if (target == null)
                {
                    FindPlayer();
                    if (target == null) return;
                }

                float distToPlayer = Vector3.Distance(transform.position, target.position);
                if (distToPlayer > followDistance)
                {
                    MoveTowardsPoint(target.position);
                }
                else if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
                {
                    agent.ResetPath();
                }
            }
        }

        private void MoveTowardsPoint(Vector3 point)
        {
            if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
            {
                pathRecalcTimer -= Time.deltaTime;
                if (pathRecalcTimer <= 0f)
                {
                    pathRecalcTimer = 0.25f;
                    agent.SetDestination(point);
                }
            }
            else
            {
                // Smooth frame-by-frame fallback movement if not on NavMesh
                Vector3 dir = (point - transform.position);
                dir.y = 0f;
                if (dir.sqrMagnitude > 0.05f)
                {
                    dir.Normalize();
                    transform.position += dir * (moveSpeed * Time.deltaTime);
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 8f * Time.deltaTime);
                }
            }
        }
    }
}
