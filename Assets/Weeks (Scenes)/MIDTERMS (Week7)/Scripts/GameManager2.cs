using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager2 : MonoBehaviour
{
    public static GameManager2 Instance;

    [Header("Objective Settings")]
    public int collectedItems = 0;
    public int requiredItems = 5;
    public bool hasAllObjectives = false;

    [Header("UI")]
    public GameObject winPanel;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void CollectObjective()
    {
        collectedItems++;

        // Check if the player has reached the required amount
        if (collectedItems >= requiredItems)
        {
            hasAllObjectives = true;
            Debug.Log("All " + requiredItems + " objectives collected! Head to the exit!");
        }
        else
        {
            Debug.Log("Collected " + collectedItems + " / " + requiredItems + " items.");
        }
    }

    public void GameOver()
    {
        Debug.Log("Caught by the guard! Restarting...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GameWon()
    {
        Debug.Log("Escaped!");
        winPanel.SetActive(true);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}