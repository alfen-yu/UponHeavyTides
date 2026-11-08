using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace UponHeavyTides.AI
{
    public class BarracudaAI : MonoBehaviour
    {
        public enum AIState
        {
            Patrol,
            Investigate,
            Chase,
            ReturnToPatrol
        }

        [Header("State")]
        [SerializeField] private AIState currentState = AIState.Patrol;

        [Header("Patrol Configuration")]
        [SerializeField] private List<Transform> patrolWaypoints = new List<Transform>();
        [SerializeField] private float patrolSpeed = 3.0f;
        [SerializeField] private float chaseSpeed = 5.2f;
        [SerializeField] private float waypointReachThreshold = 1.8f;

        [Header("Detection Settings")]
        [SerializeField] private float visionRange = 12f;
        [SerializeField] private float visionAngle = 70f;
        [SerializeField] private float loseSightDuration = 3.5f;

        [Header("Visual Feedback")]
        [SerializeField] private MeshRenderer visionConeRenderer;
        [SerializeField] private Color patrolColor = new Color(0.2f, 0.8f, 0.9f, 0.35f);
        [SerializeField] private Color investigateColor = new Color(1.0f, 0.85f, 0.2f, 0.5f);
        [SerializeField] private Color chaseColor = new Color(1.0f, 0.2f, 0.2f, 0.65f);

        private NavMeshAgent agent;
        private Transform playerTarget;
        private int currentWaypointIndex = 0;
        private Vector3 investigatePosition;
        private float timeLostSight = 0f;
        private bool isPlayerHiding = false;

        public AIState CurrentState => currentState;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.speed = patrolSpeed;
            }
        }

        private void Start()
        {
            FindPlayer();
            SetState(AIState.Patrol);
        }

        private void FindPlayer()
        {
            if (playerTarget == null)
            {
                var p = GameObject.FindGameObjectWithTag("Player");
                if (p != null) playerTarget = p.transform;
            }
        }

        public void SetWaypoints(List<Transform> waypoints)
        {
            patrolWaypoints = waypoints;
        }

        public void SetPlayerHiding(bool hiding)
        {
            isPlayerHiding = hiding;
            if (hiding && currentState == AIState.Chase)
            {
                SetState(AIState.Investigate);
            }
        }

        private void Update()
        {
            if (playerTarget == null) FindPlayer();

            // Run State Machine
            switch (currentState)
            {
                case AIState.Patrol:
                    UpdatePatrol();
                    CheckForPlayerDetection();
                    break;

                case AIState.Investigate:
                    UpdateInvestigate();
                    CheckForPlayerDetection();
                    break;

                case AIState.Chase:
                    UpdateChase();
                    break;

                case AIState.ReturnToPatrol:
                    UpdateReturn();
                    CheckForPlayerDetection();
                    break;
            }
        }

        private void SetState(AIState newState)
        {
            currentState = newState;

            if (visionConeRenderer != null && visionConeRenderer.material != null)
            {
                switch (newState)
                {
                    case AIState.Patrol:
                    case AIState.ReturnToPatrol:
                        visionConeRenderer.material.color = patrolColor;
                        break;
                    case AIState.Investigate:
                        visionConeRenderer.material.color = investigateColor;
                        break;
                    case AIState.Chase:
                        visionConeRenderer.material.color = chaseColor;
                        break;
                }
            }

            if (agent != null)
            {
                agent.speed = (newState == AIState.Chase) ? chaseSpeed : patrolSpeed;
            }
        }

        private void UpdatePatrol()
        {
            if (patrolWaypoints == null || patrolWaypoints.Count == 0) return;

            Transform targetWp = patrolWaypoints[currentWaypointIndex];
            if (targetWp == null) return;

            MoveToPosition(targetWp.position);

            float dist = Vector3.Distance(transform.position, targetWp.position);
            if (dist <= waypointReachThreshold)
            {
                currentWaypointIndex = (currentWaypointIndex + 1) % patrolWaypoints.Count;
            }
        }

        private void CheckForPlayerDetection()
        {
            if (playerTarget == null || isPlayerHiding) return;

            Vector3 dirToPlayer = (playerTarget.position - transform.position);
            float dist = dirToPlayer.magnitude;

            if (dist <= visionRange)
            {
                float angle = Vector3.Angle(transform.forward, dirToPlayer.normalized);
                if (angle <= visionAngle * 0.5f)
                {
                    // Line of sight check
                    if (!Physics.Linecast(transform.position + Vector3.up * 0.5f, playerTarget.position + Vector3.up * 0.5f, out RaycastHit hit) 
                        || hit.collider.CompareTag("Player") 
                        || hit.collider.transform == playerTarget)
                    {
                        investigatePosition = playerTarget.position;
                        SetState(AIState.Chase);
                    }
                }
            }
        }

        private void UpdateChase()
        {
            if (playerTarget == null || isPlayerHiding)
            {
                SetState(AIState.Investigate);
                return;
            }

            MoveToPosition(playerTarget.position);

            Vector3 dir = (playerTarget.position - transform.position);
            float dist = dir.magnitude;

            // Check if player broke line of sight
            bool hasLineOfSight = false;
            if (dist <= visionRange * 1.3f)
            {
                if (!Physics.Linecast(transform.position + Vector3.up * 0.5f, playerTarget.position + Vector3.up * 0.5f, out RaycastHit hit)
                    || hit.collider.CompareTag("Player")
                    || hit.collider.transform == playerTarget)
                {
                    hasLineOfSight = true;
                    investigatePosition = playerTarget.position;
                    timeLostSight = 0f;
                }
            }

            if (!hasLineOfSight)
            {
                timeLostSight += Time.deltaTime;
                if (timeLostSight >= loseSightDuration)
                {
                    SetState(AIState.Investigate);
                }
            }
        }

        private void UpdateInvestigate()
        {
            MoveToPosition(investigatePosition);
            float dist = Vector3.Distance(transform.position, investigatePosition);
            if (dist <= 2.2f)
            {
                SetState(AIState.ReturnToPatrol);
            }
        }

        private void UpdateReturn()
        {
            if (patrolWaypoints == null || patrolWaypoints.Count == 0) return;

            Transform targetWp = patrolWaypoints[currentWaypointIndex];
            MoveToPosition(targetWp.position);

            if (Vector3.Distance(transform.position, targetWp.position) <= waypointReachThreshold)
            {
                SetState(AIState.Patrol);
            }
        }

        private void MoveToPosition(Vector3 dest)
        {
            if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
            {
                agent.SetDestination(dest);
            }
            else
            {
                Vector3 dir = (dest - transform.position);
                dir.y = 0f;
                if (dir.sqrMagnitude > 0.05f)
                {
                    dir.Normalize();
                    float currentSpeed = (currentState == AIState.Chase) ? chaseSpeed : patrolSpeed;
                    transform.position += dir * (currentSpeed * Time.deltaTime);
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 5f * Time.deltaTime);
                }
            }
        }
    }
}
