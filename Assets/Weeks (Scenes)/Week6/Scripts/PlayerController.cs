using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public NavMeshAgent agent;

    [Header("Movement Settings")]
    [Tooltip("Adjusts how fast the player moves")]
    public float moveSpeed = 5.5f;

    [Header("Stealth Mechanics")]
    public float currentNoiseLevel = 0f;
    public float movingNoise = 8f; // How far sound travels when walking

    [Header("Controls")]
    [Tooltip("Check the layers the player is allowed to click on (e.g., Default or Ground)")]
    public LayerMask clickableLayers = ~0; // ~0 means "Everything" by default

    void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // 1. Apply the Inspector speed to the NavMeshAgent 
        // (Placed in Update so you can tweak the speed while testing in Play Mode!)
        if (agent != null)
        {
            agent.speed = moveSpeed;
        }

        // 2. Right-click to move
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Uses clickableLayers so your clicks don't fall through the floor
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, clickableLayers))
            {
                agent.SetDestination(hit.point);
            }
        }

        // 3. Generate noise only when moving
        if (agent != null && agent.velocity.magnitude > 0.1f)
        {
            currentNoiseLevel = movingNoise;
        }
        else
        {
            currentNoiseLevel = 0f;
        }
    }
}