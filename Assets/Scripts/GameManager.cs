using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class GameManager : MonoBehaviour
{
    // Singleton instance of the GameManager
    public static GameManager Instance { get; private set; }

    [Space(10)]
    // Animator component reference
    public Animator animator;

    [Space(10)]
    // UI elements for game start and game over
    public TextMeshProUGUI gameStartText;
    public TextMeshProUGUI gameOverText;
    public Button retryButton;
    public Button startButton;
    public Button jumpButton; // Reference to the jump button

    [Space(10)]
    // Current player score in the game
    public float score;

    private int finalScore = 0;

    [Space(10)]
    // UI elements for displaying score and high score
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI hiscoreText;

    // Reference to the PlayerController and the ObstacleGenerator
    private PlayerController player;
    private ObstacleGenerator spawner;

    [Space(10)]
    // Initial game speed and the rate at which it increases
    public float initialGameSpeed = 5f;
    public float gameSpeedIncrease = 0.1f;

    // Current game speed
    public float gameSpeed { get; private set; }

    [Space(10)]
    // Variables to track the game state
    public bool isGameStarted = false;
    public bool isGameOver = false;

    private bool hasFinalScoreProcessed = false;

    [DllImport("__Internal")]
  private static extern void SendScore(int score, int game);

    // Called when the script instance is being loaded
    private void Awake()
    {
        // Ensure only one instance of GameManager exists
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // If another instance already exists, destroy this one
            DestroyImmediate(gameObject);
        }
    }

    // Called when the MonoBehaviour will be destroyed
    private void OnDestroy()
    {
        // If this instance is the current GameManager instance, set it to null on destruction
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // Called when the script instance is being loaded
    private void Start()
    {
        // Find and store references to the PlayerController and ObstacleGenerator
        player = FindObjectOfType<PlayerController>();
        spawner = FindObjectOfType<ObstacleGenerator>();

        // Initialize the "isGameStarted" animator parameter to false.
        animator.SetBool("isGameStarted", false);

        // Set up the game speed increase to 0
        gameSpeedIncrease = 0;

        // Deactivate the obstacle spawner
        spawner.gameObject.SetActive(false);

        // Set initial UI states
        gameStartText.gameObject.SetActive(true);
        gameOverText.gameObject.SetActive(false);
        retryButton.gameObject.SetActive(false);
        jumpButton.gameObject.SetActive(false); // Initially, the jump button is hidden
    }

    // Initialize a new game
    public void NewGame()
    {
        // Set up the game speed increase to default
        gameSpeedIncrease = 0.1f;

        // Deactivate the player to reset animation
        player.gameObject.SetActive(false);

        // Find all existing ObstacleController instances and destroy them
        ObstacleController[] obstacles = FindObjectsOfType<ObstacleController>();
        foreach (var obstacle in obstacles)
        {
            Destroy(obstacle.gameObject);
        }

        // Set the initial game speed
        gameSpeed = initialGameSpeed;

        // Reset the score
        score = 0f;
        finalScore = 0;

        // Enable the GameManager script
        enabled = true;

        // Activate the player and obstacle spawner
        player.gameObject.SetActive(true);
        spawner.gameObject.SetActive(true);

        // Hide game over UI elements
        gameOverText.gameObject.SetActive(false);
        retryButton.gameObject.SetActive(false);

        // Show the jump button and hide the start button
        jumpButton.gameObject.SetActive(true);
        startButton.gameObject.SetActive(false);

        // Reset the game over and dead animation state
        isGameOver = false;
        hasFinalScoreProcessed = false;
        animator.SetBool("isGameStarted", true);
    }

    // Method to start the game (linked to Start button)
    public void StartGame()
    {
        if (!isGameStarted)
        {
            isGameStarted = true;
            gameStartText.gameObject.SetActive(false);
            NewGame();
        }
    }

    // Method to retry the game (linked to Retry button)
    public void RetryGame()
    {
        if (isGameOver)
        {
            NewGame();
        }
    }

    // Called every frame
    private void Update()
    {
        // Check if the game is not over to allow player input
        if (!isGameOver && isGameStarted)
        {
            // Increase the game speed over time
            gameSpeed += gameSpeedIncrease * Time.deltaTime;

            // Update the score based on the current game speed
            score += gameSpeed * Time.deltaTime;
            scoreText.text = Mathf.FloorToInt(score).ToString("D5");
        }
    }

    // Update and save the high score
    private void UpdateHiscore()
    {
        // Retrieve the current high score from player preferences
        float hiscore = PlayerPrefs.GetFloat("hiscore", 0);

        // Update the UI with the high score
        hiscoreText.text = Mathf.FloorToInt(hiscore).ToString("D5");

        // Check if the game is over before updating the high score
        if (isGameOver)
        {
            // If the current score is higher than the stored high score, update it
            if (score > hiscore)
            {
                hiscore = score;
                PlayerPrefs.SetFloat("hiscore", hiscore);
            }
           

            // Update the UI with the high score
            hiscoreText.text = Mathf.FloorToInt(hiscore).ToString("D5");
        }
    }

    // Triggered when the game is over
    public void GameOver()
    {
        // Stop the game speed increase and disable GameManager script
        gameSpeed = 0f;
        enabled = false;
       
        // Set up the death animation
        animator.SetBool("isGameOver", true);

        // Deactivate the obstacle spawner
        spawner.gameObject.SetActive(false);

        // Show game over UI elements
        gameOverText.gameObject.SetActive(true);
        retryButton.gameObject.SetActive(true);

        // Hide the jump button
        jumpButton.gameObject.SetActive(false);

        // Update the game over state
        isGameOver = true;

        // Update the high score display
        UpdateHiscore();
     if(!hasFinalScoreProcessed){
         ProcessFinalScore();

     }
       
    }

private void ProcessFinalScore()
{
    finalScore = Mathf.FloorToInt(score);      
    hasFinalScoreProcessed = true;
    SendScore(finalScore,2);

}
}
