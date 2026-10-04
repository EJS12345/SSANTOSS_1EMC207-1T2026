using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class FlyingAgent : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform[] groundRoute;
    [SerializeField] private Transform[] airRoute;

    [Header("Bonus")]
    [Tooltip("Speed of the hover bobbing effect")]
    [SerializeField] private float hoverSpeed = 2f;
    [Tooltip("Height of the hover bobbing effect")]
    [SerializeField] private float hoverHeight = 0.5f;
    [Tooltip("How smoothly the agent takes off and lands via Links")]
    [SerializeField] private float linkTransitionDuration = 2f;

    private float defaultBaseOffset;
    private bool isOnAirRoute = false;
    private int currentTargetIndex = 0;
    private bool isTraversingLink = false;

    private void Start()
    {
        defaultBaseOffset = agent.baseOffset;

        // Required for Bonus #2 (Smooth Link Traversal)
        agent.autoTraverseOffMeshLink = false;

        if (groundRoute.Length > 0)
        {
            agent.SetDestination(groundRoute[0].position);
        }
    }

    private void Update()
    {
        // Bonus 1: Modify vertical position using baseOffset (Hover Effect)[cite: 72]
        agent.baseOffset = defaultBaseOffset + (Mathf.Sin(Time.time * hoverSpeed) * hoverHeight);

        // Standard Waypoint Movement
        if (!isTraversingLink && !agent.pathPending && agent.remainingDistance < 0.5f)
        {
            Transform[] currentRoute = isOnAirRoute ? airRoute : groundRoute;
            currentTargetIndex++;

            // Switch routes when reaching the end of the current route[cite: 71]
            if (currentTargetIndex >= currentRoute.Length)
            {
                isOnAirRoute = !isOnAirRoute;
                currentTargetIndex = 0;
                currentRoute = isOnAirRoute ? airRoute : groundRoute;
            }

            agent.SetDestination(currentRoute[currentTargetIndex].position);
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

        // Bonus 2: Smooth over terrain (Gradual vertical transition)[cite: 71]
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