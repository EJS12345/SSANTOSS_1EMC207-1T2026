using UnityEngine;
using UnityEngine.AI;

public class SneekerAI : MonoBehaviour
{
    public enum SneekerState { Wander, Flee }
    public SneekerState currentState = SneekerState.Wander;

    [Header("References")]
    public NavMeshAgent agent;
    public Transform guard;

    [Header("Wander Settings")]
    public float wanderRadius = 10f;
    public float wanderTimer = 3f;
    private float timer;

    [Header("Flee Settings")]
    public float fleeDistance = 7f; // Distance at which Sneeker gets scared and runs

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;
    }

    void Update()
    {
        // 1. Check distance to the Guard
        float distanceToGuard = Vector3.Distance(transform.position, guard.position);

        // 2. Decide which state to be in
        if (distanceToGuard < fleeDistance)
        {
            currentState = SneekerState.Flee;
        }
        else
        {
            currentState = SneekerState.Wander;
        }

        // 3. Execute the current state
        switch (currentState)
        {
            case SneekerState.Wander:
                UpdateWander();
                break;
            case SneekerState.Flee:
                UpdateFlee();
                break;
        }
    }

    void UpdateWander()
    {
        timer += Time.deltaTime;

        // Pick a new random location on the NavMesh every few seconds
        if (timer >= wanderTimer)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.SetDestination(newPos);
            timer = 0;
        }
    }

    void UpdateFlee()
    {
        // Calculate the direction away from the guard
        Vector3 fleeDirection = transform.position - guard.position;

        // Project that direction outwards to find a safe spot
        Vector3 safePosition = transform.position + fleeDirection.normalized * 5f;

        // Ensure the safe spot is actually on the NavMesh before running there
        if (NavMesh.SamplePosition(safePosition, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    // Helper method to find a valid random point on the baked NavMesh
    private Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;

        NavMesh.SamplePosition(randDirection, out NavMeshHit navHit, dist, layermask);
        return navHit.position;
    }
}