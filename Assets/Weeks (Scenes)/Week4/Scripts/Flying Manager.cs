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

        StartCoroutine(SpawnFlyingAgentsRoutine());
    }

    private IEnumerator SpawnFlyingAgentsRoutine()
    {
        for (int i = 0; i < agentCount; i++)
        {

            GameObject newAgent = Instantiate(flyingAgentPrefab, spawnPoint.position, Quaternion.identity);


            newAgent.GetComponent<FlyingAgent>().Initialize(groundRoute, airRoute);


            yield return new WaitForSeconds(spawnDelay);
        }
    }
}