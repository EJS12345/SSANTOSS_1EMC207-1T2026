using UnityEngine;
using UnityEngine.AI;

public class PlayerStealth : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent agent;
    private Camera mainCamera;

    [Header("Movement Settings")]
    public float normalSpeed = 5f;
    public float stealthSpeed = 2.5f; // Moving in stealth is slower

    [Header("Sound Settings (Auditory Detection Method 1)")]
    public float soundRadius = 10f;
    public LayerMask enemyLayer; // Set this to "Enemy" in inspector

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        // 1. Point and Click Movement
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                agent.SetDestination(hit.point);
            }
        }

        // 2. Stealth Mode Toggle (Hold Shift)
        bool isStealth = Input.GetKey(KeyCode.LeftShift);
        agent.speed = isStealth ? stealthSpeed : normalSpeed;

        // 3. Emit Sound if moving normally
        // If moving at full speed, transmit sound that can alert AI[cite: 114]
        if (!isStealth && agent.velocity.magnitude > 0.1f)
        {
            MakeSound();
        }
    }

    void MakeSound()
    {
        // Use OverlapSphere on the player to simulate generating a sound and get AIs within range[cite: 112]
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, soundRadius, enemyLayer);
        foreach (Collider enemyObj in hitEnemies)
        {
            if (enemyObj.TryGetComponent<EnemyFSM>(out EnemyFSM enemyAI))
            {
                enemyAI.HearSound(transform.position);
            }
        }
    }

    // Draws a yellow sphere in the editor so you can see how far the sound travels
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, soundRadius);
    }
}