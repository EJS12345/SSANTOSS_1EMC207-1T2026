using UnityEngine;

public class ObjectiveItem : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // Checks if the object touching it has the "Player" tag
        if (other.CompareTag("Player"))
        {
            GameManager2.Instance.CollectObjective();
            Destroy(gameObject);
        }
    }
}