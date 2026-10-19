using UnityEngine;
using UnityEngine.AI;

public class EnemyFSM : MonoBehaviour
{
    public enum State { Patrol, Alert, Chase }
    public State currentState = State.Patrol;

    [Header("References")]
    public NavMeshAgent agent;
    public SneekerAI player;
    public Renderer guardRenderer;
    public Renderer visionConeRenderer;

    [Header("Colors")]
    public Material greenPatrolMat;
    public Material yellowAlertMat;
    public Material redChaseMat;

    [Header("Patrol Settings")]
    public Transform[] waypoints;
    private int currentWaypointIndex = 0;

    [Header("Sensory Settings (Method 2)")]
    public float visionRange = 12f;
    public float fieldOfViewAngle = 90f;
    public float catchDistance = 2.0f;
    public LayerMask obstacleMask;

    [Header("Timers")]
    public float investigateTime = 3f;
    private float alertTimer = 0f;

    private Vector3 investigateTarget;

    void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();

        ChangeColor(greenPatrolMat);

        if (waypoints != null && waypoints.Length > 0)
        {
            agent.SetDestination(waypoints[0].position);
        }
    }

    void Update()
    {
        bool canSeePlayer = CheckVisualDetection();
        bool canHearPlayer = CheckAuditoryDetection();

        switch (currentState)
        {
            case State.Patrol:
                if (canSeePlayer)
                {
                    SwitchState(State.Chase);
                }
                else if (canHearPlayer)
                {
                    investigateTarget = player.transform.position;
                    SwitchState(State.Alert);
                }
                else
                {
                    UpdatePatrol();
                }
                break;

            case State.Alert:
                if (canSeePlayer)
                {
                    SwitchState(State.Chase);
                }
                else
                {
                    UpdateAlert();
                }
                break;

            case State.Chase:
                if (!canSeePlayer)
                {
                    investigateTarget = player.transform.position;
                    SwitchState(State.Alert);
                }
                else
                {
                    UpdateChase();
                }
                break;
        }
    }

    void SwitchState(State newState)
    {
        currentState = newState;

        if (agent.isActiveAndEnabled) agent.isStopped = false;

        if (newState == State.Patrol)
        {
            ChangeColor(greenPatrolMat);
            if (waypoints != null && waypoints.Length > 0)
            {
                agent.SetDestination(waypoints[currentWaypointIndex].position);
            }
        }
        else if (newState == State.Alert)
        {
            ChangeColor(yellowAlertMat);
            agent.SetDestination(investigateTarget);
            alertTimer = 0f;
        }
        else if (newState == State.Chase)
        {
            ChangeColor(redChaseMat);
        }
    }

    void UpdatePatrol()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.5f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    void UpdateAlert()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.5f)
        {
            alertTimer += Time.deltaTime;
            if (alertTimer >= investigateTime)
            {
                SwitchState(State.Patrol);
            }
        }
    }

    void UpdateChase()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            if (distanceToPlayer <= catchDistance)
            {
                agent.isStopped = true;
                if (player.agent != null) player.agent.isStopped = true;
            }
            else
            {
                agent.isStopped = false;
                agent.SetDestination(player.transform.position);
            }
        }
    }

    bool CheckVisualDetection()
    {
        if (player == null) return false;

        Vector3 origin = transform.position + Vector3.up * 1.0f;
        Vector3 target = player.transform.position + Vector3.up * 0.5f;
        Vector3 directionToPlayer = target - origin;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer <= catchDistance) return true;

        if (distanceToPlayer <= visionRange)
        {
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
            if (angleToPlayer <= fieldOfViewAngle / 2f)
            {
                if (!Physics.Raycast(origin, directionToPlayer.normalized, distanceToPlayer, obstacleMask))
                {
                    return true;
                }
            }
        }
        return false;
    }

    bool CheckAuditoryDetection()
    {
        if (player == null || player.currentNoiseLevel <= 0) return false;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        return distanceToPlayer <= player.currentNoiseLevel;
    }

    void ChangeColor(Material newMat)
    {
        if (newMat == null) return;

        if (guardRenderer != null) guardRenderer.material = newMat;
        if (visionConeRenderer != null) visionConeRenderer.material = newMat;
    }
}