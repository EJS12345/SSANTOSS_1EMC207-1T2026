using UnityEngine;
using UnityEngine.AI;

public class EnemyFSM : MonoBehaviour
{
    // Define the limited states of the machine
    public enum State { Patrol, Alert, Attack }
    public State currentState = State.Patrol;

    [Header("References")]
    public NavMeshAgent agent;
    public Transform player;

    [Header("Patrol Settings")]
    public Transform[] waypoints;
    private int currentWaypointIndex = 0;

    [Header("Radii & Ranges")]
    public float alertRadius = 8f;
    public float attackRadius = 12f; // Slightly larger so it doesn't move rapidly

    [Header("Timers")]
    public float timeToAttack = 2f;
    public float timeToPatrol = 3f;

    private float alertTimer = 0f;
    private float calmTimer = 0f;
    private Vector3 lastKnownPosition;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (waypoints.Length > 0)
        {
            agent.SetDestination(waypoints[0].position);
        }
    }

    void Update()
    {
        // Only one state is active at any given time
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.Patrol:
                UpdatePatrol(distanceToPlayer);
                break;
            case State.Alert:
                UpdateAlert(distanceToPlayer);
                break;
            case State.Attack:
                UpdateAttack(distanceToPlayer);
                break;
        }
    }

    void UpdatePatrol(float distance)
    {
        // Cycle through points and loop back
        if (!agent.pathPending && agent.remainingDistance < 0.5f && waypoints.Length > 0)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }

        // Transition to alert if player gets near
        if (distance <= alertRadius)
        {
            currentState = State.Alert;
            agent.SetDestination(transform.position); // Stay in position
            alertTimer = 0f;
            calmTimer = 0f;
        }
    }

    void UpdateAlert(float distance)
    {
        if (distance <= alertRadius)
        {
            // Transition to attack if player stays near for a period of time
            alertTimer += Time.deltaTime;
            calmTimer = 0f;

            if (alertTimer >= timeToAttack)
            {
                currentState = State.Attack;
            }
        }
        else
        {
            // Transition back to patrol if player gets out of radius long enough
            calmTimer += Time.deltaTime;
            alertTimer = 0f;

            if (calmTimer >= timeToPatrol)
            {
                currentState = State.Patrol;
                agent.SetDestination(waypoints[currentWaypointIndex].position);
            }
        }
    }

    void UpdateAttack(float distance)
    {
        // Chase the player
        agent.SetDestination(player.position);
        lastKnownPosition = player.position;

        // If player gets out of range, go back to alert and move to last known position
        if (distance > attackRadius)
        {
            currentState = State.Alert;
            agent.SetDestination(lastKnownPosition);
            alertTimer = 0f;
            calmTimer = 0f;
        }
    }

    // Draws circles in the editor to visualize the ranges
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, alertRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}