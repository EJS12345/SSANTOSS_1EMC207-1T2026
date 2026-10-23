using UnityEngine;

public class FinishZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Only trigger the win screen if they have collected all 5 items
            if (GameManager2.Instance.hasAllObjectives)
            {
                GameManager2.Instance.GameWon();
            }
            else
            {
                // Optional: Tells the player how many they are missing if they try to leave early
                int missingItems = GameManager2.Instance.requiredItems - GameManager2.Instance.collectedItems;
                Debug.Log("You cannot leave yet! You still need " + missingItems + " more items.");
            }
        }
    }
}