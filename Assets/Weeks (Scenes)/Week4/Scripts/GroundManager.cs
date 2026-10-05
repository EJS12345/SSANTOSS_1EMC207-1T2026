using UnityEngine;
using System.Collections;

public class GroundManager : MonoBehaviour
{
    [SerializeField] private GameObject groundAgentPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private int agentCount = 100;
    [Tooltip("Time in seconds to wait between spawning each ground agent")]
    [SerializeField] private float spawnDelay = 0.2f;

    [Header("Routes")]
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

            GameObject newAgent = Instantiate(groundAgentPrefab, spawnPoint.position, Quaternion.identity);

            if (newAgent.TryGetComponent<GroundAgent>(out GroundAgent agentScript))
            {
                agentScript.Initialize(groundRoute);
            }

            yield return new WaitForSeconds(spawnDelay);
        }
    }
}