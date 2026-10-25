using UnityEngine;
using UnityEngine.AI;

public class EnemyFSM : MonoBehaviour
{
    public enum State { Patrol, Alert, Chase }
    public State currentState = State.Patrol;

    [Header("References")]
    public NavMeshAgent agent;
    public PlayerController player;
    public Renderer guardRenderer;

    [Header("Colors")]
    public Material greenPatrolMat;
    public Material yellowAlertMat;
    public Material redChaseMat;

    [Header("Patrol Settings")]
    public Transform[] waypoints;
    private int currentWaypointIndex = 0;

    [Header("Sensory Settings")]
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
        if (waypoints.Length > 0) agent.SetDestination(waypoints[0].position);
    }

    void Update()
    {
        if (player == null) return;

        // --- BONUS LOGIC: Touch Detection ---
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= catchDistance)
        {
            // Check if the player has at least 1 power charge
            if (GameManager2.Instance.powerCharges > 0)
            {
                Debug.Log("Player used a power charge! Guard destroyed!");
                GameManager2.Instance.UsePower(); // Subtract 1 from the player's charges
                Destroy(gameObject);              // Destroy the Guard
                return;
            }
            else
            {
                // No charges left? The Player is caught.
                GameManager2.Instance.GameOver();
                return;
            }
        }

        // --- STATE MACHINE ---
        switch (currentState)
        {
            case State.Patrol: UpdatePatrol(); break;
            case State.Alert: UpdateAlert(); break;
            case State.Chase: UpdateChase(); break;
        }
    }

    void SwitchState(State newState)
    {
        currentState = newState;

        if (newState == State.Patrol)
        {
            ChangeColor(greenPatrolMat);
            if (waypoints.Length > 0) agent.SetDestination(waypoints[currentWaypointIndex].position);
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
        if (CheckVisualDetection()) { SwitchState(State.Chase); return; }
        if (CheckAuditoryDetection())
        {
            investigateTarget = player.transform.position;
            SwitchState(State.Alert);
            return;
        }

        if (!agent.pathPending && agent.remainingDistance < 0.5f && waypoints.Length > 0)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    void UpdateAlert()
    {
        if (CheckVisualDetection()) { SwitchState(State.Chase); return; }

        if (!agent.pathPending && agent.remainingDistance < 1f)
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
        if (!CheckVisualDetection())
        {
            investigateTarget = player.transform.position;
            SwitchState(State.Alert);
            return;
        }

        agent.SetDestination(player.transform.position);
    }

    bool CheckVisualDetection()
    {
        Vector3 origin = transform.position + Vector3.up * 1.0f;
        Vector3 target = player.transform.position + Vector3.up * 0.5f;
        Vector3 directionToPlayer = target - origin;
        float distance = directionToPlayer.magnitude;

        if (distance <= visionRange)
        {
            float angle = Vector3.Angle(transform.forward, directionToPlayer);
            if (angle <= fieldOfViewAngle / 2f)
            {
                if (!Physics.Raycast(origin, directionToPlayer.normalized, distance, obstacleMask))
                {
                    return true;
                }
            }
        }
        return false;
    }

    bool CheckAuditoryDetection()
    {
        if (player.currentNoiseLevel <= 0) return false;
        return Vector3.Distance(transform.position, player.transform.position) <= player.currentNoiseLevel;
    }

    void ChangeColor(Material newMat)
    {
        if (guardRenderer != null && newMat != null) guardRenderer.material = newMat;
    }
}