using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class CrowdAgent : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    private Transform[] islands;
    private int currentIslandIndex = -1;

    private float defaultSpeed;
    private int slowAreaMask;

    public void Initialize(Transform[] islandTargets)
    {
        islands = islandTargets;
        defaultSpeed = agent.speed;


        slowAreaMask = 1 << NavMesh.GetAreaFromName("SlowArea");

        StartCoroutine(AgentRoutine());
    }

    private IEnumerator AgentRoutine()
    {

        yield return new WaitUntil(() => agent.isOnNavMesh);

        PickNewRandomIsland();

        while (true)
        {
            yield return new WaitForSeconds(0.2f);

            CheckSurfaceSpeed();


            if (agent.isOnNavMesh && !agent.pathPending && agent.remainingDistance < 1f)
            {
                PickNewRandomIsland();
            }
        }
    }

    private void CheckSurfaceSpeed()
    {
        if (!agent.isOnNavMesh) return;

        if (agent.SamplePathPosition(NavMesh.AllAreas, 0.1f, out NavMeshHit navHit))
        {

            if ((navHit.mask & slowAreaMask) != 0)
            {
                agent.speed = defaultSpeed * 0.5f;
            }
            else
            {
                agent.speed = defaultSpeed;
            }
        }
    }

    private void PickNewRandomIsland()
    {
        if (!agent.isOnNavMesh) return;

        int newIndex = currentIslandIndex;

        while (newIndex == currentIslandIndex)
        {
            newIndex = Random.Range(0, islands.Length);
        }

        currentIslandIndex = newIndex;

        NavMeshPath path = new NavMeshPath();
        if (agent.CalculatePath(islands[currentIslandIndex].position, path))
        {
            agent.SetPath(path);
        }
    }
}