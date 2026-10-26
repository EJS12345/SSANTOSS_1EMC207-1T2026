using UnityEngine;
using UnityEngine.AI;

public class PlayerController2 : MonoBehaviour
{
    public NavMeshAgent agent;

    [Header("Animation")]
    public Animator animator; // NEW: Drag your Animator here

    [Header("Movement Settings")]
    public float moveSpeed = 5.5f;

    [Header("Stealth Mechanics")]
    public float currentNoiseLevel = 0f;
    public float movingNoise = 8f;

    [Header("Controls")]
    public LayerMask clickableLayers = ~0;

    void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (agent != null)
        {
            agent.speed = moveSpeed;
        }

        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, clickableLayers))
            {
                agent.SetDestination(hit.point);
            }
        }

        // Check if the agent is moving faster than 0.1 units
        if (agent != null && agent.velocity.magnitude > 0.1f)
        {
            currentNoiseLevel = movingNoise;

            // NEW: Play the run animation
            if (animator != null) animator.Play("Slow Run");
        }
        else
        {
            currentNoiseLevel = 0f;

            // NEW: Stop running (return to Idle if you have an Idle animation setup)
            // If you don't have an Idle state in your Animator Controller yet, you can leave this alone for now.
        }
    }
}