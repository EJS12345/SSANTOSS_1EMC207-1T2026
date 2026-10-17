using UnityEngine;
using UnityEngine.AI;

public class EnemyFSM : MonoBehaviour
{
    public enum State { Patrol, Alert, Chase }
    public State currentState = State.Patrol;

    [Header("References")]
    public NavMeshAgent agent;
    public Transform player;

    [Header("Patrol Settings")]
    public Transform[] waypoints;
    private int currentWaypointIndex = 0;

    [Header("Vision (Visual Detection Method 2)")]
    public float viewRadius = 10f;
    [Range(0, 360)] public float viewAngle = 90f;
    public LayerMask obstacleMask; // Set this to "Obstacles" in inspector

    [Header("Alert & Chase Settings")]
    public float investigateWaitTime = 3f;
    private float waitTimer = 0f;
    private Vector3 targetInvestigatePos;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (waypoints.Length > 0) agent.SetDestination(waypoints[0].position);
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Patrol:
                UpdatePatrol();
                break;
            case State.Alert:
                UpdateAlert();
                break;
            case State.Chase:
                UpdateChase();
                break;
        }
    }

    void UpdatePatrol()
    {
        // Cycle through route[cite: 114]
        if (!agent.pathPending && agent.remainingDistance < 0.5f && waypoints.Length > 0)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }

        // If player is within FOV, and there are no obstructions, go straight to chase[cite: 114]
        if (CanSeePlayer())
        {
            currentState = State.Chase;
        }
    }

    void UpdateAlert()
    {
        // AI investigates location where a sound was last done[cite: 114]
        agent.SetDestination(targetInvestigatePos);

        // If a player is within FOV while AI is investigating, head straight to chase[cite: 114]
        if (CanSeePlayer())
        {
            currentState = State.Chase;
            return;
        }

        // Wait at the spot for a few seconds, then go back to patrol
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= investigateWaitTime)
            {
                currentState = State.Patrol;
                agent.SetDestination(waypoints[currentWaypointIndex].position);
            }
        }
    }

    void UpdateChase()
    {
        agent.SetDestination(player.position);

        // If player goes out of vision, go back to alert state (investigate last known pos)[cite: 114]
        if (!CanSeePlayer())
        {
            currentState = State.Alert;
            targetInvestigatePos = player.position;
            waitTimer = 0f;
        }
    }

    // Called by the PlayerStealth script when making a noise
    public void HearSound(Vector3 soundPos)
    {
        // If a player makes a sound near AI, go to alert state[cite: 114]
        if (currentState == State.Patrol)
        {
            currentState = State.Alert;
            targetInvestigatePos = soundPos;
            waitTimer = 0f;
        }
        else if (currentState == State.Alert)
        {
            targetInvestigatePos = soundPos; // Update location if hearing more sounds
        }
    }

    // Visual FOV Detection logic using Vector3.Angle and a Raycast[cite: 107, 112]
    bool CanSeePlayer()
    {
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float dstToPlayer = Vector3.Distance(transform.position, player.position);

        // Check if player is near enough[cite: 103]
        if (dstToPlayer <= viewRadius)
        {
            // Check if player is within the angle in front of AI[cite: 103, 107]
            if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2)
            {
                // Raycast to check if vision is being obstructed[cite: 107, 112]
                // We lift the raycast slightly up (Vector3.up) so it shoots from the chest, not the feet.
                if (!Physics.Raycast(transform.position + Vector3.up, dirToPlayer, dstToPlayer, obstacleMask))
                {
                    return true;
                }
            }
        }
        return false;
    }
}