using UnityEngine;
using UnityEngine.AI;

public class GroundAgent : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform[] route;
    private int targetIndex = 0;

    // The GameManager calls this when the agent spawns
    public void Initialize(Transform[] assignedRoute)
    {
        agent = GetComponent<NavMeshAgent>();
        route = assignedRoute;

        if (route != null && route.Length > 0)
        {
            agent.SetDestination(route[targetIndex].position);
        }
    }

    private void Update()
    {
        // Patrol in a loop
        if (agent != null && !agent.pathPending && agent.remainingDistance < 0.5f)
        {
            targetIndex = (targetIndex + 1) % route.Length;
            agent.SetDestination(route[targetIndex].position);
        }
    }
}