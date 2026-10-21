using UnityEngine;
using UnityEngine.AI;

public class SneekerAI : MonoBehaviour
{
    public enum SneekerState { Wander, Flee }
    public SneekerState currentState = SneekerState.Wander;

    [Header("References")]
    public NavMeshAgent agent;
    public Transform guard;

    [Header("Movement & Stealth")]
    public float stealthSpeed = 2f;
    public float normalSpeed = 5.5f;
    public float soundRadius = 12f;
    public bool isStealthing = false;

    [HideInInspector] public float currentNoiseLevel = 0f;

    [Header("Wander Settings")]
    public float wanderRadius = 6f;
    public float wanderTimer = 5f;
    private float timer;

    [Header("Flee Settings")]
    public float fleeDistance = 7f;

    void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;
    }

    void Update()
    {
        if (guard == null || !agent.isActiveAndEnabled || agent.isStopped) return;

        float distanceToGuard = Vector3.Distance(transform.position, guard.position);

        if (distanceToGuard < fleeDistance)
        {
            currentState = SneekerState.Flee;
            isStealthing = false;
        }
        else
        {
            currentState = SneekerState.Wander;
            if (timer >= wanderTimer)
            {
                isStealthing = Random.value > 0.5f;
            }
        }

        if (isStealthing)
        {
            agent.speed = stealthSpeed;
            currentNoiseLevel = 0f;
        }
        else
        {
            agent.speed = normalSpeed;
            currentNoiseLevel = soundRadius;
        }

        if (currentState == SneekerState.Wander) UpdateWander();
        else if (currentState == SneekerState.Flee) UpdateFlee();
    }

    void UpdateWander()
    {
        timer += Time.deltaTime;
        if (timer >= wanderTimer)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.SetDestination(newPos);
            timer = 0;
        }
    }

    void UpdateFlee()
    {
        Vector3 fleeDirection = transform.position - guard.position;
        Vector3 safePosition = transform.position + fleeDirection.normalized * 5f;

        if (NavMesh.SamplePosition(safePosition, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    private Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMesh.SamplePosition(randDirection, out NavMeshHit navHit, dist, layermask);
        return navHit.position;
    }
}