using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class FlyingAgent : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform[] groundRoute;
    private Transform[] airRoute;

    [Header("Bonuses (+10 Points)")]
    public float hoverSpeed = 2f;
    public float hoverHeight = 0.5f;
    public float linkTransitionDuration = 2f;

    private float defaultBaseOffset;
    private bool isOnAirRoute = false;
    private int currentTargetIndex = 0;
    private bool isTraversingLink = false;

    public void Initialize(Transform[] assignedGroundRoute, Transform[] assignedAirRoute)
    {
        agent = GetComponent<NavMeshAgent>();
        groundRoute = assignedGroundRoute;
        airRoute = assignedAirRoute;

        defaultBaseOffset = agent.baseOffset;
        agent.autoTraverseOffMeshLink = false;

        if (groundRoute != null && groundRoute.Length > 0)
        {

            float closestDistance = Mathf.Infinity;
            int closestIndex = 0;

            for (int i = 0; i < groundRoute.Length; i++)
            {
                float distanceToWaypoint = Vector3.Distance(transform.position, groundRoute[i].position);
                if (distanceToWaypoint < closestDistance)
                {
                    closestDistance = distanceToWaypoint;
                    closestIndex = i;
                }
            }

            currentTargetIndex = closestIndex;
            agent.SetDestination(groundRoute[currentTargetIndex].position);
        }
    }

    private void Update()
    {
        if (agent == null) return;


        agent.baseOffset = defaultBaseOffset + (Mathf.Sin(Time.time * hoverSpeed) * hoverHeight);


        if (!isTraversingLink && !agent.pathPending && agent.remainingDistance < 0.5f)
        {
            Transform[] currentRoute = isOnAirRoute ? airRoute : groundRoute;
            currentTargetIndex++;

            if (currentTargetIndex >= currentRoute.Length)
            {
                isOnAirRoute = !isOnAirRoute;
                currentTargetIndex = 0;
                currentRoute = isOnAirRoute ? airRoute : groundRoute;
            }

            if (currentRoute.Length > 0)
            {
                agent.SetDestination(currentRoute[currentTargetIndex].position);
            }
        }

        if (agent.isOnNavMesh && agent.isOnOffMeshLink && !isTraversingLink)
        {
            StartCoroutine(TraverseLink());
        }
    }

    private IEnumerator TraverseLink()
    {
        isTraversingLink = true;
        OffMeshLinkData data = agent.currentOffMeshLinkData;

        Vector3 startPos = agent.transform.position;
        Vector3 endPos = data.endPos;
        float time = 0f;

        while (time < 1f)
        {
            time += Time.deltaTime / linkTransitionDuration;
            agent.transform.position = Vector3.Lerp(startPos, endPos, time);
            yield return null;
        }

        agent.CompleteOffMeshLink();
        isTraversingLink = false;
    }
}