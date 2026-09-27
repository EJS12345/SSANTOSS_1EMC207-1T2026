using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Agent Spawning")]
    [SerializeField] private CrowdAgent agentPrefab;
    [SerializeField] private int agentCount = 150;
    [SerializeField] private Transform[] islandTargets;

    [Tooltip("How far agents spread out when spawning")]
    [SerializeField] private float spawnRadius = 8f;

    [Header("Realtime Baking Bonus")]
    [SerializeField] private NavMeshSurface navMeshSurface;
    [SerializeField] private float bakeInterval = 3f;

    private void Start()
    {
        StartCoroutine(InitializeGame());
        StartCoroutine(RealtimeBakeRoutine());
    }

    private IEnumerator InitializeGame()
    {
        // Wait 1 second to guarantee the NavMesh is fully baked before agents try to move
        yield return new WaitForSeconds(1f);

        // --- NEW CODE: Find and initialize any manually placed agents in the Hierarchy ---
        CrowdAgent[] manuallyPlacedAgents = FindObjectsByType<CrowdAgent>(FindObjectsSortMode.None);
        foreach (CrowdAgent agent in manuallyPlacedAgents)
        {
            agent.Initialize(islandTargets);
        }
        // --------------------------------------------------------------------------------

        // Spawn the remaining required agents
        for (int i = 0; i < agentCount; i++)
        {
            Transform randomIsland = islandTargets[Random.Range(0, islandTargets.Length)];

            Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
            randomOffset.y = 0;
            Vector3 spawnPos = randomIsland.position + randomOffset;

            if (NavMesh.SamplePosition(spawnPos, out NavMeshHit hit, 10f, NavMesh.AllAreas))
            {
                CrowdAgent newAgent = Instantiate(agentPrefab, hit.position, Quaternion.identity);
                newAgent.Initialize(islandTargets);
            }
            else
            {
                Debug.LogWarning("Failed to find NavMesh at " + spawnPos);
            }
        }
    }

    private IEnumerator RealtimeBakeRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(bakeInterval);

            if (navMeshSurface != null)
            {
                navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
            }
        }
    }
}