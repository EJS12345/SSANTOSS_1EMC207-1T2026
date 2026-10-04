using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class FlyingAgent : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform[] groundRoute;
    private Transform[] airRoute;

    [Header("Info")]
    [Tooltip("Speed of the hover bobbing effect")]
    public float hoverSpeed = 2f;
    [Tooltip("Height of the hover bobbing effect")]
    public float hoverHeight = 0.5f;
    [Tooltip("How smoothly the agent takes off and lands via Links")]
    public float linkTransitionDuration = 2f;

    private float defaultBaseOffset;
    private bool isOnAirRoute = false;
    private int currentTargetIndex = 0;
    private bool isTraversingLink = false;

    // The FlyingManager calls this when the agent spawns
    public void Initialize(Transform[] assignedGroundRoute, Transform[] assignedAirRoute)
    {
        agent = GetComponent<NavMeshAgent>();
        groundRoute = assignedGroundRoute;
        airRoute = assignedAirRoute;

        defaultBaseOffset = agent.baseOffset;

        // Disable automatic link traversal so our custom smooth coroutine runs instead
        agent.autoTraverseOffMeshLink = false;

        // Start by heading to the first ground waypoint
        if (groundRoute != null && groundRoute.Length > 0)
        {
            agent.SetDestination(groundRoute[0].position);
        }
    }

    private void Update()
    {
        if (agent == null) return;

        // Bonus 1: Modify vertical position using baseOffset (Hover Effect)
        agent.baseOffset = defaultBaseOffset + (Mathf.Sin(Time.time * hoverSpeed) * hoverHeight);

        // Standard Waypoint Movement
        if (!isTraversingLink && !agent.pathPending && agent.remainingDistance < 0.5f)
        {
            Transform[] currentRoute = isOnAirRoute ? airRoute : groundRoute;
            currentTargetIndex++;

            // Switch routes when reaching the end of the current route
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

        // Intercept NavMesh Link for smooth takeoff/landing
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

        // Bonus 2: Smooth over terrain (Gradual vertical transition)
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