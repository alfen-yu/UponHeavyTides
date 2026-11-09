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
        [SerializeField] private float patrolSpeed = 3.2f;
        [SerializeField] private float chaseSpeed = 5.2f;
        [SerializeField] private float waypointReachThreshold = 2.0f;

        [Header("Detection Settings")]
        [SerializeField] private float visionRange = 13.5f;
        [SerializeField] private float visionAngle = 85f;
        [SerializeField] private float loseSightDuration = 3.5f;

        [Header("Obstacle Avoidance Whiskers")]
        [SerializeField] private float whiskerLength = 2.8f;
        [SerializeField] private float sideWhiskerLength = 2.0f;
        [SerializeField] private float whiskerRepulsionForce = 4.5f;

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

        private Vector3 lastTrackedPos;
        private float stuckTimer = 0f;

        public AIState CurrentState => currentState;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.speed = patrolSpeed;
                agent.radius = 0.45f;
                agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
                agent.avoidancePriority = Random.Range(30, 60);
                agent.autoRepath = true;
            }
        }

        private void Start()
        {
            FindPlayer();

            if (agent != null && agent.isActiveAndEnabled && !agent.isOnNavMesh)
            {
                if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 6.0f, NavMesh.AllAreas))
                {
                    agent.Warp(hit.position);
                }
            }

            lastTrackedPos = transform.position;
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
                    CheckForStuck();
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

            // Always run raycast whiskers so barracudas steer smoothly away from kelp walls
            ApplyWallRepulsionWhiskers();
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
                stuckTimer = 0f;
            }
        }

        private void CheckForStuck()
        {
            float distMoved = Vector3.Distance(transform.position, lastTrackedPos);
            if (distMoved < 0.25f)
            {
                stuckTimer += Time.deltaTime;
                if (stuckTimer >= 1.5f)
                {
                    stuckTimer = 0f;
                    if (patrolWaypoints != null && patrolWaypoints.Count > 0)
                    {
                        currentWaypointIndex = (currentWaypointIndex + 1) % patrolWaypoints.Count;
                        if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
                        {
                            agent.ResetPath();
                            agent.SetDestination(patrolWaypoints[currentWaypointIndex].position);
                        }
                    }
                }
            }
            else
            {
                stuckTimer = 0f;
                lastTrackedPos = transform.position;
            }
        }

        private void CheckForPlayerDetection()
        {
            if (playerTarget == null || isPlayerHiding) return;

            Vector3 origin = transform.position + Vector3.up * 0.5f;
            Vector3 target = playerTarget.position + Vector3.up * 0.5f;
            Vector3 dirToPlayer = (target - origin);
            float dist = dirToPlayer.magnitude;

            // Proximity detection
            if (dist <= 2.2f || (dist <= 3.8f && Vector3.Angle(transform.forward, dirToPlayer.normalized) <= 110f))
            {
                if (!Physics.Linecast(origin, target, out RaycastHit hit, ~0, QueryTriggerInteraction.Ignore) 
                    || hit.collider.CompareTag("Player") 
                    || hit.collider.transform == playerTarget)
                {
                    investigatePosition = playerTarget.position;
                    SetState(AIState.Chase);
                    return;
                }
            }

            // Vision Cone detection
            if (dist <= visionRange)
            {
                float angle = Vector3.Angle(transform.forward, dirToPlayer.normalized);
                if (angle <= visionAngle * 0.5f)
                {
                    // Line of sight
                    if (!Physics.Linecast(origin, target, out RaycastHit hit, ~0, QueryTriggerInteraction.Ignore) 
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

            // Catch player
            if (dist <= 2.2f)
            {
                CatchPlayer();
                return;
            }

            // Check if player broke line of sight
            bool hasLineOfSight = false;
            if (dist <= visionRange * 1.3f)
            {
                Vector3 origin = transform.position + Vector3.up * 0.5f;
                Vector3 target = playerTarget.position + Vector3.up * 0.5f;

                if (!Physics.Linecast(origin, target, out RaycastHit hit, ~0, QueryTriggerInteraction.Ignore)
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

        private void ApplyWallRepulsionWhiskers()
        {
            Vector3 origin = transform.position + Vector3.up * 0.5f;
            Vector3 forward = transform.forward;
            Vector3 right = transform.right;

            // Whiskers
            Vector3[] whiskerDirs = new Vector3[]
            {
                (forward - right * 0.65f).normalized,
                (forward + right * 0.65f).normalized,
                (-right).normalized,
                (right).normalized
            };

            float[] lengths = new float[] { whiskerLength, whiskerLength, sideWhiskerLength, sideWhiskerLength };
            Vector3 repulsion = Vector3.zero;

            for (int i = 0; i < whiskerDirs.Length; i++)
            {
                if (Physics.Raycast(origin, whiskerDirs[i], out RaycastHit hit, lengths[i], ~0, QueryTriggerInteraction.Ignore))
                {
                    if (!hit.collider.CompareTag("Player") && (playerTarget == null || hit.collider.transform != playerTarget))
                    {
                        float factor = 1.0f - (hit.distance / lengths[i]);
                        repulsion += hit.normal * (factor * whiskerRepulsionForce);
                    }
                }
            }

            if (repulsion.sqrMagnitude > 0.05f)
            {
                repulsion.y = 0f;
                if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
                {
                    agent.Move(repulsion * Time.deltaTime);
                }
                else
                {
                    transform.position += repulsion * Time.deltaTime;
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player") || collision.gameObject.GetComponent<UponHeavyTides.Player.PlayerMovement>() != null)
            {
                CatchPlayer();
            }
        }

        public void CatchPlayer()
        {
            SetState(AIState.Patrol);

            if (UponHeavyTides.Level1.Level1Controller.Instance != null)
            {
                UponHeavyTides.Level1.Level1Controller.Instance.OnPlayerCaughtByBarracuda();
            }
        }

        public void ResetToPatrol()
        {
            SetState(AIState.Patrol);
            if (patrolWaypoints != null && patrolWaypoints.Count > 0)
            {
                currentWaypointIndex = 0;
                Vector3 targetPos = patrolWaypoints[0].position;
                if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 5.0f, NavMesh.AllAreas))
                {
                    targetPos = hit.position;
                }
                transform.position = targetPos;
                if (agent != null && agent.isActiveAndEnabled)
                {
                    agent.Warp(targetPos);
                    agent.ResetPath();
                }
            }
        }
    }
}
