using UnityEngine;
using System.Collections;

public class GroundManager : MonoBehaviour
{
    [SerializeField] private GameObject groundAgentPrefab;
    [SerializeField] private int agentCount = 100;

    [Tooltip("Drag your GroundWaypoints here")]
    [SerializeField] private Transform[] groundRoute;
    [SerializeField] private Transform spawnPoint;

    private void Start()
    {
        StartCoroutine(SpawnAgents());
    }

    private IEnumerator SpawnAgents()
    {
        for (int i = 0; i < agentCount; i++)
        {
            // 1. Spawn the agent
            GameObject newAgent = Instantiate(groundAgentPrefab, spawnPoint.position, Quaternion.identity);

            // 2. Hand the route data to the GroundAgent script
            newAgent.GetComponent<GroundAgent>().Initialize(groundRoute);

            // 3. Wait a moment before spawning the next one
            yield return new WaitForSeconds(0.1f);
        }
    }
}