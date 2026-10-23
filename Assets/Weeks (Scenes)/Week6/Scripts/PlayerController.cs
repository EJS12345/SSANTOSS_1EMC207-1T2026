using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public NavMeshAgent agent;

    [Header("Stealth Mechanics")]
    public float currentNoiseLevel = 0f;
    public float movingNoise = 8f; // How far sound travels when walking

    void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // Right-click to move
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                agent.SetDestination(hit.point);
            }
        }

        // Generate noise only when moving
        if (agent.velocity.magnitude > 0.1f)
        {
            currentNoiseLevel = movingNoise;
        }
        else
        {
            currentNoiseLevel = 0f;
        }
    }
}