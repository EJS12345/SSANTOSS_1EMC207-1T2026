using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager2 : MonoBehaviour
{
    public static GameManager2 Instance;

    [Header("Objective Settings")]
    public int collectedItems = 0;
    public int requiredItems = 5;
    public bool hasAllObjectives = false;

    [Header("Combat Settings")]
    public int powerCharges = 0; // Tracks how many enemies the player can currently kill

    [Header("UI References")]
    public GameObject winPanel;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI itemsText;
    public TextMeshProUGUI powerText; // Your new PowerLeft Text

    private float elapsedTime = 0f;
    private bool isGameActive = true;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        UpdateItemUI();
        UpdatePowerUI();
    }

    void Update()
    {
        if (isGameActive)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerUI();
        }
    }

    public void CollectObjective()
    {
        collectedItems++;
        powerCharges++; // Gain 1 kill charge per item picked up

        UpdateItemUI();
        UpdatePowerUI();

        if (collectedItems >= requiredItems)
        {
            hasAllObjectives = true;
            Debug.Log("All " + requiredItems + " objectives collected! Head to the exit!");
        }
    }

    // Function called by the Guard when the player touches them
    public void UsePower()
    {
        if (powerCharges > 0)
        {
            powerCharges--;
            UpdatePowerUI();
        }
    }

    private void UpdateItemUI()
    {
        if (itemsText != null) itemsText.text = "Items: " + collectedItems + " / " + requiredItems;
    }

    private void UpdatePowerUI()
    {
        if (powerText != null) powerText.text = "Power Left: " + powerCharges;
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(elapsedTime / 60F);
            int seconds = Mathf.FloorToInt(elapsedTime - minutes * 60);
            timerText.text = string.Format("Timer: {0:00}:{1:00}", minutes, seconds);
        }
    }

    public void GameOver()
    {
        isGameActive = false;
        Debug.Log("Caught by the guard! Restarting...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GameWon()
    {
        isGameActive = false;
        Debug.Log("Escaped!");
        winPanel.SetActive(true);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}