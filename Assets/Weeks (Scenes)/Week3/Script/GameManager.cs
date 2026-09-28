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
    [SerializeField] private float spawnRadius = 8f;

    [Header("Realtime Baking Bonus")]
    [SerializeField] private NavMeshSurface navMeshSurface;
    [SerializeField] private Transform[] doors; 
    [SerializeField] private float doorOpenHeight = 4f;
    [SerializeField] private float bakeInterval = 3f;
    [SerializeField] private float moveDuration = 0.5f;

    private Vector3[] doorClosedPositions;
    private bool doorsAreOpen = false;

    private void Start()
    {

        doorClosedPositions = new Vector3[doors.Length];
        for (int i = 0; i < doors.Length; i++)
        {
            doorClosedPositions[i] = doors[i].position;
        }

        StartCoroutine(InitializeGame());
    }

    private IEnumerator InitializeGame()
    {
        yield return new WaitForSeconds(1f);

        CrowdAgent[] manuallyPlacedAgents = FindObjectsByType<CrowdAgent>(FindObjectsSortMode.None);
        foreach (CrowdAgent agent in manuallyPlacedAgents)
        {
            agent.Initialize(islandTargets);
        }

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
        }

        StartCoroutine(RealtimeBakeRoutine());
    }

    private IEnumerator RealtimeBakeRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(bakeInterval);

            doorsAreOpen = !doorsAreOpen;
            float time = 0;

            while (time < 1f)
            {
                time += Time.deltaTime / moveDuration;
                for (int i = 0; i < doors.Length; i++)
                {
                    Vector3 targetPos = doorsAreOpen ?
                        doorClosedPositions[i] + new Vector3(0, doorOpenHeight, 0) :
                        doorClosedPositions[i];

                    doors[i].position = Vector3.Lerp(doors[i].position, targetPos, time);
                }
                yield return null;
            }

            if (navMeshSurface != null)
            {
                navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
            }
        }
    }
}