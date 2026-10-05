using UnityEngine;
using UnityEngine.AI;

public class GroundAgent : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform[] route;
    private int targetIndex = 0;

    public void Initialize(Transform[] assignedRoute)
    {
        agent = GetComponent<NavMeshAgent>();
        route = assignedRoute;

        if (route != null && route.Length > 0)
        {

            float closestDistance = Mathf.Infinity;
            int closestIndex = 0;

            for (int i = 0; i < route.Length; i++)
            {
                float distanceToWaypoint = Vector3.Distance(transform.position, route[i].position);
                if (distanceToWaypoint < closestDistance)
                {
                    closestDistance = distanceToWaypoint;
                    closestIndex = i;
                }
            }

            targetIndex = closestIndex;
            agent.SetDestination(route[targetIndex].position);
        }
    }

    private void Update()
    {
        if (agent != null && !agent.pathPending && agent.remainingDistance < 0.5f)
        {
            targetIndex = (targetIndex + 1) % route.Length;
            agent.SetDestination(route[targetIndex].position);
        }
    }
}