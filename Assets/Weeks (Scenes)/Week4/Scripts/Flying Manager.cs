using UnityEngine;
using System.Collections;

public class FlyingManager : MonoBehaviour
{
    [SerializeField] private GameObject flyingAgentPrefab;
    [SerializeField] private Transform spawnPoint;

    [Header("Spawn Settings")]
    [Tooltip("How many flying agents to spawn")]
    [SerializeField] private int agentCount = 1;
    [Tooltip("Time to wait between spawning each agent so they don't collide")]
    [SerializeField] private float spawnDelay = 0.5f;

    [Header("Routes")]
    [Tooltip("Drag the Ground points here")]
    [SerializeField] private Transform[] groundRoute;

    [Tooltip("Drag the PointFlying points here")]
    [SerializeField] private Transform[] airRoute;

    private void Start()
    {
        // Start the Coroutine to handle multiple spawns over time
        StartCoroutine(SpawnFlyingAgentsRoutine());
    }

    private IEnumerator SpawnFlyingAgentsRoutine()
    {
        for (int i = 0; i < agentCount; i++)
        {
            // 1. Spawn the flying agent
            GameObject newAgent = Instantiate(flyingAgentPrefab, spawnPoint.position, Quaternion.identity);

            // 2. Hand both route data arrays to the FlyingAgent script
            newAgent.GetComponent<FlyingAgent>().Initialize(groundRoute, airRoute);

            // 3. Wait briefly before spawning the next agent
            yield return new WaitForSeconds(spawnDelay);
        }
    }
}